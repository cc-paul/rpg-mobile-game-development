using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class EnemyAIManager : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject enemyPrefabToSpawn;
    [SerializeField] private Transform spawnArea;

    [Space(2)]

    [Header("Variable Declaration")]
    [SerializeField] private int enemySpawnCount = 100;
    [SerializeField] private int enemySpawnCountOnEditor = 100;
    [SerializeField] private bool isAreaRange = false;
    [SerializeField] private bool isAreaAggressive = false;

    private int defaultPatrolCount = 3;
    private int spawnCount;
    private float randomX;
    private float randomZ;
    private float yPosition;
    private Bounds planeBounds;
    private Vector3 randomPosition;
    private Transform enemySpawnParent;
    private GameObject enemyToSpawn;
    private GameObject controller;
    private GameObject currentEnemy;

    private EnemyAI enemyAI;

    private CoroutineHandle spawnEnemies;
    private ObjectPoolManager enemyPoolManager;


    private void Awake() {
        planeBounds = spawnArea.GetComponent<Renderer>().bounds;
        enemyPoolManager = GetComponent<ObjectPoolManager>();
        enemySpawnParent = transform;
    }

    private void Start() {
        #if UNITY_EDITOR
            enemySpawnCount = enemySpawnCountOnEditor;
        #endif

        spawnCount = 0;
        Timing.RunCoroutine(StartControllingTheEnemy());
    }

    private void StartSpawningTheEnemies() {
        spawnEnemies = Timing.RunCoroutine(SpawnEnemies());
    }

    private IEnumerator<float> SpawnEnemies() {
        while (spawnCount < enemySpawnCount) {
            GetRandomPosition();
            CreateMobs();

            spawnCount++;

            yield return Timing.WaitForSeconds(0.2f);
        }
    }

    public IEnumerator<float> TestSpawnMobs() {
        GetRandomPosition();
        CreateMobs();

        yield return Timing.WaitForSeconds(0f);
    }

    private void CreateMobs() {
        enemyToSpawn = enemyPoolManager.SpawnFromPool(enemyPrefabToSpawn.name.ToString());
        enemyToSpawn.transform.position = randomPosition;
        enemyToSpawn.SetActive(true);

        controller = enemyToSpawn.transform.Find(Global.CONTROLLER).gameObject;
        enemyAI = controller.GetComponent<EnemyAI>();

        for (int patrolIndex = 0; patrolIndex < defaultPatrolCount; patrolIndex++) {
            GetRandomPosition();
            enemyAI.GetSetPatrolPositions.Add(randomPosition);
        }

        enemyAI.GetSetIsEnemyAgressive = isAreaRange;
        enemyAI.GetSetIsEnemyRange = isAreaAggressive;
        enemyAI.AddDefaulStats();
        enemyAI.SetNavigationDefaultStats();
        enemyAI.RecalibrateSettings();
        enemyAI.PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
    }

    private IEnumerator<float> StartControllingTheEnemy() {
        yield return Timing.WaitForSeconds(2f);

        while (true) {
            yield return Timing.WaitForSeconds(1f);

            for (int enemyChild_i = 0; enemyChild_i < enemySpawnParent.childCount; enemyChild_i++) {
                currentEnemy = enemySpawnParent.transform.GetChild(enemyChild_i).gameObject;

                if (currentEnemy.activeSelf) {
                    controller = currentEnemy.transform.Find(Global.CONTROLLER).gameObject;
                    enemyAI = controller.GetComponent<EnemyAI>();
                    enemyAI.DoAITask();
                }
            }
        }
    }

    private void GetRandomPosition() {
        randomX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        randomZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
        yPosition = spawnArea.position.y;
        randomPosition = new Vector3(randomX, yPosition, randomZ);
    }
}

/*public class EnemyAIManager : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnArea;

    [Space(2)]

    [Header("Variable Declaration")]
    [SerializeField] private int enemySpawnCount = 100;
    [SerializeField] private int enemySpawnCountOnEditor = 100;
    [SerializeField] private bool isEnemyInThisAreaAgressive = false;

    private ObjectPoolManager enemyPoolManager;
    private Bounds planeBounds;
    private Vector3 spawnPosition;
    private Transform enemySpawnParent;
    private float randomX;
    private float randomZ;
    private float yPosition;

    private void Awake() {
        enemySpawnParent = transform;
        enemyPoolManager = GetComponent<ObjectPoolManager>();
    }

    private void Start() {
#if UNITY_EDITOR
        enemySpawnCount = enemySpawnCountOnEditor;
#endif

        Invoke(nameof(StartSpawningEnemy), 1f);
    }

    private void StartSpawningEnemy() {
        GameObject enemyToSpawn;
        GameObject controller;
        EnemyAI enemyAI;

        if (enemyPoolManager != null) {
            for (int i = 0; i < enemySpawnCount; i++) {
                enemyToSpawn = enemyPoolManager.SpawnFromPool(enemyPrefab.name.ToString());
                enemyToSpawn.transform.position = GetRandomSpawnPosition();
                enemyToSpawn.SetActive(true);

                controller = enemyToSpawn.transform.Find(Global.CONTROLLER).gameObject;
                enemyAI = controller.GetComponent<EnemyAI>();

                enemyAI.GetSetIsEnemyAgressive = isEnemyInThisAreaAgressive;
                enemyAI.PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
                enemyAI.AddDefaulStats();
                enemyAI.SetNavigationDefaultStats();
            }
        }

        //TODO: Do not run if there is no enemy
        Timing.RunCoroutine(StartControllingTheEnemy());
    }

    private IEnumerator<float> StartControllingTheEnemy() {
        EnemyAI enemyAI;
        GameObject currentEnemy;
        GameObject controller;
        int timeToMove;

        while (true) {
            yield return Timing.WaitForSeconds(1f);

            for (int enemyChild_i = 0; enemyChild_i < enemySpawnParent.childCount; enemyChild_i++) {
                currentEnemy = enemySpawnParent.transform.GetChild(enemyChild_i).gameObject;

                if (currentEnemy.activeSelf) {
                    controller = currentEnemy.transform.Find(Global.CONTROLLER).gameObject;
                    enemyAI = controller.GetComponent<EnemyAI>();

                    if (enemyAI.GetSetEnemyStatsManager.Health.Value > 0) {
                        if (enemyAI.GetSetAttackerController == null) {
                            timeToMove = Random.Range(1, 3);

                            if (enemyAI.GetSetIsEnemyAgressive) {
                                enemyAI.CheckNearbyPlayer();
                            }

                            if (enemyAI.GetSetIsEnemyMoving) {
                                if (enemyAI.GetSetEnemyAgent.remainingDistance <= enemyAI.GetSetEnemyAgent.stoppingDistance) {
                                    enemyAI.GetSetCurrentDestination = GetRandomSpawnPosition();
                                    enemyAI.RepatrolTheEnemy(timeToMove: timeToMove);
                                }
                            } else {
                                enemyAI.RepatrolTheEnemy(timeToMove: timeToMove);
                            }
                        } else {
                            enemyAI.GetSetCurrentDestination = enemyAI.GetSetAttackerController.transform.position;
                            enemyAI.GoToTarget();

                            if (enemyAI.GetSetEnemyAgent.remainingDistance <= enemyAI.GetSetEnemyAgent.stoppingDistance && !enemyAI.GetSetIsEnemyAttacking) {
                                enemyAI.AttackTheAttacker();
                            } else if (enemyAI.GetSetEnemyAgent.remainingDistance > 20f) {
                                enemyAI.StopTheEnemy();
                                enemyAI.StopFollowingTheAttacker();
                            }
                        }
                    } else {
                        enemyAI.EnemyIsDie();
                    }
                }
            }
        }
    }

    private Vector3 GetRandomSpawnPosition() {
        planeBounds = spawnArea.GetComponent<Renderer>().bounds;
        randomX = Random.Range(planeBounds.min.x, planeBounds.max.x);
        randomZ = Random.Range(planeBounds.min.z, planeBounds.max.z);
        yPosition = spawnArea.position.y;

        spawnPosition = new Vector3(randomX, yPosition, randomZ);
        return spawnPosition;
    }
}*/