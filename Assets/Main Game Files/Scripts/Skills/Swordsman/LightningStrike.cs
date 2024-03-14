using UnityEngine;
using UnityEngine.Pool;

/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */

public class LightningStrike : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private GameObject magicCirclePrefab;
    [SerializeField] private GameObject lightningStrikePrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Lightning_Strike.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ShowCloud() {
        GameObject cloud = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(cloudPrefab.name.ToString());
        cloud.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        cloud.SetActive(true);
        cloud.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: cloud);
    }

    public void ShowMagicCircle() {
        GameObject magicCircle = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(magicCirclePrefab.name.ToString());
        magicCircle.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        magicCircle.SetActive(true);
        magicCircle.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: magicCircle);
    }

    public void ShowLightning() {
        GameObject lightning;
        GameObject currentTarget;

        for (int i = 0; i < skillBaseCast.GetSetTargetManager.GetTargetList().Count; i++) {
            currentTarget = skillBaseCast.GetSetTargetManager.GetTargetList()[i];
            lightning = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(lightningStrikePrefab.name.ToString());
            lightning.transform.position = currentTarget.transform.position;
            lightning.SetActive(true);
            lightning.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: lightning);
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
