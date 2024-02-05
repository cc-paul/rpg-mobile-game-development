using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationClipInfo {
    public Global.SwordsmanNormalAnimation swordsmanNormalAnimationName;
    public AnimationClip animationClip;
    public ClipTransition clipTransition;

    public AnimationClipInfo(Global.SwordsmanNormalAnimation _swordsmanNormalAnimationName, AnimationClip _animationClip, ClipTransition _clipTransition) {
        swordsmanNormalAnimationName = _swordsmanNormalAnimationName;
        animationClip = _animationClip;
        clipTransition = _clipTransition;
    }
}