using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLookAtCamera : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private List<NameTagInfo> nameTagInfoList = new List<NameTagInfo> ();

    private Camera camera;
    private Vector3 vectorForward = Vector3.forward;
    private Vector3 vectorUp = Vector3.up;
    private Vector3 vectorOne = Vector3.one;
    private float scaleFactor;
    private float scaleFactorHP;
    private float finalScale;
    private float distance;

    private void Awake() {
        camera = Camera.main;
    }

    private void LateUpdate() {
        foreach (NameTagInfo nameTagInfo in nameTagInfoList) {
            nameTagInfo.uiTag.LookAt(
                nameTagInfo.uiTag.position + camera.transform.rotation * vectorForward,
                camera.transform.rotation * vectorUp
            );

            distance = Vector3.Distance(nameTagInfo.uiTag.position,camera.transform.position);
            scaleFactor = distance / 10f;
            scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1.5f);
            scaleFactorHP = distance / 15f;
            scaleFactorHP = Mathf.Clamp(scaleFactorHP, 0.091f, 0.5f);
            finalScale = nameTagInfo.isHP ? scaleFactorHP : scaleFactor;

            nameTagInfo.uiTag.localScale = vectorOne * finalScale;
        }
    }
}
