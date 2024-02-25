using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject enemyModel;

    [Space(2)]

    [Header("Enemy Animations")]
    [SerializeField] private List<AnimationClipInfo<Global.EnemyAnimation>> enemyAnimation = new List<AnimationClipInfo<Global.EnemyAnimation>>();

    private AnimancerComponent animancerComponent;
    [Space(2)]
    [SerializeField] private List<GameObject> patrolPoints = new List<GameObject>();


    private NavMeshAgent mobsAgent;
    private EnemyStatsManager enemyStatsManager;
    private GameObject enemyPlayer;
    private Vector3 currentDestination;
    private Coroutine patrolCoroutine;
    private float enemyAIUpdateInterval = 1f;
    private int navigationIndex;

    #region GetSet Properties
    public GameObject GetSetEnemyPlayer {
        get { return enemyPlayer; }
        set { enemyPlayer = value; }
    }
    #endregion

    private void Awake() {
        OptionalWarning.NativeControllerHumanoid.Disable();

        mobsAgent = transform.parent.GetComponent<NavMeshAgent>();
        animancerComponent = enemyModel.GetComponent<AnimancerComponent>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();
    }

    public void SetNavigationDefaultStats() {
        mobsAgent.speed = enemyStatsManager.Speed.Value;
        mobsAgent.isStopped = false;
        navigationIndex = 0;
    }

    public void CreatePatrollingPoints() {
        //TODO: The patrol points is only from a game object it must be random vector3 from a spawn area

        if (navigationIndex == patrolPoints.Count - 1) {
            navigationIndex = 0;
        } else {
            navigationIndex++;
        }

        currentDestination = patrolPoints[navigationIndex].transform.position;
    }

    public void InitializePatrol() {
        if (patrolCoroutine != null) {
            StopCoroutine(patrolCoroutine);
        }

        patrolCoroutine = StartCoroutine(nameof(StartPatrolling));
    }

    private IEnumerator StartPatrolling() {
        yield return new WaitForSeconds(2f);

        mobsAgent.SetDestination(currentDestination);
        PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Walk.ToString());

        while (enabled) {
            yield return new WaitForSeconds(enemyAIUpdateInterval);

            if (enemyPlayer != null) {
                //TODO: Attack the player if it doesnt reach the max lure capacity
                break;
            }

            
            if (mobsAgent.remainingDistance <= mobsAgent.stoppingDistance) {
                CreatePatrollingPoints();
                mobsAgent.SetDestination(currentDestination);
            }
        }
    }

    public void TakeDamage(PlayerStatsController playerStatsController,float damage) {

    }

    public void PlayEnemyAnimation(string _currentAnimationName) {
        ClipTransition currentClipTransition = null;

        var animationClipInfo = enemyAnimation.Find(
            clipInfo => clipInfo.animationName.ToString() == _currentAnimationName
        );

        if (animationClipInfo != null) {
            currentClipTransition = animationClipInfo.clipTransition;
        }

        if (currentClipTransition != null) {
            animancerComponent.Play(currentClipTransition);
        } else {
            Debug.LogWarning("Animation not found: " + _currentAnimationName);
        }
    }
}