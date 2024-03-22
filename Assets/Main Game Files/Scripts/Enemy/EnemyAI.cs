using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;

public class EnemyAI : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject enemyModel;
    [SerializeField] private GameObject castingAreaForRange;
    [SerializeField] private GameObject weaponToShoot;
    [SerializeField] private GameObject hitPrefab;

    [Space(2)]

    [Header("Enemy Animations")]
    [SerializeField] private List<AnimationClipInfo<Global.EnemyAnimation>> enemyAnimation = new List<AnimationClipInfo<Global.EnemyAnimation>>();

    [Space(2)]

    [Header("AI Settings")]
    [SerializeField] private float distanceToAttackMelee = 3f;
    [SerializeField] private float distanceToAttackRange = 6f;
    [SerializeField] private float agressiveDistance;
    [SerializeField] private float addedSpeed;
    [SerializeField] private bool isEnemyRange;
    [SerializeField] private bool isEnemyAgressive;
    [SerializeField] private int attackEndRange;

    [Space(2)]

    [Header("Layer Mask")]
    [SerializeField] private List<LayerMask> layersToAttack = new List<LayerMask>();

    private float rowClipTransitionLength;
    private float attackRange;
    private float attackDuration;
    private float enemyDamage;
    private bool isEnemyMoving;
    private bool isEnemyAttacking;
    private bool isGizmosDrawn;
    private bool isPlayerInsideSphere;
    private int attackNumber;
    private string attackName;
    private string currentAnimationName = null;
    private GameObject enemyParentContainer;
    private Vector3 currentDestination;
    private Vector3 lookDirection;
    private Global.EnemyAnimation rowAnimationName;
    private ClipTransition currentClipTransition = null;
    private ClipTransition rowClipTransition = null;
    private Collider[] colliders;

    private EnemyStatsManager enemyStatsManager;
    private EnemyUIController enemyUIController;
    private NavMeshAgent enemyAgent;
    private AnimancerComponent animancerComponent;
    private BoxCollider enemyBoxCollider;
    private EnemyProjectile enemyProjectile;

    /* Attacker Data */
    private GameObject attackerGeneralSettings;
    private GameObject attackerController;
    private GameObject attackerController_Temp;
    private PlayerStatsController attackerStatsController;
    private PlayerStatsController attackerStatsController_Temp;
    private PlayerStatsManager attackerStatsManager;
    private PlayerStatsManager attackerStatsManager_Temp;

    private StatModifier damageTaken;
    private StatModifier addedChaseSpeed;

    #region GetSet Properties 
    public GameObject GetSetAttackerController {
        get { return attackerController; }
        set { attackerController = value; }
    }

    public Vector3 GetSetCurrentDestination {
        get { return currentDestination; }
        set { currentDestination = value; }
    }

    public bool GetSetIsEnemyMoving {
        get { return isEnemyMoving; }
        set { isEnemyMoving = value; }
    }

    public NavMeshAgent GetSetEnemyAgent {
        get { return enemyAgent; }
        set { enemyAgent = value; }
    }

    public GameObject GetSetEnemyParentContainer {
        get { return enemyParentContainer; }
        set { enemyParentContainer = value; }
    }

    public bool GetSetIsEnemyAttacking {
        get { return isEnemyAttacking; }
        set { isEnemyAttacking = value; }
    }

    public bool GetSetIsEnemyAgressive {
        get { return isEnemyAgressive; }
        set { isEnemyAgressive = value; }
    }

    public EnemyStatsManager GetSetEnemyStatsManager {
        get { return enemyStatsManager; }
        set { enemyStatsManager = value; }
    }
    #endregion

    private void Awake() {
        OptionalWarning.NativeControllerHumanoid.Disable();
        enemyParentContainer = transform.parent.gameObject;

        enemyUIController = GetComponent<EnemyUIController>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();
        enemyAgent = enemyParentContainer.GetComponent<NavMeshAgent>();
        enemyBoxCollider = enemyParentContainer.GetComponent<BoxCollider>();
        animancerComponent = enemyModel.GetComponent<AnimancerComponent>();

        if (weaponToShoot != null) {
            enemyProjectile = weaponToShoot.GetComponent<EnemyProjectile>();
        }

        attackEndRange = attackEndRange + 1;
    }

    public void AddDefaulStats() {
        enemyBoxCollider.enabled = true;
        enemyAgent.ResetPath();
        isEnemyMoving = true;
        isEnemyAttacking = false;
        enemyStatsManager.AddDefaultStats();
        enemyUIController.UpdateHealthUI(
            _currentHP: enemyStatsManager.Health.Value,
            _maxHP: enemyStatsManager.MaxHealth.Value
        );
    }

    private void AddBoostingStats() {
        addedChaseSpeed = new StatModifier(addedSpeed,Global.StatModType.Flat,this);
        enemyStatsManager.Speed.AddModifier(addedChaseSpeed);
        SetNavigationDefaultStats();
    }

    public void SetNavigationDefaultStats() {
        attackRange = isEnemyRange ? distanceToAttackRange : distanceToAttackMelee;
        enemyAgent.stoppingDistance = attackRange;
        enemyAgent.radius = attackRange;
        enemyAgent.speed = enemyStatsManager.Speed.Value;
        enemyAgent.isStopped = false;
    }

    public void LookAtTarget() {
        lookDirection = new Vector3(currentDestination.x, enemyParentContainer.transform.position.y, currentDestination.z);
        enemyParentContainer.transform.LookAt(lookDirection);
    }

    public void RepatrolTheEnemy(int timeToMove) {
        StopTheEnemy();
        Invoke(nameof(GoToTarget), timeToMove);
    }

    public void GoToTarget() {
        if (enemyStatsManager.Health.Value <= 0) return;
        if (isEnemyAttacking) return;

        enemyAgent.SetDestination(currentDestination);
        enemyAgent.isStopped = false;
        isEnemyMoving = true;
        isEnemyAttacking = false;
        PlayEnemyAnimation(_currentAnimationName: attackerController == null ? Global.EnemyAnimation.Enemy_Walk.ToString() : Global.EnemyAnimation.Enemy_Chase.ToString());
        LookAtTarget();
    }

    public void StopTheEnemy() {
        isEnemyMoving = false;
        enemyAgent.isStopped = true;
        enemyAgent.ResetPath();
        PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
    }

    public void StopFollowingTheAttacker() {
        if (attackerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentContainer)) {
            attackerStatsManager.GetSetCurrentMobsFollowingMe.Remove(enemyParentContainer);
        }

        attackerController = null;
        attackerStatsController = null;
        attackerStatsManager = null;
        isEnemyAttacking = false;
        isEnemyMoving = true;
        enemyStatsManager.Speed.RemoveModifier(addedChaseSpeed);
        SetNavigationDefaultStats();
    }

    public void EnemyIsDie() {
        enemyAgent.isStopped = true;
        isEnemyMoving = false;
        isEnemyAttacking = false;
        enemyBoxCollider.enabled = false;
        enemyStatsManager.GetSetIsRegenPaused = true;
        enemyStatsManager.Speed.RemoveModifier(addedChaseSpeed);

        if (attackerController != null) {
            if (attackerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentContainer)) {
                attackerStatsManager.GetSetCurrentMobsFollowingMe.Remove(enemyParentContainer);
            }
        }

        Timing.PauseCoroutines(enemyStatsManager.GetSetHPCoroutine);
        PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Death.ToString());

        Invoke(nameof(DisableEnemy),4f);
    }

    public void DisableEnemy() {
        enemyParentContainer.gameObject.SetActive(false);
    }

    public void EnemyTakeDamage(
        PlayerStatsManager playerStatsManager,
        PlayerStatsController playerStatsController,
        float damage
    ) {
        damageTaken = new StatModifier(-damage, Global.StatModType.Flat, this);
        enemyStatsManager.Health.AddModifier(damageTaken);

        enemyUIController.UpdateHealthUI(
            _currentHP: enemyStatsManager.Health.Value,
            _maxHP: enemyStatsManager.MaxHealth.Value
        );

        if (enemyStatsManager.GetSetIsRegenPaused) {
            enemyStatsManager.GetSetIsRegenPaused = false;

            enemyStatsManager.GetSetHPCoroutine = Timing.RunCoroutine(enemyStatsManager.RegenStatCoroutine(
                enemyStatsManager.Health,
                enemyStatsManager.MaxHealth,
                enemyStatsManager.HealthRegenValue
            ));
        }

        if (attackerController == null && playerStatsManager.GetSetCurrentMobsFollowingMe.Count < playerStatsManager.GetSetMaxTargetToLure) {
            if (enemyStatsManager.Health.Value <= 0) {
                EnemyIsDie();
            } else {
                attackerController = playerStatsManager.transform.parent.transform.Find(Global.CONTROLLER).gameObject;
                attackerStatsController = playerStatsController;
                attackerStatsManager = playerStatsManager;

                if (!attackerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentContainer)) {
                    attackerStatsManager.GetSetCurrentMobsFollowingMe.Add(enemyParentContainer);
                }

                StopTheEnemy();
                AddBoostingStats();
            }
        }
    }

    public void AttackTheAttacker() {
        if (enemyStatsManager.Health.Value <= 0) return;
        if (isEnemyAttacking) return;

        isEnemyAttacking = true;
        isEnemyMoving = false;
        enemyAgent.isStopped = true;
        attackNumber = Random.Range(1, attackEndRange);
        attackName = $"Attack{attackNumber}_Animation";

        enemyAgent.ResetPath();
        PlayEnemyAnimation(_currentAnimationName: attackName);
    }

    public void ShootTheEnemy() {
        weaponToShoot.transform.position = castingAreaForRange.transform.position;
        enemyProjectile.GetSetTargetEnemy = attackerController;
        enemyProjectile.GetSetTargetEnemyPosition = attackerController.transform.position;
        weaponToShoot.SetActive(true);
    }

    public void ShowHitExplosion() {
        hitPrefab.transform.position = attackerController.transform.Find(Global.HITTING_AREA).transform.position;
        hitPrefab.SetActive(true);

        Invoke(nameof(HideExplosion),0.5f);
    }

    private void HideExplosion() {
        hitPrefab.SetActive(false);
        hitPrefab.transform.position = Vector3.zero;
    }

    public void DealDamage() {
        enemyDamage = enemyStatsManager.BaseDamage.Value;

        attackerStatsController.ReceiveDamage(_damageAmount: enemyDamage,_sourceComponent: this);
    }

    public void CheckNearbyPlayer() {
        for (int layer_i = 0; layer_i < layersToAttack.Count; layer_i++) {
            isPlayerInsideSphere = Physics.CheckSphere(transform.position, agressiveDistance, layersToAttack[layer_i]);

            if (isPlayerInsideSphere) {
                colliders = Physics.OverlapSphere(transform.position, agressiveDistance, layersToAttack[layer_i]);
                if (colliders.Length > 0) {
                    attackerController_Temp = colliders[0].transform.parent.transform.Find(Global.CONTROLLER).gameObject;
                    attackerGeneralSettings = colliders[0].transform.parent.transform.Find(Global.GENERAL_SETTINGS).gameObject;
                    attackerStatsController_Temp = attackerGeneralSettings.GetComponent<PlayerStatsController>();
                    attackerStatsManager_Temp = attackerGeneralSettings.GetComponent<PlayerStatsManager>();

                    if (attackerController == null && attackerStatsManager_Temp.GetSetCurrentMobsFollowingMe.Count < attackerStatsManager_Temp.GetSetMaxTargetToLure) {
                        attackerController = attackerController_Temp;
                        attackerStatsController = attackerStatsController_Temp;
                        attackerStatsManager = attackerStatsManager_Temp;

                        if (!attackerStatsManager.GetSetCurrentMobsFollowingMe.Contains(enemyParentContainer)) {
                            attackerStatsManager.GetSetCurrentMobsFollowingMe.Add(enemyParentContainer);
                        }

                        StopTheEnemy();
                        AddBoostingStats();
                    }
                }
            }
            break;
        }
    }

    /*void OnDrawGizmosSelected() {
        // Draw the sphere cast for visualization in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agressiveDistance);
    }*/

    #region Animation Player
    public void PlayEnemyAnimation(string _currentAnimationName) {
        if (currentAnimationName == _currentAnimationName) return;

        for (int animation_i = 0; animation_i < enemyAnimation.Count; animation_i++) {
            rowAnimationName = enemyAnimation[animation_i].animationName;

            if (rowAnimationName.ToString() == _currentAnimationName) {
                rowClipTransition = enemyAnimation[animation_i].clipTransition;
                rowClipTransitionLength = enemyAnimation[animation_i].clipLength;
                attackDuration = rowClipTransitionLength;

                if (currentClipTransition != rowClipTransition) {
                    currentClipTransition = rowClipTransition;
                    animancerComponent.Play(rowClipTransition);
                    currentAnimationName = rowAnimationName.ToString();
                }
                break;
            }
        }
    }
    #endregion
}

/*public class EnemyAI : MonoBehaviour {
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
    [SerializeField] private float attackDuration;
    [SerializeField] private bool isEnemyRange;

    private List<int> navigationIndices = new List<int>();

    private Coroutine enemySelfCommandCoroutine;
    private Coroutine enemyRegenCoroutine;
    private GameObject enemyParentContainer;
    private Vector3 currentDestination;
    private Vector3 lookDirection;
    private float attackRange;
    private float enemyAIUpdateInterval;

    private EnemyStatsManager enemyStatsManager;
    private EnemyUIController enemyUIController;
    private NavMeshAgent enemyAgent;
    private AnimancerComponent animancerComponent;

    private StatModifier damageTaken;

    #region GetSet Properties
    public Coroutine GetSetEnemyRegenCoroutine {
        get { return enemyRegenCoroutine; }
        set { enemyRegenCoroutine = value; }    
    }
    #endregion

    private void Awake() {
        OptionalWarning.NativeControllerHumanoid.Disable();
        enemyParentContainer = transform.parent.gameObject;

        enemyUIController = GetComponent<EnemyUIController>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();
        enemyAgent = enemyParentContainer.GetComponent<NavMeshAgent>();
        animancerComponent = enemyModel.GetComponent<AnimancerComponent>();
    }

    private void OnEnable() {
        enemyStatsManager.AddDefaultStats();
        enemyUIController.UpdateHealthUI(
            _currentHP: enemyStatsManager.Health.Value,
            _maxHP: enemyStatsManager.MaxHealth.Value
        );
        SetNavigationDefaultStats();
        CreatePatrolPoints();
        
        if (enemySelfCommandCoroutine != null) {
            StopCoroutine(enemySelfCommandCoroutine);
        }

        enemySelfCommandCoroutine = StartCoroutine(nameof(InitializeEnemyAICommand));
    }

    public void SetNavigationDefaultStats() {
        attackRange = isEnemyRange ? distanceToAttackRange : distanceToAttackMelee;
        enemyAgent.stoppingDistance = attackRange;
        enemyAgent.radius = attackRange;
        enemyAgent.speed = enemyStatsManager.Speed.Value;
        enemyAgent.isStopped = false;
    }

    private void StopTheEnemyAgent() {
        enemyAgent.isStopped = true;
        enemyAgent.ResetPath();
    }

    private void CreatePatrolPoints() {
        if (navigationIndices.Count == patrolPoints.Count) {
            navigationIndices.Clear();
        }

        int index;
        do {
            index = Random.Range(0, patrolPoints.Count);
        } while (navigationIndices.Contains(index));

        navigationIndices.Add(index);
        enemyAgent.isStopped = false;
        currentDestination = patrolPoints[index].transform.position;
    }

    private void GoToTarget() {
        //TODO: Change animation if the enemy is chasing a player
        enemyAgent.SetDestination(currentDestination);
        enemyAgent.isStopped = false;
        PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Walk.ToString());
        LookAtTarget();
    }

    private void LookAtTarget() {
        lookDirection = new Vector3(currentDestination.x, enemyParentContainer.transform.position.y, currentDestination.z);
        enemyParentContainer.transform.LookAt(lookDirection);
    }

    public void EnemyTakeDamage(
        PlayerStatsManager playerStatsManager,
        PlayerStatsController playerStatsController,
        float damage
    ) {
        damageTaken = new StatModifier(-damage, Global.StatModType.Flat, this);
        enemyStatsManager.Health.AddModifier(damageTaken);

        enemyUIController.UpdateHealthUI(
            _currentHP: enemyStatsManager.Health.Value,
            _maxHP: enemyStatsManager.MaxHealth.Value
        );

        if (enemyRegenCoroutine != null) {
            StopCoroutine(enemyRegenCoroutine);
        }

        enemyRegenCoroutine = StartCoroutine(enemyStatsManager.RegenStatCoroutine(
            enemyStatsManager.Health,
            enemyStatsManager.MaxHealth,
            enemyStatsManager.HealthRegenValue
        ));
    }

    private IEnumerator InitializeEnemyAICommand() {
        yield return new WaitForSeconds(2f);

        GoToTarget();

        while (true) {
            yield return new WaitForSeconds(enemyAIUpdateInterval);

            if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance) {
                PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
                StopTheEnemyAgent();

                yield return new WaitForSeconds(3f);
                CreatePatrolPoints();
                GoToTarget();
            }
        }
    }

    #region Animation Player
    private void PlayEnemyAnimation(string _currentAnimationName) {
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
    #endregion
}*/


/*[Header("Game Object and Others")]
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
*//*    public GameObject GetSetEnemyPlayer {
        get { return enemyPlayer; }
        set { enemyPlayer = value; }
    }*//*
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
}*/