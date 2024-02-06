using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour {
    [Header("Swordsman Normal Animation")]
    [SerializeField] private List<AnimationClipInfo> swordsmanNormalAttackAnimation = new List<AnimationClipInfo>();

    [Space(2)]

    [Header("Components")]
    [SerializeField] private AnimancerComponent animancerComponent;

    private PlayerStatsManager playerStatsManager;
    private AnimationClip animationClip;
    private AnimationClipInfo animationClipInfo;

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    public void PlayAnimationByName(string _currentAnimationName,bool _isNormalAnimation) {
        animationClip = null;

        if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
            if (_isNormalAnimation) {
                animationClipInfo = swordsmanNormalAttackAnimation.Find(
                    clipInfo => clipInfo.swordsmanNormalAnimationName.ToString() == _currentAnimationName
                );

                if (animationClipInfo != null) {
                    animationClip = animationClipInfo.animationClip;
                }
            }
        }


        if (animationClip == null) return;
        animancerComponent.Play(animationClip);
    }
}