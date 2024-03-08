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
    
    [Space(2)]

    [Header("AI Settings")]
    [SerializeField] private List<GameObject> patrolPoints = new List<GameObject>();


    private AnimancerComponent animancerComponent;
    private NavMeshAgent mobsAgent;
    private EnemyStatsManager enemyStatsManager;
    private GameObject enemyPlayer;
    private List<int> navigationIndices = new List<int>();
    private Vector3 currentDestination;
    private Vector3 directionToTarget;
    private Coroutine patrolCoroutine;
    private float enemyAIUpdateInterval = 1f;

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
    }

    public void CreatePatrollingPoints() {
        //TODO: The patrol points is only from a game object it must be random vector3 from a spawn area
        if (navigationIndices.Count == patrolPoints.Count) {
            navigationIndices.Clear();
        }

        int index;
        do {
            index = Random.Range(0, patrolPoints.Count);
        } while (navigationIndices.Contains(index));

        navigationIndices.Add(index);
        currentDestination = patrolPoints[index].transform.position;
        directionToTarget = (currentDestination - transform.parent.transform.position).normalized;
        mobsAgent.isStopped = false;

        if (directionToTarget.sqrMagnitude > 0.0001f) {
            Quaternion yOnlyRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
            transform.parent.transform.rotation = yOnlyRotation;
        }
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
                PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
                mobsAgent.isStopped = true;
                mobsAgent.ResetPath();

                yield return new WaitForSeconds(3f);
                CreatePatrollingPoints();
                InitializePatrol();
                break;
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