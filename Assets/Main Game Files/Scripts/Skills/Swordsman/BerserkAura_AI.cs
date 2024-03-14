using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkAura_AI : MonoBehaviour {
    private PlayerStatsManager playerStatsManager;
    private PlayerStatsController playerStatsController;
    private float addedDamage;
    private float deductedSpeed;
    private float duration;
    private float timer = 0f;
    private Coroutine effecCoroutine;

    /* Stat Modifier */
    private StatModifier addedDamageValue;
    private StatModifier deductedSpeedValue;

    private WaitForSeconds skillNullDuration = new WaitForSeconds(0f);

    #region GetSet Properties 
    public float GetSetAddedDamage {
        get { return addedDamage; }
        set { addedDamage = value; }
    }

    public float GetSetDeductedSpeed {
        get { return deductedSpeed; }
        set { deductedSpeed = value; }
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
        addedDamageValue = new StatModifier(addedDamage, Global.StatModType.Flat, this);
        deductedSpeedValue = new StatModifier(-deductedSpeed, Global.StatModType.PercentMult, this);
        playerStatsManager.BaseDamage.AddModifier(addedDamageValue);
        playerStatsManager.Speed.AddModifier(deductedSpeedValue);

        InitializeEffectDuration();
    }

    private void OnDisable() {
        playerStatsManager.BaseDamage.RemoveModifier(addedDamageValue);
        playerStatsManager.Speed.RemoveModifier(deductedSpeedValue);
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

