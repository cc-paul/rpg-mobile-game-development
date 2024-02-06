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

    private ModelInfoDisplay modelInfoDisplay;

    #region GetSet Properties
    public Global.Gender GetSetGender {
        get { return characterGender; }
        set { characterGender = value; }
    }

    public Global.Characters GetSetCharacterType {
        get { return characterType; }
        set { characterType = value; }
    }

    public bool GetSetHasWeapon {
        get { return hasWeapon; }
        set { hasWeapon = value; }
    }
    #endregion

    private void Awake() {
        modelInfoDisplay = GetComponent<ModelInfoDisplay>();

        if (useInspectorStats) {
            AddDefaultStats();
        }
    }

    private void Start() {
        modelInfoDisplay.DisplayCharacterDetails(
            _characterName: characterName,
            _clanName: clanName,
            _hasClan: hasClan
        );

        modelInfoDisplay.DisplayLifeStats(
            _currentHP: Health.Value,
            _maxHP: MaxHealth.Value,
            _currentMP: MP.Value,
            _maxMP: MaxMP.Value
        );
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
}