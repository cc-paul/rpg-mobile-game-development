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
    [SerializeField] private Dictionary<Global.SwordsmanSkillAnimation, AnimationClipInfo<Global.SwordsmanSkillAnimation>> swordsmanSkillAttackAnimation2 = new Dictionary<Global.SwordsmanSkillAnimation, AnimationClipInfo<Global.SwordsmanSkillAnimation>>();

    [Space(2)]

    [Header("Components")]
    [SerializeField] private AnimancerComponent animancerComponent;

    [Space(2)]

    [Header("Animation Duration")]
    [SerializeField] private GameObject castingProgress;
    [SerializeField] private Image progressBarFill;

    private WaitForSeconds skillCastNullWait = new WaitForSeconds(0f);
    private PlayerStatsManager playerStatsManager;
    private Coroutine castingCoroutine;
    private ClipTransition currentClipTransition = null;
    private float expectedCooldown = 0;
    private float timer;

    private void Awake() {
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    public void PlayAnimationByName(string _currentAnimationName,bool _isNormalAnimation) {
        float rowClipTransitionLength;
        string rowAnimationName;
        ClipTransition rowClipTransition;

        if (_isNormalAnimation) {
            for (int animation_i = 0; animation_i < swordsmanNormalAttackAnimation.Count; animation_i++) {
                rowAnimationName = swordsmanNormalAttackAnimation[animation_i].animationName.ToString();

                if (rowAnimationName == _currentAnimationName) {
                    rowClipTransition = swordsmanNormalAttackAnimation[animation_i].clipTransition;

                    if (currentClipTransition != rowClipTransition || _currentAnimationName.Contains(Global.SWORD_NORMAN_ATTACK)) {
                        animancerComponent.Play(rowClipTransition);
                    }
                    break;
                }
            }
        } else {
            if (playerStatsManager.GetSetCharacterType == Global.Characters.Swordsman) {
                for (int animation_i = 0; animation_i < swordsmanSkillAttackAnimation.Count; animation_i++) {
                    rowAnimationName = swordsmanSkillAttackAnimation[animation_i].animationName.ToString();

                    if (rowAnimationName == _currentAnimationName) {
                        rowClipTransition = swordsmanSkillAttackAnimation[animation_i].clipTransition;
                        expectedCooldown = rowClipTransition.Length / playerStatsManager.AttackSpeed.Value;
                        InitializeCastingProgress();
                        animancerComponent.Play(rowClipTransition);
                        break;
                    }
                }
            }
        }
    }

    private void InitializeCastingProgress() {
        //TODO: Hide Casting Progress when:
        //1. Moving
        //2. Dead
        timer = 0;
        castingProgress.SetActive(true);


        if (castingCoroutine == null) {
            progressBarFill.fillAmount = 1;
            castingCoroutine = StartCoroutine(nameof(StartCasting));
        }
    }

    private IEnumerator StartCasting() {
        while (timer < expectedCooldown) {
            progressBarFill.fillAmount = 1 - (timer / expectedCooldown);
            timer += Time.deltaTime;
            yield return skillCastNullWait;
        }

        castingCoroutine = null;
        HideCastProgress();
    }

    public void HideCastProgress() {
        if (castingProgress.activeSelf) {
            castingCoroutine = null;
            castingProgress.gameObject.SetActive(false);
        }
    }
}