using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */

public class SwordsmanNormalAttack : MonoBehaviour {
    [Header("Normal Attack Settings")]
    [SerializeField] private int swordEndRange;

    private int attackRangePosition;
    private string attackaAnimationName;

    private SkillBaseCast skillBaseCast;

    private void Awake() {
        skillBaseCast = transform.parent.GetComponent<SkillBaseCast>();
    }

    private void Start() {
        swordEndRange = swordEndRange + 1;
    }

    public void ActivateSkill() {
        ActivateAnimation();
    }

    private void ActivateAnimation() {
        skillBaseCast.GetSetIsNormalAttacking = true;
        skillBaseCast.GetSetIsCastingSkill = true;
        skillBaseCast.InitializeCancelDelay();
        skillBaseCast.GetSetBasicAnimation.InitializeRerturnToNonCombat();
        skillBaseCast.GetSetTargetManager.LookAtNearestTarget();

        if (skillBaseCast.GetSetPlayerStatsManager.GetSetHasWeapon) {
            skillBaseCast.GetSetWeaponWeilding.ChangeWeaponVisibility(isAttackMode: true);
        }

        if (skillBaseCast.GetSetPlayerStatsManager.GetSetHasWeapon) {
            attackRangePosition = Random.Range(1,swordEndRange);
            attackaAnimationName = Global.SWORD_NORMAN_ATTACK + attackRangePosition;
        } else {
            //TODO: Normal attack animation if the player has no sword
        }

        //skillBaseCast.GetMessageBoxManager.ShowMessage(attackaAnimationName);

        skillBaseCast.GetSetAnimationPlayer.PlayAnimationByName(
            _currentAnimationName: attackaAnimationName,
            _isNormalAnimation: true
        );
    }

    public void StopNormalAttacking() {
        skillBaseCast.GetSetIsNormalAttacking = false;
    }

    public void ApplyDamage() {
        float expectedDamage = skillBaseCast.GetSetPlayerStatsController.GetTotalBaseDamage() +
                               skillBaseCast.GetSetSkillReference.GetSkillDamage(skillBaseCast.GetSetSkillID);
        bool isDamageApplied = false;

        /* Mobs Preferences */
        GameObject enemyController;
        GameObject currentTarget = null;
        EnemyAI enemyAI;

        for (int i = 0; i < skillBaseCast.GetSetTargetManager.GetTargetList().Count; i++) {
            currentTarget = skillBaseCast.GetSetTargetManager.GetTargetList()[i];

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
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}