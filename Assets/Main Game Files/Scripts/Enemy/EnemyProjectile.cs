using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class EnemyProjectile : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject explosionPrefab;

    [Space(2)]

    [Header("Variable Declaration")]
    [SerializeField] private float launchVelocity;
    [SerializeField] private float offset;
    [SerializeField] private float distanceToHit;

    private CoroutineHandle shootCoroutine;
    private GameObject targetEnemy;
    private Vector3 targetEnemyPosition;
    private Transform projectile;

    #region GetSet Properties
    private CoroutineHandle GetSetShootCoroutineHandle {
        get { return shootCoroutine; }
        set { shootCoroutine = value; }
    }

    public GameObject GetSetTargetEnemy {
        get { return targetEnemy; }
        set { targetEnemy = value; }
    }

    public Vector3 GetSetTargetEnemyPosition {
        get { return targetEnemyPosition; }
        set { targetEnemyPosition = value; }
    }
    #endregion

    private void Awake() {
        projectile = transform;
    }

    private void OnEnable() {
        projectile.GetComponent<Rigidbody>().velocity = projectile.up * launchVelocity;
        shootCoroutine = Timing.RunCoroutine(GoToEnemy());
    }

    private void OnDisable() {
        Timing.KillCoroutines(shootCoroutine);
    }

    private IEnumerator<float> GoToEnemy() {
        while (true) {
            /*if (!targetEnemy.activeSelf) {
                ResetTheProjectile();
                Timing.KillCoroutines(shootCoroutine);
            }*/

            if (Vector3.Distance(transform.position, targetEnemyPosition) < distanceToHit) {
                ResetTheProjectile();
                Timing.KillCoroutines(shootCoroutine);
            }

            yield return Timing.WaitForSeconds(0f);
        }
    }

    private void ResetTheProjectile() {
        projectile.gameObject.SetActive(false);
        projectile.transform.position = Vector3.zero;
    }
}
