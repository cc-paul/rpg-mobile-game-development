using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    TODO: 
    1. Character Stats must be API generated
 */


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

    private WaitForSeconds regenDelayTimer = new WaitForSeconds(0.2f);
    private List<GameObject> currentMobsFollowingMe = new List<GameObject>();
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

    public List<GameObject> GetSetCurrentMobsFollowingMe {
        get { return currentMobsFollowingMe; }
        set { currentMobsFollowingMe = value; }
    }
    #endregion

    private void Awake() {
        playerStatsController = GetComponent<PlayerStatsController>();

        if (useInspectorStats) {
            //TODO: Uncheck this or if ever can remove because this is API based
            AddDefaultStats();
        }  
    }

    private void OnEnable() {
        currentMobsFollowingMe.Clear();
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

    public IEnumerator RegenStatCoroutine(
        CharacterStat stat,
        CharacterStat maxStat,
        CharacterStat regenValue,
        string regenCategory
    ) {
        while (stat.Value < maxStat.Value) {
            StatModifier regenModifier = new StatModifier(regenValue.Value, Global.StatModType.Flat, this);
            stat.AddModifier(regenModifier);

            playerStatsController.UpdateHealthUI();
            //TODO: Stop also the regen if the player is dead

            if (stat.Value >= maxStat.Value) {
                if (regenCategory == Global.RegenCategory.HPRegen.ToString()) {
                    regenHPCourotine = null;
                } else if (regenCategory == Global.RegenCategory.MPRegen.ToString()) {
                    regenMPCourotine = null;
                }

                RecalibrateStat(stat,maxStat);
                break;
            }

            yield return regenDelayTimer;
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