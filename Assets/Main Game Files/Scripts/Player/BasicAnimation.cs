using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : MonoBehaviour {
    private AnimationPlayer animationPlayer;
    private PlayerStatsManager playerStatsManager;

    private void Awake() {
        animationPlayer = GetComponent<AnimationPlayer>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    private void Start() {
        PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
    }

    public void PlayBasicAnimation(Global.AnimationCategory _animationCategory) {
        string animationToPlay = null;

        if (_animationCategory == Global.AnimationCategory.Idle) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    animationToPlay = Global.SwordsmanNormalAnimation.Sword_Base_Idle.ToString();
                }
            }
        }

        if (_animationCategory == Global.AnimationCategory.Walk) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    if (playerStatsManager.GetSetHasWeapon) {
                        animationToPlay = Global.SwordsmanNormalAnimation.Walk_Sword.ToString();
                    } else {
                        animationToPlay = Global.SwordsmanNormalAnimation.Walk_No_Sword.ToString();
                    }
                }
            }
        }

        if (_animationCategory == Global.AnimationCategory.Run) {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (playerStatsManager.GetSetGender == Global.Gender.Male) {
                    if (playerStatsManager.GetSetHasWeapon) {
                        animationToPlay = Global.SwordsmanNormalAnimation.Run_Sword.ToString();
                    } else {
                        animationToPlay = Global.SwordsmanNormalAnimation.Run_No_Sword.ToString();
                    }
                }
            }
        }

        //Debug.Log(animationToPlay);
        if (animationToPlay == null) return;

        animationPlayer.PlayAnimationByName(
            _currentAnimationName: animationToPlay,
            _isNormalAnimation: true
        );
    }
}