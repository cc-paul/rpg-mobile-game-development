using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp_AI : MonoBehaviour {
    private PlayerStatsManager playerStatsManager;
    private float speedValue;
    private float duration;
    private float timer = 0f;
    private Coroutine effecCoroutine;

    /* Stat Modifier */
    private StatModifier speed;

    #region GetSet Properties 
    public float GetSetSpeedValue {
        get { return speedValue; }
        set { speedValue = value; }
    }

    public float GetSetDuration {
        get { return duration; }
        set { duration = value; }
    }

    public PlayerStatsManager GetSetPlayerStatManager {
        get { return playerStatsManager; }
        set { playerStatsManager = value; }
    }
    #endregion

    private void OnEnable() {
        speed = new StatModifier(speedValue, Global.StatModType.Flat, this);
        playerStatsManager.Speed.AddModifier(speed);
        InitializeEffectDuration();
    }

    private void OnDisable() {
        playerStatsManager.Speed.RemoveModifier(speed);
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

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
