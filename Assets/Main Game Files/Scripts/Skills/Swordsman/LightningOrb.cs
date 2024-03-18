using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */

public class LightningOrb : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject chargePrefab;
    [SerializeField] private GameObject lightningOrbPrefab;

    private SkillBaseCast skillBaseCast;

    private void Awake() {
        skillBaseCast = transform.parent.GetComponent<SkillBaseCast>();
    }

    public void ActivateSkill() {
        ActivateAnimation();
    }

    private void ActivateAnimation() {
        skillBaseCast.GetSetIsCastingSkill = true;
        skillBaseCast.InitializeCancelDelay();
        skillBaseCast.GetSetBasicAnimation.InitializeRerturnToNonCombat();
        skillBaseCast.GetSetTargetManager.LookAtNearestTarget();
        skillBaseCast.GetSetWeaponWeilding.ChangeWeaponVisibility(isAttackMode: true);

        skillBaseCast.GetSetAnimationPlayer.PlayAnimationByName(
            _currentAnimationName: Global.SwordsmanSkillAnimation.Lightning_Orb.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ShowCharge() {
        GameObject charge = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(chargePrefab.name.ToString());
        charge.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        charge.SetActive(true);
        charge.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: charge);
    }

    public void ShowLightningOrb() {
        GameObject lightningOrb;
        LightningOrb_AI lightningOrb_AI;
        GameObject currentTarget;

        for (int i = 0; i < skillBaseCast.GetSetTargetManager.GetTargetList().Count; i++) {
            currentTarget = skillBaseCast.GetSetTargetManager.GetTargetList()[i];
            lightningOrb = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(lightningOrbPrefab.name.ToString());
            lightningOrb.transform.position = new Vector3(
                skillBaseCast.GetSetTargetManager.GetSetPlayerPosition.x,
                skillBaseCast.GetSetTargetManager.GetSetPlayerPosition.y,
                skillBaseCast.GetSetTargetManager.GetSetPlayerPosition.z + 2f
            );
            lightningOrb.SetActive(true);

            lightningOrb_AI = lightningOrb.GetComponent<LightningOrb_AI>();
            lightningOrb_AI.GetSetTargetEnemy = currentTarget;
            lightningOrb_AI.GetSetLightningOrb = this;
            lightningOrb_AI.IntializeLightningOrb();
        }
    }

    public void ApplyDamage(GameObject currentTarget) { 
        float expectedDamage = skillBaseCast.GetSetPlayerStatsController.GetTotalBaseDamage() +
                               skillBaseCast.GetSetSkillReference.GetSkillDamage(skillBaseCast.GetSetSkillID) / 2;
        bool isDamageApplied = false;

        /* Mobs Preferences */
        GameObject enemyController;
        EnemyAI enemyAI;

        if (currentTarget.transform.Find(Global.CONTROLLER) != null) {
            /* If mobs is target */
            enemyController = currentTarget.transform.Find(Global.CONTROLLER).gameObject;
            enemyAI = enemyController.GetComponent<EnemyAI>();
            enemyAI.EnemyTakeDamage(
                playerStatsManager: skillBaseCast.GetSetPlayerStatsManager,
                playerStatsController: skillBaseCast.GetSetPlayerStatsController,
                damage: expectedDamage
            );
            isDamageApplied = true;
        } else if (currentTarget.transform.Find(Global.DUMMY) != null) {
            isDamageApplied = true;
        }

        if (isDamageApplied) {
            skillBaseCast.DisplayDamage(damageTextPosition: currentTarget.transform.position, damage: expectedDamage);
        }
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}
