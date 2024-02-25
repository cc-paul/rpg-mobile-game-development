using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour {
    [Header("Swordsman Normal Animation")]
    [SerializeField] private List<AnimationClipInfo<Global.SwordsmanNormalAnimation>> swordsmanNormalAttackAnimation = new List<AnimationClipInfo<Global.SwordsmanNormalAnimation>>();

    [Space(2)] 
    
    [Header("Swordsman Skill Animations")]
    [SerializeField] private List<AnimationClipInfo<Global.SwordsmanSkillAnimation>> swordsmanSkillAttackAnimation = new List<AnimationClipInfo<Global.SwordsmanSkillAnimation>>();

    [Space(2)]

    [Header("Components")]
    [SerializeField] private AnimancerComponent animancerComponent;

    private PlayerStatsManager playerStatsManager;

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    public void PlayAnimationByName(string _currentAnimationName,bool _isNormalAnimation) {
        ClipTransition currentClipTransition = null;

        if (_isNormalAnimation) {
            var animationClipInfo = swordsmanNormalAttackAnimation.Find(
                clipInfo => clipInfo.animationName.ToString() == _currentAnimationName
            );

            if (animationClipInfo != null) {
                currentClipTransition = animationClipInfo.clipTransition;
            }
        } else {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                var animationClipInfo = swordsmanSkillAttackAnimation.Find(
                    clipInfo => clipInfo.animationName.ToString() == _currentAnimationName
                );

                if (animationClipInfo != null) {
                    currentClipTransition = animationClipInfo.clipTransition;
                }
            }
        }

        if (currentClipTransition != null) {
            animancerComponent.Play(currentClipTransition);
        } else {
            Debug.LogWarning("Animation not found: " + _currentAnimationName);
        }
    }
}