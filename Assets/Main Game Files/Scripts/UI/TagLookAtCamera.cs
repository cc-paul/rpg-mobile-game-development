using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLookAtCamera : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private List<NameTagInfo> nameTagInfoList = new List<NameTagInfo>();

    private Camera camera;
    private Vector3 vectorForward = Vector3.forward;
    private Vector3 vectorUp = Vector3.up;
    private Vector3 vectorOne = Vector3.one;
    private float scaleFactor;
    private float scaleFactorHP;
    private float finalScale;
    private float distance;

    #region GetSet Properties
    public List<NameTagInfo> GetSetNameTagInfoList {
        get { return nameTagInfoList; }
        set { nameTagInfoList = value; }
    }
    #endregion

    private void Awake() {
        camera = Camera.main;
    }

    private void LateUpdate() {
        for (int i = 0; i < nameTagInfoList.Count; i++) {
            if (nameTagInfoList[i].uiTag.gameObject.activeSelf) {
                nameTagInfoList[i].uiTag.LookAt(
                    nameTagInfoList[i].uiTag.position + camera.transform.rotation * vectorForward,
                    camera.transform.rotation * vectorUp
                );

                distance = Vector3.Distance(nameTagInfoList[i].uiTag.position, camera.transform.position);
                scaleFactor = distance / 10f;
                scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1.5f);
                scaleFactorHP = distance / 15f;
                scaleFactorHP = Mathf.Clamp(scaleFactorHP, 0.08f, 0.1f);
                finalScale = nameTagInfoList[i].isHP ? scaleFactorHP : scaleFactor;

                nameTagInfoList[i].uiTag.localScale = vectorOne * finalScale;
            }
        }
    }
}