using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPositioning : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject bottomPosition;

    [Space(10)]

    [Header("Variable Declarations and Other Assignments")]
    [SerializeField] private bool rotateAlso;

    private Transform myTransform;

    private void Awake() {
        myTransform = transform;
    }

    public void RepositionTargetIndicator() {
        myTransform.position = new Vector3(model.transform.position.x, bottomPosition.transform.position.y, model.transform.position.z);

        if (rotateAlso) {
            myTransform.rotation = model.transform.rotation;
        }
    }
}
