using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : MonoBehaviour {
    [Header("Skill Effect Prefab")]
    [SerializeField] private GameObject healthUpPrefab;

    private SkillBaseCast skillBaseCast;
    float duration;

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
            _currentAnimationName: Global.SwordsmanSkillAnimation.Health_Up.ToString(),
            _isNormalAnimation: false
        );
    }

    public void ActivateEffect() {
        float expectedMaxHealth = skillBaseCast.GetSetSkillReference.GetSkillAddedHP(skillID: skillBaseCast.GetSetSkillID);
        duration = skillBaseCast.GetSetSkillReference.GetSetDeactivationTime(skillID: skillBaseCast.GetSetSkillID);
        GameObject healthUpAura;
        GameObject buffEffectPool;
        HealthUp_AI healthUp_AI;
        PlayerStatsManager playerStatsManager;
        PlayerStatsController playerStatsController;

        foreach (GameObject currentTargetController in skillBaseCast.GetSetTargetManager.GetTargetList()) {
            playerStatsManager = currentTargetController.transform.parent.Find(Global.GENERAL_SETTINGS).gameObject.GetComponent<PlayerStatsManager>();
            playerStatsController = currentTargetController.transform.parent.Find(Global.GENERAL_SETTINGS).gameObject.GetComponent<PlayerStatsController>();
            buffEffectPool = currentTargetController.transform.Find(Global.BUFF_EFFECT_POOL).gameObject;
            healthUpAura = buffEffectPool.transform.Find(healthUpPrefab.name.ToString()).gameObject;

            healthUp_AI = healthUpAura.GetComponent<HealthUp_AI>();
            healthUp_AI.GetSetMaxHealthValue = expectedMaxHealth;
            healthUp_AI.GetSetDuration = duration;
            healthUp_AI.GetSetPlayerStatManager = playerStatsManager;
            healthUp_AI.GetSetPlayerStatsController = playerStatsController;

            healthUpAura.SetActive(false);
            healthUpAura.SetActive(true);
        }
    }

    public void DisplayDurationInMainUI() {
        string skillObjectNameToCall = $"{skillBaseCast.GetSetPlayerStatsManager.GetSetCharacterType}_{skillBaseCast.GetSetSkillID}_{Global.DURATION_ITEM}";
        GameObject skillDurationItem = skillBaseCast.GetSetSkillDurationWindow.transform.Find(skillObjectNameToCall).gameObject;
        SkillWindowDurationSetter skillWindowDurationSetter = skillDurationItem.GetComponent<SkillWindowDurationSetter>();

        skillWindowDurationSetter.GetSetSkillID = skillBaseCast.GetSetSkillID;
        skillWindowDurationSetter.GetSetDuration = duration;
        skillWindowDurationSetter.GetSetSkillSprite = skillBaseCast.GetSetSkillReference.GetSkillSprite(
            iconName: skillBaseCast.GetSetSkillReference.GetIconName(
                skillID: skillBaseCast.GetSetSkillID
            )
        );

        skillDurationItem.SetActive(false);
        skillDurationItem.SetActive(true);
    }

    public void ReturnToCombatMode() {
        skillBaseCast.GetSetIsCastingSkill = false;
        skillBaseCast.GetSetBasicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }
}
