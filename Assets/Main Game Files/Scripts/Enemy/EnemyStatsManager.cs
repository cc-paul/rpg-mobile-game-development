using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Coroutine regenHPCourotine;
    private bool isEnemyDead;


    #region GetSet Properties
    public Coroutine GetSetHPCoroutine {
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
        Speed.BaseValue = defaultSpeed;
        Health.BaseValue = defaultHealth;
        MaxHealth.BaseValue = Health.Value;
        HealthRegenValue.BaseValue = baseRegenHP;
        BaseDamage.BaseValue = baseDamage;
    }

    public IEnumerator RegenStatCoroutine(CharacterStat stat, CharacterStat maxStat, CharacterStat regenValue) {
        while (stat.Value < maxStat.Value && (int)stat.Value != 0) {
            StatModifier regenModifier = new StatModifier(regenValue.Value, Global.StatModType.Flat, this);
            stat.AddModifier(regenModifier);

            enemyUIController.UpdateHealthUI(_currentHP: Health.Value,_maxHP: MaxHealth.Value);

            if (stat.Value >= maxStat.Value) {
                RecalibrateStat(stat, maxStat);
                break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void RecalibrateStat(CharacterStat stat, CharacterStat maxStat) {
        if (stat.Value > maxStat.Value) {
            float difference = stat.Value - maxStat.Value;
            StatModifier deductModifier = new StatModifier(-difference, Global.StatModType.Flat, this);
            stat.AddModifier(deductModifier);
        }
    }
}
