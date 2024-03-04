using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationPlayer : MonoBehaviour {
    [Header("Swordsman Normal Animation")]
    [SerializeField] private List<AnimationClipInfo<Global.SwordsmanNormalAnimation>> swordsmanNormalAttackAnimation = new List<AnimationClipInfo<Global.SwordsmanNormalAnimation>>();

    [Space(2)] 
    
    [Header("Swordsman Skill Animations")]
    [SerializeField] private List<AnimationClipInfo<Global.SwordsmanSkillAnimation>> swordsmanSkillAttackAnimation = new List<AnimationClipInfo<Global.SwordsmanSkillAnimation>>();

    [Space(2)]

    [Header("Components")]
    [SerializeField] private AnimancerComponent animancerComponent;

    [Space(2)]

    [Header("Animation Duration")]
    [SerializeField] private GameObject castingProgress;
    [SerializeField] private Image progressBarFill;

    private PlayerStatsManager playerStatsManager;
    private Coroutine castingCoroutine;
    private float expectedCooldown = 0;
    private float timer;

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
                    expectedCooldown = currentClipTransition.Length;
                    InitializeCastingProgress();
                }
            }
        }

        if (currentClipTransition != null) {
            animancerComponent.Play(currentClipTransition);
        } else {
            Debug.LogWarning("Animation not found: " + _currentAnimationName);
        }
    }

    private void InitializeCastingProgress() {
        //TODO: Hide Casting Progress when:
        //1. Moving
        //2. Dead

        if (castingCoroutine != null) {
            progressBarFill.fillAmount = 0;
            StopCoroutine(castingCoroutine);
        }

        timer = 0;
        castingProgress.SetActive(true);
        castingCoroutine = StartCoroutine(nameof(StartCasting));
    }

    private IEnumerator StartCasting() {
        while (timer < expectedCooldown) {
            progressBarFill.fillAmount = 1 - (timer / expectedCooldown);
            timer += Time.deltaTime;
            yield return null;
        }

        castingProgress.gameObject.SetActive(false);
    }

    public void HideCastProgress() {
        castingProgress.gameObject.SetActive(false);
    }
}