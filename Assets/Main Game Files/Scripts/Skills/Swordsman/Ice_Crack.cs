using UnityEngine;
using UnityEngine.Pool;

/*
 *  Note
 *  1. InitializeRerturnToNonCombat is default to 8 seconds
 *     make sure your skills duration are lesser
 * 
 */

public class Ice_Crack : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject snowAuraPreb;
    [SerializeField] private GameObject iceCrackPrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Ice_Crack.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ShowSnowAura() {
        GameObject snowAura = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(snowAuraPreb.name.ToString());
        snowAura.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        snowAura.SetActive(true);
        snowAura.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: snowAura);
    }

    public void ShowIceCrack() {
        GameObject iceCrack = skillBaseCast.GetSetObjectPoolManager.SpawnFromPool(iceCrackPrefab.name.ToString());
        iceCrack.transform.position = skillBaseCast.GetSetTargetManager.GetSetPlayerPosition;
        iceCrack.transform.rotation = Quaternion.LookRotation(
            skillBaseCast.GetSetTargetManager.GetSetLineSkillLook.transform.position -
            skillBaseCast.GetSetTargetManager.GetSetPlayerPosition
        );
        iceCrack.transform.rotation = Quaternion.Euler(0, iceCrack.transform.eulerAngles.y, iceCrack.transform.eulerAngles.z);
        iceCrack.SetActive(true);
        iceCrack.GetComponent<ReturnObjectToPool>().InitializeReturn(spawnedObject: iceCrack);
    }

    public void ApplyDamage() {
        float expectedDamage = skillBaseCast.GetSetPlayerStatsController.GetTotalBaseDamage() + 
                               skillBaseCast.GetSetSkillReference.GetSkillDamage(skillBaseCast.GetSetSkillID);
        bool isDamageApplied = false;

        /* For Mobs */
        GameObject controller;
        EnemyAI enemyAI;

        foreach (GameObject currentTarget in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            if (currentTarget.transform.Find(Global.CONTROLLER) != null) {
                /* Current Target is Mob */
                controller = currentTarget.transform.Find(Global.CONTROLLER).gameObject;
                enemyAI = controller.GetComponent<EnemyAI>();
                enemyAI.TakeDamage(playerStatsController: skillBaseCast.GetSetPlayerStatsController, damage: expectedDamage);
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
