using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonSwapper : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Button buttonSwap;

    private GameObject skillButtonParent;
    private GameObject currentSkillButton;
    private int skillButtonListCount;
    private int startNumber = 1;
    private int endNumber = 4;
    private float parentSize = 100;
    private float holderSize = 50;
    private float textSize = 36;

    private void Awake() {
        skillButtonParent = gameObject;
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
    }

    private void HideShowButtonSkill(GameObject currentButton, bool showIt) {
        RectTransform buttonRect = currentButton.GetComponent<RectTransform>();
        GameObject background = currentButton.transform.Find(Global.BACKGROUND).gameObject;
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        RectTransform handleRect = currentButton.transform.Find(Global.HANDLE).GetComponent<RectTransform>();
        RectTransform timerImageRect = currentButton.transform.Find(Global.TIMER_IMAGE).GetComponent<RectTransform>();
        TextMeshProUGUI textCounter = currentButton.transform.Find(Global.COUNTER).GetComponent<TextMeshProUGUI>();

        Vector2 sizeDelta = showIt ? new Vector2(parentSize, parentSize) : Vector2.zero;
        Vector2 timerSizeDelta = showIt ? new Vector2(parentSize + 5, parentSize + 5) : Vector2.zero;

        buttonRect.sizeDelta = sizeDelta;
        backgroundRect.sizeDelta = sizeDelta;
        handleRect.sizeDelta = showIt ? new Vector2(holderSize, holderSize) : Vector2.zero;
        timerImageRect.sizeDelta = timerSizeDelta;
        textCounter.fontSize = showIt ? textSize : 0;
    }
}
