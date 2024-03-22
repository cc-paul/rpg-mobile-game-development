using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHealAndManaRegen : MonoBehaviour {
    [Header("Variable Declarations and other assignments")]
    [SerializeField] private Global.RegenCategory regenCategory;
    [SerializeField] private float additionalHPRegen;
    [SerializeField] private float additionalMPRegen;

    private PlayerStatsManager playerStatsManager;

    public StatModifier HPRegenSpeed;
    public StatModifier MPRegenSpeed;

    private void OnTriggerEnter(Collider target) {
        playerStatsManager = target.transform.parent.Find(Global.GENERAL_SETTINGS).GetComponent<PlayerStatsManager>();

        if (regenCategory == Global.RegenCategory.HPRegen) {
            HPRegenSpeed = new StatModifier(-additionalHPRegen,Global.StatModType.PercentMult,this);
            playerStatsManager.BaseHPRegenSpeed.AddModifier(HPRegenSpeed);
        } else {
            MPRegenSpeed = new StatModifier(-additionalMPRegen,Global.StatModType.PercentMult, this);
            playerStatsManager.BaseMPRegenSpeed.AddModifier(MPRegenSpeed);
        }
    }

    private void OnTriggerExit(Collider target) {
        if (regenCategory == Global.RegenCategory.HPRegen) {
            playerStatsManager.BaseHPRegenSpeed.RemoveModifier(HPRegenSpeed);
        } else {
            playerStatsManager.BaseMPRegenSpeed.RemoveModifier(MPRegenSpeed);
        }
    }
}