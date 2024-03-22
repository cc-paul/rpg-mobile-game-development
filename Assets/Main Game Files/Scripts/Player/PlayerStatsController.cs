using System.Collections;
using MEC;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour {
    private PlayerStatsManager playerStatsManager;
    private ModelInfoDisplay modelInfoDisplay;

    private Coroutine coroutineTakeContiniousDamage;
    private Coroutine coroutineTakeContiniousHeal;
    private Component sourceComponent;

    private StatModifier damageStat;
    private StatModifier healthStat;
    private StatModifier mpStat;
    private SkillBaseCast skillBaseCast;


    #region GetSet Properties
    public Component GetSetSourceComponent {
        get { return sourceComponent; }
        set { sourceComponent = value; }
    }
    #endregion

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
        modelInfoDisplay = GetComponent<ModelInfoDisplay>();
        skillBaseCast = transform.parent.Find(Global.SKILL_SETTINGS).GetComponent<SkillBaseCast>();
    }

    private void Start() {
        DisplayCharacterDetails();
        UpdateHealthUI();
    }

    public void ReceiveDamage(float _damageAmount,Component _sourceComponent) {
        damageStat = new StatModifier(-_damageAmount, Global.StatModType.Flat, this);
        playerStatsManager.Health.AddModifier(damageStat);
        skillBaseCast.DisplayDamage(damageTextPosition:skillBaseCast.GetSetTargetManager.GetSetPlayerPosition,damage:_damageAmount);
        UpdateHealthUI();

        /*
           TODO List : Do somthing if the players health reaches to zero and.
           1. Who is the source
           2. What is the current situation
        */
        if (playerStatsManager.Health.Value > 0 && playerStatsManager.Health.Value < playerStatsManager.MaxHealth.Value) {
            Timing.ResumeCoroutines(playerStatsManager.GetSetHPCoroutine);

            if (_sourceComponent as TestHealAndManaRegen) {

            }
        } else {
            
        }
    }

    private void ReceiveHealth(float _healthAmount) {
        healthStat = new StatModifier(_healthAmount, Global.StatModType.Flat, this);
        playerStatsManager.Health.AddModifier(healthStat);
        UpdateHealthUI();
    }

    public void DeductMP(float _mpAmount) {
        mpStat = new StatModifier(-_mpAmount, Global.StatModType.Flat, this);
        playerStatsManager.MP.AddModifier(mpStat);
        UpdateHealthUI();
        Timing.ResumeCoroutines(playerStatsManager.GetSetMPCoroutine);
    }

    private void DisplayCharacterDetails() {
        modelInfoDisplay.DisplayCharacterDetails(
            _characterName: playerStatsManager.GetSetCharacterName,
            _clanName: playerStatsManager.GetSetClanName,
            _hasClan: playerStatsManager.GetSetHasClan
        );
    }

    public void UpdateHealthUI() {
        modelInfoDisplay.DisplayLifeStats(
            _currentHP: playerStatsManager.Health.Value,
            _maxHP: playerStatsManager.MaxHealth.Value,
            _currentMP: playerStatsManager.MP.Value,
            _maxMP: playerStatsManager.MaxMP.Value
        );
    }

    public float GetTotalBaseDamage() {
        return playerStatsManager.BaseDamage.Value;
    }
}