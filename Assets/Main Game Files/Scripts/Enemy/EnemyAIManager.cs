using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();

    [Space(2)]

    [Header("Variable Declaration")]
    [SerializeField] private int enemySpawnCount = 100;

    private ObjectPoolManager enemyPoolManager;

    private void Awake() {
        enemyPoolManager = GetComponent<ObjectPoolManager>();
    }

    private void Start() {
        Invoke(nameof(StartSpawningEnemy),1f);
    }

    private void StartSpawningEnemy() {
        GameObject enemyToSpawn;
        GameObject controller;
        EnemyAI enemyAI;

        if (enemyPoolManager != null) {
            for (int i = 0; i < enemySpawnCount; i++) {
                enemyToSpawn = enemyPoolManager.SpawnFromPool(enemyPrefab.name.ToString());
                enemyToSpawn.transform.position = spawnArea.position;
                enemyToSpawn.SetActive(true);

                controller = enemyToSpawn.transform.Find(Global.CONTROLLER).gameObject;
                enemyAI = controller.GetComponent<EnemyAI>();

                enemyAI.PlayEnemyAnimation(_currentAnimationName: Global.EnemyAnimation.Enemy_Idle.ToString());
            }
        }
    }
}

