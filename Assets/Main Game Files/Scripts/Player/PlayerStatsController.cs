using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour {
    private PlayerStatsManager playerStatsManager;
    private ModelInfoDisplay modelInfoDisplay;

    private Coroutine coroutineTakeContiniousDamage;
    private Coroutine coroutineTakeContiniousHeal;
    private Component sourceComponent;

    private StatModifier damageStat;
    private StatModifier healthStat;

    #region GetSet Properties
    public Component GetSetSourceComponent {
        get { return sourceComponent; }
        set { sourceComponent = value; }
    }
    #endregion

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
        modelInfoDisplay = GetComponent<ModelInfoDisplay>();
    }

    private void Start() {
        DisplayCharacterDetails();
        UpdateHealthUI();
    }

    private void ReceiveDamage(float _damageAmount) {
        damageStat = new StatModifier(-_damageAmount, Global.StatModType.Flat, this);
        playerStatsManager.Health.AddModifier(damageStat);
        UpdateHealthUI();

        /*
           TODO List : Do somthing if the players health reaches to zero and.
           1. Who is the source
           2. What is the current situation
        */
        if (playerStatsManager.Health.Value <= 0) {
            if (playerStatsManager.GetSetHPCoroutine != null) {
                StopCoroutine(playerStatsManager.GetSetHPCoroutine);
            }

            if (sourceComponent as TestHealAndDamage) {

            }
        } else {
            RegenThePlayer();
        }
    }

    private void ReceiveHealth(float _healthAmount) {
        healthStat = new StatModifier(_healthAmount, Global.StatModType.Flat, this);
        playerStatsManager.Health.AddModifier(healthStat);
        UpdateHealthUI();
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

    public void RegenThePlayer() {
        if (playerStatsManager.GetSetHPCoroutine != null) {
            StopCoroutine(playerStatsManager.GetSetHPCoroutine);
        }

        playerStatsManager.GetSetHPCoroutine = StartCoroutine(
            playerStatsManager.RegenStatCoroutine(
                playerStatsManager.Health,
                playerStatsManager.MaxHealth,
                playerStatsManager.HealthRegenValue
            )
        );
    }

    public void InitializeContiniousDamage(Component _sourceComponent) {
        sourceComponent = _sourceComponent;
        StopContiniousDamage();
        coroutineTakeContiniousDamage = StartCoroutine(nameof(TakeContiniousDamage));
    }

    public void StopContiniousDamage() {
        if (coroutineTakeContiniousDamage != null) {
            StopCoroutine(coroutineTakeContiniousDamage);
        }
    }

    private IEnumerator TakeContiniousDamage() {
        while (true) {
            ReceiveDamage(_damageAmount: Random.Range(100f,800f));

            if (playerStatsManager.Health.Value <= 0f) {
                break;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    public void InitializeContiniousHeal(Component _sourceComponent) {
        sourceComponent = _sourceComponent;
        StopContiniousHeal();

        coroutineTakeContiniousHeal = StartCoroutine(nameof(TakeContiniousHeal));
    }

    public void StopContiniousHeal() {
        if (coroutineTakeContiniousHeal != null) {
            StopCoroutine(coroutineTakeContiniousHeal);
        }
    }

    private IEnumerator TakeContiniousHeal() {
        while (true) {
            ReceiveHealth(_healthAmount: Random.Range(100f, 800f));

            if (playerStatsManager.Health.Value >= playerStatsManager.MaxHealth.Value) {
                playerStatsManager.RecalibrateStat(playerStatsManager.Health,playerStatsManager.MaxHealth);
                UpdateHealthUI();
                break;
            }

            yield return new WaitForSeconds(4f);
        }
    }
}