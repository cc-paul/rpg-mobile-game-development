using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindowDurationSetter : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownProgress;

    private PlayerStatsManager playerStatsManager;
    private Coroutine cooldownCoroutine;
    private Sprite skillSprite;
    private float duration;
    private float timer;
    private int skillID = 0;

    #region GetSet Propeties
    public Sprite GetSetSkillSprite {
        get { return skillSprite; }
        set { skillSprite = value; }    
    }

    public float GetSetDuration {
        get { return duration; }
        set { duration = value; }
    }

    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }

    public PlayerStatsManager GetPlayerStatsManager {
        get { return playerStatsManager; }
        set { playerStatsManager = value; }
    }
    #endregion

    private void OnEnable() {
        InititalizeDurationWindowItem();
    }

    public void InititalizeDurationWindowItem() {
        icon.sprite = skillSprite;
        timer = 0;

        if (cooldownCoroutine != null) {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(nameof(StartDuration));
    }

    private IEnumerator StartDuration() {
        while (timer < duration) {
            cooldownProgress.fillAmount = 1 - (timer / duration);
            timer += Time.deltaTime;

            //TODO: Add code to remove the buffs if the player has been dead

            yield return null;
        }

        gameObject.SetActive(false);
    }
}