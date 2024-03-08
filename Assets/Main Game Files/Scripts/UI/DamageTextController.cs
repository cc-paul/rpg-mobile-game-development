using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextController : MonoBehaviour {
    [Header("Game Object and others")]
    [SerializeField] private GameObject damageTextHolderPrefab;

    [Space(2)]

    [Header("Variable Declarations and other Assignments")]
    [SerializeField] private int poolSize = 50;

    public Queue<GameObject> damageTextPool = new Queue<GameObject>();
    private TagLookAtCamera tagLookAtCamera;

    private void Awake() {
        tagLookAtCamera = GetComponent<TagLookAtCamera>();
    }

    private void Start () {
        SetupDamageTextPool();
    }

    private void SetupDamageTextPool() {
        for (int i = 0; i < poolSize; i++) {
            GameObject damageTextHolder = Instantiate(damageTextHolderPrefab, damageTextHolderPrefab.transform.position, Quaternion.identity, transform);
            damageTextPool.Enqueue(damageTextHolder);

            damageTextHolder.SetActive(false);
        }
    }
}
