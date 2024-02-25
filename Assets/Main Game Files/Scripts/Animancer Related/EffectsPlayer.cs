using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPlayer : MonoBehaviour {
    [Space(2)]

    [Header("Clip Animation")]
    [SerializeField] private ClipTransition clipTransition;

    #region GetSet Properties
    public ClipTransition GetSetClipTransition {
        get { return clipTransition; }
        set { clipTransition = value; }
    }
    #endregion

    private AnimancerComponent animancerComponent;

    public void Awake() {
        animancerComponent = GetComponent<AnimancerComponent>();
    }

    public void PlaySkillEffect() {
        animancerComponent.Play(clipTransition);
    }
}

