using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject speedAuraPrefab;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Speed_Up.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ActivateEffect() {
        float expectedSpeed = skillBaseCast.GetSetSkillReference.GetSkillAddedSpeed(skillID: skillBaseCast.GetSetSkillID);
        float duration = skillBaseCast.GetSetSkillReference.GetSetDeactivationTime(skillID: skillBaseCast.GetSetSkillID);
        GameObject speedUpAura;
        GameObject buffEffectPool;
        SpeedUp_AI speedUp_AI;
        PlayerStatsManager playerStatsManager;

        foreach (GameObject currentTargetController in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            playerStatsManager = currentTargetController.transform.parent.Find(Global.GENERAL_SETTINGS).gameObject.GetComponent<PlayerStatsManager>();
            buffEffectPool = currentTargetController.transform.Find(Global.BUFF_EFFECT_POOL).gameObject;
            speedUpAura = buffEffectPool.transform.Find(speedAuraPrefab.name.ToString()).gameObject;

            speedUp_AI = speedUpAura.GetComponent<SpeedUp_AI>();
            speedUp_AI.GetSetSpeedValue = expectedSpeed;
            speedUp_AI.GetSetDuration = duration;
            speedUp_AI.GetSetPlayerStatManager = playerStatsManager;

            speedUpAura.SetActive(false);
            speedUpAura.SetActive(true);
        }
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}
