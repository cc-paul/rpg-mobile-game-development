using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */

public class IceDownWave : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject snowLightPrefab;
    [SerializeField] private GameObject circleSnowPrefab;
    [SerializeField] private GameObject iceclePrfab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Ice_Down_Wave.ToString(),
            _isNormalAnimation: false
        );
    }

    public void showIceGlow() {
        GameObject iceGlow = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(snowLightPrefab.name.ToString());
        iceGlow.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        iceGlow.SetActive(true);
        iceGlow.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: iceGlow);
    }

    public void showSnow() {
        GameObject snow = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(circleSnowPrefab.name.ToString());
        snow.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        snow.SetActive(true);
        snow.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: snow);
    }

    public void ShowIcecle() {
        GameObject icecle ;
        GameObject currentTarget;

        for (int i = 0; i < skillBaseCast.GetSetTargetManager.GetTargetList().Count; i++) {
            currentTarget = skillBaseCast.GetSetTargetManager.GetTargetList()[i];
            icecle = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(iceclePrfab.name.ToString());
            icecle.transform.position = currentTarget.transform.position;
            icecle.SetActive(true);
            icecle.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: icecle);
        }
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
                /*enemyController = currentTarget.transform.Find(Global.CONTROLLER).gameObject;
                enemyAI = enemyController.GetComponent<EnemyAI>();
                enemyAI.EnemyTakeDamage(
                    playerStatsManager: skillBaseCast.GetSetPlayerStatsManager,
                    playerStatsController: skillBaseCast.GetSetPlayerStatsController,
                    damage: expectedDamage
                );*/
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
