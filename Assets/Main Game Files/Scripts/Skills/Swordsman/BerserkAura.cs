using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkAura : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject berserkAuraPrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Berserk_Aura.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ActivateEffect() {
        float expectedAddedDamage = skillBaseCast.GetSetSkillReference.GetAddedDamage(skillID: skillBaseCast.GetSetSkillID);
        float expectedDeductedSpeed = skillBaseCast.GetSetSkillReference.GetDeductedSpeed(skillID: skillBaseCast.GetSetSkillID);
        float duration = skillBaseCast.GetSetSkillReference.GetSetDeactivationTime(skillID: skillBaseCast.GetSetSkillID);
        GameObject berserkAura;
        GameObject buffEffectPool;
        BerserkAura_AI berserkAura_AI;
        PlayerStatsManager playerStatsManager;

        foreach (GameObject currentTargetController in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            playerStatsManager = currentTargetController.transform.parent.Find(Global.GENERAL_SETTINGS).gameObject.GetComponent<PlayerStatsManager>();
            buffEffectPool = currentTargetController.transform.Find(Global.BUFF_EFFECT_POOL).gameObject;
            berserkAura = buffEffectPool.transform.Find(berserkAuraPrefab.name.ToString()).gameObject;

            berserkAura_AI = berserkAura.GetComponent<BerserkAura_AI>();
            berserkAura_AI.GetSetAddedDamage = expectedAddedDamage;
            berserkAura_AI.GetSetDeductedSpeed = expectedDeductedSpeed;
            berserkAura_AI.GetSetDuration = duration;
            berserkAura_AI.GetSetPlayerStatManager = playerStatsManager;

            berserkAura.SetActive(false);
            berserkAura.SetActive(true);
        }
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}
