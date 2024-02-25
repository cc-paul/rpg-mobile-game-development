using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */


public class GroundImpact : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject groundImpactPrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Ground_Impact.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ShowGroundImpact() {
        GameObject groundImpact;

        foreach (GameObject currentTarget in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            groundImpact = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(groundImpactPrefab.name.ToString());
            groundImpact.transform.position = currentTarget.transform.position;
            groundImpact.SetActive(true);
            groundImpact.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: groundImpact);
        }
    }

    public void ApplyDamage() {
        float expectedDamage = skillBaseCast.GetSetPlayerStatsController.GetTotalBaseDamage() +
                               skillBaseCast.GetSetSkillReference.GetSkillDamage(skillBaseCast.GetSetSkillID) / 3;
        bool isDamageApplied = false;

        /* For Mobs */
        GameObject controller;
        EnemyAI enemyAI;

        foreach (GameObject currentTarget in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            if (currentTarget.transform.Find(Global.CONTROLLER) != null) {
                /* Current Target is Mob */
                controller = currentTarget.transform.Find(Global.CONTROLLER).gameObject;
                enemyAI = controller.GetComponent<EnemyAI>();
                enemyAI.TakeDamage(playerStatsController: skillBaseCast.GetSetPlayerStatsController,damage: expectedDamage);
                isDamageApplied = true;
            } else if (currentTarget.transform.Find(Global.DUMMY) != null) {
                isDamageApplied = true;
            }

            if (isDamageApplied) {
                skillBaseCast.DisplayDamage(damageTextPosition: currentTarget.transform.position, damage: expectedDamage);
            }
        }
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}
