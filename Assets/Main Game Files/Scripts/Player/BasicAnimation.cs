using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillSettings;

    private WeaponWeilding weaponWeilding;
    private SkillBaseCast skillBaseCast;
    private AnimationPlayer animationPlayer;
    private PlayerStatsManager playerStatsManager;
    private Coroutine returnToNonCombat;
    private Global.AnimationCategory currentAnimationCategory;

    private void Awake() {
        animationPlayer = GetComponent<AnimationPlayer>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        skillBaseCast = skillSettings.GetComponent<SkillBaseCast>();
        weaponWeilding = GetComponent<WeaponWeilding>();
    }

    private void Start() {
        PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }

    public void PlayBasicAnimation(Global.AnimationCategory _animationCategory) {
        string animationToPlay = null;
        currentAnimationCategory = _animationCategory;

        if (_animationCategory == Global.AnimationCategory.Idle) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    animationToPlay = skillBaseCast.GetSetDidCastSkill ? Global.SwordsmanNormalAnimation.Idle_Sword.ToString() : Global.SwordsmanNormalAnimation.Sword_Base_Idle.ToString();
                }
            }
        }

        if (_animationCategory == Global.AnimationCategory.Walk) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    if (skillBaseCast.GetSetDidCastSkill) {
                        if (playerStatsManager.GetSetHasWeapon) {
                            animationToPlay = Global.SwordsmanNormalAnimation.Walk_Sword.ToString();
                        } else {
                            animationToPlay = Global.SwordsmanNormalAnimation.Walk_No_Sword.ToString();
                        }
                    } else {
                        animationToPlay = Global.SwordsmanNormalAnimation.Walk_No_Sword.ToString();
                    }
                }
            }
        }

        if (_animationCategory == Global.AnimationCategory.Run) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    if (skillBaseCast.GetSetDidCastSkill) {
                        if (playerStatsManager.GetSetHasWeapon) {
                            animationToPlay = Global.SwordsmanNormalAnimation.Run_Sword.ToString();
                        } else {
                            animationToPlay = Global.SwordsmanNormalAnimation.Run_No_Sword.ToString();
                        }
                    } else {
                        animationToPlay = Global.SwordsmanNormalAnimation.Run_No_Sword.ToString();
                    }
                }
            }
        }

        if (animationToPlay == null) return;

        animationPlayer.PlayAnimationByName(
            _currentAnimationName: animationToPlay,
            _isNormalAnimation: true
        );
    }

    public void InitializeRerturnToNonCombat() {
        if (returnToNonCombat != null) {
            StopCoroutine(returnToNonCombat);
        }

        returnToNonCombat = StartCoroutine(nameof(ReturnToNonCombat));
    }

    private IEnumerator ReturnToNonCombat() {
        skillBaseCast.GetSetDidCastSkill = true;
        yield return new WaitForSeconds(8f);

        skillBaseCast.GetSetDidCastSkill = false;
        weaponWeilding.ChangeWeaponVisibility(isAttackMode: false);
        PlayBasicAnimation(_animationCategory: currentAnimationCategory);
    }
}