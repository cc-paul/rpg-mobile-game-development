using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationClipInfo<TEnum> where TEnum : System.Enum {
    public string name;
    public TEnum animationName;
    public ClipTransition clipTransition;
    [HideInInspector] public float clipLength;

    public AnimationClipInfo(TEnum _animationName, ClipTransition _clipTransition) {
        animationName = _animationName;
        clipTransition = _clipTransition;
        clipLength = _clipTransition.Length;
    }
}