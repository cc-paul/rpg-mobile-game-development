using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour {
    [Header("Reusable Variable")]
    [SerializeField] private bool useInspectorStats;
    [SerializeField] private bool showLogs;
    [SerializeField] private bool hasWeapon;
    [SerializeField] private int maxTargetToLure = 5;

    [Space(2)]

    [Header("Player Stats Editor")]
    [SerializeField] private Global.Characters characterType;
    [SerializeField] private Global.Gender characterGender;
    [SerializeField] private int playerID;
    [SerializeField] private float defaultSpeed = 2f;
    [SerializeField] private float defaultHealth = 100000f;
    [SerializeField] private float defaultMP = 100f;
    [SerializeField] private float baseRegenHP = 0.5f;
    [SerializeField] private float baseRegenMP = 0.5f;
    [SerializeField] private float baseDamage = 100f;
    [SerializeField] private bool hasClan = false;
    [SerializeField] private string characterName = "";
    [SerializeField] private string clanName = "";

    [Space(2)]

    [Header("Sword Settings")]
    [SerializeField] private bool isLongSword;

    [Space(2)]

    [Header("Character Stats")]
    public CharacterStat Speed;
    public CharacterStat Health;
    public CharacterStat MaxHealth;
    public CharacterStat MP;
    public CharacterStat MaxMP;
    public CharacterStat HealthRegenValue;
    public CharacterStat MPRegenValue;
    public CharacterStat BaseDamage;

    private PlayerStatsController playerStatsController;
    private Coroutine regenMPCourotine;
    private Coroutine regenHPCourotine;
    private bool isPlayerDead;

    #region Character GetSet Properties
    public Global.Characters GetSetCharacterType {
        get { return characterType; }
        set { characterType = value; }
    }

    public int GetSetPlayerID {
        get { return playerID; }
        set { playerID = value; }
    }

    public Global.Gender GetSetGender {
        get { return characterGender; }
        set { characterGender = value; }
    }

    public string GetSetCharacterName {
        get { return characterName; }
        set { characterName = value; }
    }

    public string GetSetClanName {
        get { return clanName; }
        set { clanName = value; }
    }

    public bool GetSetHasClan {
        get { return hasClan; }
        set { hasClan = value; }
    }

    public bool GetSetIsLongSword {
        get { return isLongSword; }
        set { isLongSword = value; }
    }

    public bool GetSetHasWeapon {
        get { return hasWeapon; }
        set { hasWeapon = value; }
    }

    public int GetSetMaxTargetToLure {
        get { return maxTargetToLure; }
        set { maxTargetToLure = value; }
    }

    public Coroutine GetSetHPCoroutine {
        get { return regenHPCourotine; }
        set { regenHPCourotine = value; }
    }

    public Coroutine GetSetMPCoroutine {
        get { return regenMPCourotine; }
        set { regenMPCourotine = value; }
    }

    public bool GetSetIsPlayerDead {
        get { return isPlayerDead; }
        set { isPlayerDead = value; }
    }
    #endregion

    private void Awake() {
        playerStatsController = GetComponent<PlayerStatsController>();

        if (useInspectorStats) {
            AddDefaultStats();
        }
    }

    private void AddDefaultStats() {
        playerID = 0;
        characterGender = Global.Gender.Male;
        characterType = Global.Characters.Swordsman;

        Speed.BaseValue = defaultSpeed;
        Health.BaseValue = defaultHealth;
        MaxHealth.BaseValue = Health.Value;
        MP.BaseValue = defaultMP;
        MaxMP.BaseValue = MP.Value;
        HealthRegenValue.BaseValue = baseRegenHP;
        MPRegenValue.BaseValue = baseRegenMP;
        BaseDamage.BaseValue = baseDamage;
    }

    public IEnumerator RegenStatCoroutine(CharacterStat stat, CharacterStat maxStat, CharacterStat regenValue) {
        while (stat.Value < maxStat.Value && stat.Value > 0) {
            StatModifier regenModifier = new StatModifier(regenValue.Value, Global.StatModType.Flat, this);
            stat.AddModifier(regenModifier);

            playerStatsController.UpdateHealthUI();

            if (stat.Value >= maxStat.Value) {
                RecalibrateStat(stat,maxStat);
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