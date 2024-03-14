using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyAI : MonoBehaviour {
    [Header("Variable Declarations and other Assignment")]
    [SerializeField] private bool isAIWillMove;

    [Space(2)]

    [Header("AI Settings")]
    [SerializeField] private List<GameObject> patrolPoints = new List<GameObject>();

    private PlayerStatsManager playerStatsManager;
    private AnimationPlayer animationPlayer;
    private GameObject controller;
    private NavMeshAgent allyAgent;

    private string animationName;
    private float allyUpdateInterval = 1f;
    private List<int> navigationIndices = new List<int>();
    private Vector3 currentDestination;
    private Coroutine patrolCoroutine;
    private Vector3 directionToTarget;


    private void Awake() {
        controller = transform.parent.Find(Global.CONTROLLER).gameObject;
        animationPlayer = GetComponent<AnimationPlayer>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        allyAgent = controller.GetComponent<NavMeshAgent>();
    }

    private void Start() {
        if (isAIWillMove) {
            bool isPlayerWillWalk = Random.Range(0, 2) == 0;

            if (isPlayerWillWalk) {
                animationName = Global.SwordsmanNormalAnimation.Walk_No_Sword.ToString();
            } else {
                StatModifier basicAddedSpeed = new StatModifier(4, Global.StatModType.Flat, this);

                animationName = Global.SwordsmanNormalAnimation.Run_No_Sword.ToString();
                playerStatsManager.Speed.AddModifier(basicAddedSpeed);
            }

            SetupNavmeshSettings();
            CreatePatrollingPoints();
            InitializePatrol();
        } else {
            PlayAIAnimation(_currentAnimationName: Global.SwordsmanNormalAnimation.Idle_No_Sword.ToString());
        }
    }

    private void SetupNavmeshSettings() {
        allyAgent.speed = playerStatsManager.Speed.Value;
    }

    private void CreatePatrollingPoints() {
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
        directionToTarget = (currentDestination - controller.transform.position).normalized;
        allyAgent.isStopped = false;

        var lookDirection = new Vector3(currentDestination.x, controller.transform.position.y, currentDestination.z);
        controller.transform.LookAt(lookDirection);
    }

    private void InitializePatrol() {
        if (patrolCoroutine != null) {
            StopCoroutine(patrolCoroutine);
        }

        patrolCoroutine = StartCoroutine(nameof(StartPatrolling));
    }

    private IEnumerator StartPatrolling() {
        yield return new WaitForSeconds(2F);

        allyAgent.SetDestination(currentDestination);
        PlayAIAnimation(_currentAnimationName: animationName);

        while (enabled) {
            yield return new WaitForSeconds(allyUpdateInterval);

            if (allyAgent.remainingDistance < allyAgent.stoppingDistance) {
                PlayAIAnimation(_currentAnimationName: Global.SwordsmanNormalAnimation.Idle_No_Sword.ToString());
                allyAgent.isStopped = true;
                allyAgent.ResetPath();

                yield return new WaitForSeconds(3f);
                CreatePatrollingPoints();
            } else {
                allyAgent.SetDestination(currentDestination);
            }
        }
    }

    private void PlayAIAnimation(string _currentAnimationName) {
        animationPlayer.PlayAnimationByName(
            _currentAnimationName: _currentAnimationName,
            _isNormalAnimation: true
        );
    }
}
