using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class EnemyStatsManager : MonoBehaviour {
    [Header("Reusable Variable")]
    [SerializeField] private bool isRange;

    [Header("Player Stats Editor")]
    [SerializeField] private float defaultSpeed = 1f;
    [SerializeField] private float defaultHealth = 100000f;
    [SerializeField] private float baseRegenHP = 0.5f;
    [SerializeField] private float baseDamage = 100f;

    [Header("Character Stats")]
    public CharacterStat Speed;
    public CharacterStat Health;
    public CharacterStat MaxHealth;
    public CharacterStat HealthRegenValue;
    public CharacterStat BaseDamage;

    private EnemyUIController enemyUIController;
    private EnemyAI enemyAI;
    private CoroutineHandle regenHPCourotine;
    private StatModifier regenModifier;
    private StatModifier deductModifier;
    private bool isEnemyDead;
    private float difference;


    #region GetSet Properties
    public CoroutineHandle GetSetHPCoroutine {
        get { return regenHPCourotine; }
        set { regenHPCourotine = value; }
    }

    public bool GetSetIsEnemyDead {
        get { return isEnemyDead; }
        set { isEnemyDead = value; }
    }
   
    #endregion

    private void Awake() {
        enemyUIController = GetComponent<EnemyUIController>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void AddDefaultStats() {
        Health.ResetModifiers();
        Speed.ResetModifiers();

        Speed.BaseValue = defaultSpeed;
        Health.BaseValue = defaultHealth;
        MaxHealth.BaseValue = Health.Value;
        HealthRegenValue.BaseValue = baseRegenHP;
        BaseDamage.BaseValue = baseDamage;
    }

    public IEnumerator<float> RegenStatCoroutine(CharacterStat stat, CharacterStat maxStat, CharacterStat regenValue) {
        while (stat.Value < maxStat.Value && (int)stat.Value > 0 && enemyAI.GetSetEnemyParentContainer.activeSelf) {
            regenModifier = new StatModifier(regenValue.Value, Global.StatModType.Flat, this);
            stat.AddModifier(regenModifier);

            enemyUIController.UpdateHealthUI(_currentHP: Health.Value,_maxHP: MaxHealth.Value);

            if (stat.Value >= maxStat.Value) {
                RecalibrateStat(stat, maxStat);
            }

            yield return Timing.WaitForSeconds(0.2f);
        }

        Timing.PauseCoroutines(regenHPCourotine);
    }

    public IEnumerator<float> RegenHealth() {
        while (true) {
            if (Health.Value > 0 && Health.Value < MaxHealth.Value) {
                regenModifier = new StatModifier(HealthRegenValue.Value, Global.StatModType.Flat, this);
                Health.AddModifier(regenModifier);
            } else {
                RecalibrateStat(stat: Health,maxStat: MaxHealth);
                Timing.PauseCoroutines(regenHPCourotine);
            }

            yield return Timing.WaitForSeconds(0.2f);
        }
    }

    private void RecalibrateStat(CharacterStat stat, CharacterStat maxStat) {
        if (stat.Value > maxStat.Value) {
            difference = stat.Value - maxStat.Value;
            deductModifier = new StatModifier(-difference, Global.StatModType.Flat, this);
            stat.AddModifier(deductModifier);
        }
    }
}
