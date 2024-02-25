using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonSwapper : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject skillButtonParent;

    [Header("UI")]
    [SerializeField] private Button buttonSwap;
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject line2;
    [SerializeField] private GameObject line3;

    private TargetManager targetManager;
    private GameObject currentSkillButton;
    private int skillButtonListCount;
    private int startNumber = 1;
    private int endNumber = 4;
    private float parentSize = 100;
    private float holderSize = 50;
    private float textSize = 36;

    private void Awake() {
        targetManager = skillSettings.GetComponent<TargetManager>();
    }

    private void Start() {
        skillButtonListCount = skillButtonParent.transform.childCount - 1;
        UpdateSkillButtonList();
    }

    private void OnEnable() {
        buttonSwap.onClick.AddListener(() => {
            if (endNumber < 12) {
                startNumber = Mathf.Min(endNumber + 1, skillButtonListCount);
                endNumber = Mathf.Min(endNumber + 4, skillButtonListCount);
            } else {
                startNumber = 1;
                endNumber = 4;
            }

            targetManager.HideTargetContainer();
            targetManager.HideAllTargetIndicators();
            UpdateSkillButtonList();
        });
    }

    private void UpdateSkillButtonList() {
        for (int i = 1; i <= skillButtonListCount; i++) {
            currentSkillButton = skillButtonParent.transform.
                Find(Global.BUTTON_SKILL_NAME + i).
                gameObject;

            HideShowButtonSkill(currentSkillButton, i >= startNumber && i <= endNumber);
        }

        line1.SetActive(endNumber == 4);
        line2.SetActive(endNumber == 8);
        line3.SetActive(endNumber == 12);
    }

    private void HideShowButtonSkill(GameObject currentButton, bool showIt) {
        RectTransform buttonRect = currentButton.GetComponent<RectTransform>();
        GameObject background = currentButton.transform.Find(Global.BACKGROUND).gameObject;
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        RectTransform borderRect = currentButton.transform.Find(Global.BORDER).GetComponent<RectTransform>();
        RectTransform handleRect = currentButton.transform.Find(Global.HANDLE).GetComponent<RectTransform>();
        RectTransform timerImageRect = currentButton.transform.Find(Global.TIMER_IMAGE).GetComponent<RectTransform>();
        TextMeshProUGUI textCounter = currentButton.transform.Find(Global.COUNTER).GetComponent<TextMeshProUGUI>();

        Vector2 sizeDelta = showIt ? new Vector2(parentSize, parentSize) : Vector2.zero;
        Vector2 timerSizeDelta = showIt ? new Vector2(parentSize, parentSize) : Vector2.zero;
        Vector2 borderSizeDelta = showIt ? new Vector2(parentSize + 10f, parentSize + 10f) : Vector2.zero;

        borderRect.sizeDelta = borderSizeDelta;
        buttonRect.sizeDelta = sizeDelta;
        backgroundRect.sizeDelta = sizeDelta;
        handleRect.sizeDelta = showIt ? new Vector2(holderSize, holderSize) : Vector2.zero;
        timerImageRect.sizeDelta = timerSizeDelta;
        textCounter.fontSize = showIt ? textSize : 0;
    }
}
