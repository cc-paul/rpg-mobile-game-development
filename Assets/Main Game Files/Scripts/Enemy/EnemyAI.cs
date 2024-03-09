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
    [SerializeField] private float distanceToAttackMelee = 3f;
    [SerializeField] private float distanceToAttackRange = 6f;
    [SerializeField] private bool isEnemyRange;

    private BoxCollider enemyCollider;
    private EnemyUIController enemyUIController;
    private AnimancerComponent animancerComponent;
    private NavMeshAgent mobsAgent;
    private EnemyStatsManager mobsStatsManager;
    private List<int> navigationIndices = new List<int>();
    private Vector3 currentDestination;
    private Vector3 directionToTarget;
    private Coroutine patrolCoroutine;
    private float enemyAIUpdateInterval = 1f;
    private float additionalSpeed;
    private float attackRange;
    private float attackDuration;

    //For player who hit the mobs
    private GameObject enemyPlayerController;
    private GameObject enemyPlayerHitter;
    private GameObject enemyPlayerToAttack;

    private GameObject enemyParentObject;
    private PlayerStatsManager enemyPlayerHitterStatsManager;

    private StatModifier damageTaken;
    private StatModifier addedChaseSpeed;

    #region GetSet Properties
    /*    public GameObject GetSetEnemyPlayer {
            get { return enemyPlayer; }
            set { enemyPlayer = value; }
        }*/
    #endregion

    private void Awake() {
        OptionalWarning.NativeControllerHumanoid.Disable();

        enemyParentObject = transform.parent.gameObject;

        enemyUIController = GetComponent<EnemyUIController>();
        mobsAgent = enemyParentObject.GetComponent<NavMeshAgent>();
        animancerComponent = enemyModel.GetComponent<AnimancerComponent>();
        mobsStatsManager = GetComponent<EnemyStatsManager>();
        enemyCollider = enemyParentObject.GetComponent<BoxCollider>();
    }

    public void SetNavigationDefaultStats() {
        attackRange = isEnemyRange ? distanceToAttackRange : distanceToAttackMelee;
        mobsAgent.stoppingDistance = attackRange;
        mobsAgent.radius = attackRange;
        mobsAgent.speed = mobsStatsManager.Speed.Value;
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
        mobsAgent.isStopped = false;
        currentDestination = patrolPoints[index].transform.position;

        LookAtTheTarget();
    }

    private void LookAtTheTarget() {
        var lookDirection = new Vector3(currentDestination.x, enemyParentObject.transform.position.y, currentDestination.z);
        enemyParentObject.transform.LookAt(lookDirection);
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

            if (mobsStatsManager.Health.Value <= 0) {
                
                if (enemyPlayerToAttack != null) {
                    GameObject playerManager = enemyPlayerToAttack.transform.parent.gameObject;
                    GameObject generalSettings = playerManager.transform.Find(Global.GENERAL_SETTINGS).gameObject;
                    PlayerStatsManager playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();

                    if (playerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentObject)) {
                        playerStatsManager.GetSetCurrentMobsFollowingMe.Remove(enemyParentObject);
                    }

                    enemyPlayerToAttack = null;
                }

                PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Death.ToString());
                enemyCollider.enabled = false;
                mobsAgent.ResetPath();
                mobsAgent.isStopped = true;

                yield return new WaitForSeconds(3f);
                enemyParentObject.SetActive(false);
                break;
            }

            if (enemyPlayerToAttack != null) {
                GameObject playerManager = enemyPlayerToAttack.transform.parent.gameObject;
                GameObject generalSettings = playerManager.transform.Find(Global.GENERAL_SETTINGS).gameObject;
                PlayerStatsManager playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();

                if (playerStatsManager.Health.Value <= 0) {
                    PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
                    yield return new WaitForSeconds(1f);

                    enemyPlayerToAttack = null;
                    addedChaseSpeed = new StatModifier(-additionalSpeed, Global.StatModType.Flat, this);
                    mobsStatsManager.Speed.AddModifier(addedChaseSpeed);
                    mobsAgent.speed = mobsStatsManager.Speed.Value;

                    if (playerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentObject)) {
                        playerStatsManager.GetSetCurrentMobsFollowingMe.Remove(enemyParentObject);
                    }

                    mobsAgent.ResetPath();
                    CreatePatrollingPoints();
                    InitializePatrol();
                    break;
                }

                mobsAgent.speed = mobsStatsManager.Speed.Value;
                currentDestination = enemyPlayerToAttack.transform.position;
                mobsAgent.SetDestination(currentDestination);
                LookAtTheTarget();

                if (mobsAgent.remainingDistance <= attackRange) {
                    int attackNumber = Random.Range(1,4);
                    string attackName = $"Attack{attackNumber}_Animation";
                    PlayEnemyAnimation(_currentAnimationName: attackName);
                    mobsAgent.isStopped = true;
                    yield return new WaitForSeconds(attackDuration);
                    mobsAgent.isStopped = false;
                } else {
                    PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Chase.ToString());
                }
            } else {
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
    }

    public void TakeDamage(PlayerStatsController playerStatsController,float damage) {
        damageTaken = new StatModifier(-damage, Global.StatModType.Flat, this);
        mobsStatsManager.Health.AddModifier(damageTaken);

        enemyUIController.UpdateHealthUI(
            _currentHP: mobsStatsManager.Health.Value,
            _maxHP: mobsStatsManager.MaxHealth.Value
        );

        StartCoroutine(mobsStatsManager.RegenStatCoroutine(
            mobsStatsManager.Health,
            mobsStatsManager.MaxHealth,
            mobsStatsManager.HealthRegenValue
        ));

        if (enemyPlayerHitter != null) return;

        enemyPlayerController = playerStatsController.transform.parent.gameObject.transform.Find(Global.CONTROLLER).gameObject;
        enemyPlayerHitter = playerStatsController.gameObject;
        enemyPlayerHitterStatsManager = enemyPlayerHitter.GetComponent<PlayerStatsManager>();

        int currentEnemyFollowingCount = enemyPlayerHitterStatsManager.GetSetCurrentMobsFollowingMe.Count;
        int maxTargetToLure = enemyPlayerHitterStatsManager.GetSetMaxTargetToLure;

        if (currentEnemyFollowingCount < maxTargetToLure) {
            if (!enemyPlayerHitterStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentObject)) {
                enemyPlayerHitterStatsManager.GetSetCurrentMobsFollowingMe.Add(enemyParentObject);
                enemyPlayerToAttack = enemyPlayerController;

                additionalSpeed = 1;
                addedChaseSpeed = new StatModifier(additionalSpeed, Global.StatModType.Flat,this);
                mobsStatsManager.Speed.AddModifier(addedChaseSpeed);


                mobsAgent.ResetPath();
            }
        }
    }

    public void PlayEnemyAnimation(string _currentAnimationName) {
        ClipTransition currentClipTransition = null;

        var animationClipInfo = enemyAnimation.Find(
            clipInfo => clipInfo.animationName.ToString() == _currentAnimationName
        );

        if (animationClipInfo != null) {
            attackDuration = animationClipInfo.clipTransition.Length;
            currentClipTransition = animationClipInfo.clipTransition;
        }

        if (currentClipTransition != null) {
            animancerComponent.Play(currentClipTransition);
        } else {
            Debug.LogWarning("Animation not found: " + _currentAnimationName);
        }
    }
}