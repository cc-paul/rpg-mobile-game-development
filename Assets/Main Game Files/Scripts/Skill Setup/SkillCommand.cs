using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCommand : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image skillBackground;
    [SerializeField] private Image timerImageCooldown;
    [SerializeField] private Sprite defaultBackground;

    [Header("Game Objects")]
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject skillSettings;

    [Header("Variable Declarations and other assignment")]
    [SerializeField] private bool isNormalAttack;

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
    #endregion

    private void Awake() {
        skillReference = skillSettings.GetComponent<SkillReference>();
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

    public void OnCastingSkill() {

    }

    public void OnSkillCasted() {


        //TODO: Use the function below only after the player casted a skill
        expectedCooldown = skillReference.GetSkillDefaultCoolDown(skillID: skillID);
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
        timerImageCooldown.gameObject.SetActive(true);
        cooldownCoroutine = StartCoroutine(nameof(StartCoolDown));
    }

    private IEnumerator StartCoolDown() {
        while (timer < expectedCooldown) {
            timerImageCooldown.fillAmount = 1 - (timer / expectedCooldown);
            timer += Time.deltaTime;
            yield return null;
        }

        timerImageCooldown.gameObject.SetActive(false);
    }
}