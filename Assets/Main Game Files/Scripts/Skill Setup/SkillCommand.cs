using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCommand : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image skillBackground;
    [SerializeField] private Image timerImageCooldown;
    [SerializeField] private Sprite defaultBackground;

    [Header("Game Objects and otherts")]
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject lineTargetIndicator;
    [SerializeField] private GameObject areaTargetIndicator;

    [Header("Variable Declarations and other assignment")]
    [SerializeField] private bool isNormalAttack;

    private GameObject currentTargetIndicator;
    private TargetManager targetManager;
    private SkillSetup skillSetup;
    private SkillReference skillReference;
    private DateAndTime dateAndTime;
    private Coroutine cooldownCoroutine;
    private int skillID = 0;
    private float expectedCooldown = 0;
    private float timer;
    private float percentage;
    private string prefCooldownRef;
    private bool isSkillCasted = false;
    private bool showTimerImage;
    private bool isCoolingDown;

    #region GetSet Properties
    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }

    public string GetSetPrefCooldownRef {
        get { return prefCooldownRef; }
        set { prefCooldownRef = value; }
    }

    public bool GetSetIsNormalAttack {
        get { return isNormalAttack; }
        set { isNormalAttack = value; }
    }

    public bool GetSetIsCoolingDown {
        get { return isCoolingDown; }
        set { isCoolingDown = value; }
    }
    #endregion

    private void Awake() {
        targetManager = skillSettings.GetComponent<TargetManager>();
        skillReference = skillSettings.GetComponent<SkillReference>();
        skillSetup = skillSettings.GetComponent<SkillSetup>();
        dateAndTime = gameManager.GetComponent<DateAndTime>();        
    }

    private void Start() {
        if (isNormalAttack) {
            timerImageCooldown.gameObject.SetActive(false);
        }
    }

    public void SetSkillSprite(Sprite skillSprite) {
        skillBackground.sprite = skillSprite == null ? defaultBackground : skillSprite;
    }

    public void OnBeforeCast() {
        currentTargetIndicator = skillReference.GetTargetIndicator(skillID: skillID) == Global.TargetIndicator.Line.ToString() ? lineTargetIndicator : areaTargetIndicator;
        currentTargetIndicator.SetActive(true);

        if (currentTargetIndicator == areaTargetIndicator) {
            float scale = skillReference.GetSkillTargetRanger(skillID: skillID);
            GameObject areaParentTarget = currentTargetIndicator.transform.GetChild(0).gameObject;
            GameObject areaChildTarget = areaParentTarget.transform.GetChild(0).gameObject;
            AreaTarget areaTarget = areaChildTarget.GetComponent<AreaTarget>();
            RangeAdjust rangeAdjust = areaParentTarget.GetComponent<RangeAdjust>();

            rangeAdjust.GetSetRange = scale;
            rangeAdjust.DoTheRescaling();
            areaTarget.ResizeTheCollider();
        } else {
            GameObject lineParentTarget = currentTargetIndicator.transform.GetChild(0).gameObject;
            GameObject lineRangePivot = lineParentTarget.transform.GetChild(0).gameObject;
            LineTarget lineTarget = lineRangePivot.GetComponent<LineTarget>();

            lineTarget.ResizeTheCollider();
        }
    }

    public void OnCastingSkill(SkillJoystick skillJoystick) {
        if (currentTargetIndicator == areaTargetIndicator) {
            GameObject areaParentTarget = currentTargetIndicator.transform.GetChild(0).gameObject;
            GameObject childTarget = areaParentTarget.transform.GetChild(0).gameObject;
            AreaTarget areaTarget = childTarget.GetComponent<AreaTarget>();

            areaTarget.ControlTheChildTarget(skillJoystick: skillJoystick);
        } else {
            GameObject lineParentTarget = currentTargetIndicator.transform.GetChild(0).gameObject;
            GameObject lineRangePivot = lineParentTarget.transform.GetChild(0).gameObject;
            LineTarget lineTarget = lineRangePivot.GetComponent<LineTarget>();

            lineTarget.ControlTheChildTarget(skillJoystick: skillJoystick);
        }
    }

    public void OnSkillCasted() {
        //TODO: Use the function below only after the player casted a skill
        expectedCooldown = skillReference.GetSkillDefaultCoolDown(skillID: skillID);
        currentTargetIndicator.SetActive(false);
        targetManager.HideAllTargetIndicators();
        SetSkillLastUse();
        InitializeCooldown();
    }

    private void SetSkillLastUse() {
        PlayerPrefs.SetString(prefCooldownRef,dateAndTime.GetSkillReferenceDateTime());
        PlayerPrefs.Save();
    }

    public void RecalibrateCooldown() {
        if (isNormalAttack) {
            showTimerImage = false;
        } else {
            if (PlayerPrefs.HasKey(prefCooldownRef)) {
                expectedCooldown = skillReference.GetSkillCoolDown(
                    skillID: skillID,
                    currentDateTime: dateAndTime.GetSkillReferenceDateTime(),
                    skillDateTime: PlayerPrefs.GetString(prefCooldownRef)
                );
                showTimerImage = expectedCooldown > 0f;

                if (showTimerImage) {
                    InitializeCooldown();
                }
            } else {
                showTimerImage = false;
            }
        }

        timerImageCooldown.gameObject.SetActive(showTimerImage);
    }

    public void InitializeCooldown() {
        if (cooldownCoroutine != null) {
            StopCoroutine(cooldownCoroutine);
        }

        timer = 0f;
        isCoolingDown = true;
        timerImageCooldown.gameObject.SetActive(true);
        cooldownCoroutine = StartCoroutine(nameof(StartCoolDown));
    }

    private IEnumerator StartCoolDown() {
        while (timer < expectedCooldown) {
            timerImageCooldown.fillAmount = 1 - (timer / expectedCooldown);
            timer += Time.deltaTime;
            yield return null;
        }

        isCoolingDown = false;
        timerImageCooldown.gameObject.SetActive(false);
    }
}