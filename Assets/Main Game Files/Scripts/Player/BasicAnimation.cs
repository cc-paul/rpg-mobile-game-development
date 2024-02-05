using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimation : MonoBehaviour {
    private AnimationPlayer animationPlayer;
    private StatsManager statsManager;

    private void Awake() {
        animationPlayer = GetComponent<AnimationPlayer>();
        statsManager = GetComponent<StatsManager>();
    }

    private void Start() {
        PlayBasicAnimation(Global.AnimationCategory.Idle);
    }

    public void PlayBasicAnimation(Global.AnimationCategory animationCategory) {
        string animationToPlay = null;

        if (animationCategory == Global.AnimationCategory.Idle) {
            if (statsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (statsManager.GetSetGender == Global.Gender.Male) {
                    animationToPlay = Global.SwordsmanNormalAnimation.Sword_Base_Idle.ToString();
                }
            }
        }

        if (animationCategory == Global.AnimationCategory.Walk) {
            if (statsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                if (statsManager.GetSetGender == Global.Gender.Male) {
                    if (statsManager.GetSetHasWeapon) {
                        animationToPlay = Global.SwordsmanNormalAnimation.Walk_Sword.ToString();
                    } else {
                        animationToPlay = Global.SwordsmanNormalAnimation.Walk_No_Sword.ToString();
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