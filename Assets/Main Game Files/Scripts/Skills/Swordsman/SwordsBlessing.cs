using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsBlessing : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject swordChargePrefab;
    [SerializeField] private GameObject swordDropPrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Swords_Blessing.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ShowSwordCharge() {
        GameObject swordCharge = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(swordChargePrefab.name.ToString());
        swordCharge.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        swordCharge.SetActive(true);
        swordCharge.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: swordCharge);
    }

    public void InitializeSwordDrop() {
        Invoke(nameof(DropSword),1f);
    }

    public void DropSword() {
        GameObject swordDrop;
        GameObject currentTarget;

        for (int i = 0; i < skillBaseCast.GetSetTargetManager.GetTargetList().Count; i++) {
            currentTarget = skillBaseCast.GetSetTargetManager.GetTargetList()[i];
            swordDrop = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(swordDropPrefab.name.ToString());
            swordDrop.transform.position = currentTarget.transform.position;
            swordDrop.SetActive(true);
            swordDrop.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: swordDrop);
        }

        ApplyDamage();
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
