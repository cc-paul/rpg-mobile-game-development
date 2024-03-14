using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp_AI : MonoBehaviour {
    [Header("TODO: This is for testing only. Use network for this one")]
    [SerializeField] private bool updateMainUI;

    private PlayerStatsManager playerStatsManager;
    private PlayerStatsController playerStatsController;
    private float maxHealthValue;
    private float duration;
    private float timer = 0f;
    private Coroutine effecCoroutine;

    /* Stat Modifier */
    private StatModifier maxHealth;

    private WaitForSeconds skillNullDuration = new WaitForSeconds(0f);

    #region GetSet Properties 
    public float GetSetMaxHealthValue {
        get { return maxHealthValue; }
        set { maxHealthValue = value; }
    }

    public float GetSetDuration {
        get { return duration; }
        set { duration = value; }
    }

    public PlayerStatsManager GetSetPlayerStatManager {
        get { return playerStatsManager; }
        set { playerStatsManager = value; }
    }

    public PlayerStatsController GetSetPlayerStatsController {
        get { return playerStatsController; }
        set { playerStatsController = value; }
    }
    #endregion

    private void OnEnable() {
        maxHealth = new StatModifier(maxHealthValue, Global.StatModType.Flat, this);
        playerStatsManager.MaxHealth.AddModifier(maxHealth);

        if (updateMainUI) {
            //TODO: As of now it only reflects on self need to check if on a party online
            playerStatsController.UpdateHealthUI();
            playerStatsController.RegenHP();
        }

        InitializeEffectDuration();
    }

    private void OnDisable() {
        playerStatsManager.MaxHealth.RemoveModifier(maxHealth);

        if (updateMainUI) {
            //TODO: As of now it only reflects on self need to check if on a party online
            playerStatsManager.RecalibrateStat(stat: playerStatsManager.Health,maxStat: playerStatsManager.MaxHealth);
            playerStatsController.UpdateHealthUI();
        }
    }

    private void InitializeEffectDuration() {
        if (effecCoroutine != null) {
            StopCoroutine(effecCoroutine);
        }

        timer = 0f;
        effecCoroutine = StartCoroutine(nameof(StartEffectDuration));
    }

    private IEnumerator StartEffectDuration() {
        while (timer < duration) {
            timer += Time.deltaTime;

            //TODO: Add code to remove the buffs if the player has been dead

            yield return skillNullDuration;
        }

        gameObject.SetActive(false);
    }
}
