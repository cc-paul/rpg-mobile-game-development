/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLookAtCamera : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private List<NameTagInfo> nameTagInfoList = new List<NameTagInfo> ();

    [Space(2)]

    [Header("Variable declarations and assignments")]
    [SerializeField] private bool getChildElement = false;

    private Camera camera;
    private Vector3 vectorForward = Vector3.forward;
    private Vector3 vectorUp = Vector3.up;
    private Vector3 vectorOne = Vector3.one;
    private float scaleFactor;
    private float scaleFactorHP;
    private float finalScale;
    private float distance;

    private GameObject basicInfoUI;
    private Transform nameAndHPHolder;

    private void Awake() {
        camera = Camera.main;
    }

    private void Start() {
        if (!getChildElement) return;
        if (gameObject.transform.childCount == 0) return;

        foreach (Transform currentObject in gameObject.transform) {
            basicInfoUI = currentObject.transform.Find(Global.BASIC_INFO_UI_V2).gameObject;
            nameAndHPHolder = basicInfoUI.transform.GetChild(0);

            AddOnLookAtCamera(uiInfo: nameAndHPHolder,uiIsHP: false);
        }
    }

    public void AddOnLookAtCamera(Transform uiInfo,bool uiIsHP) {
        nameTagInfoList.Add(new NameTagInfo {
            uiTag = uiInfo,
            isHP = uiIsHP
        });
    }

    private void LateUpdate() {
        foreach (NameTagInfo nameTagInfo in nameTagInfoList) {
            if (nameTagInfo.uiTag.gameObject.activeSelf) {
                nameTagInfo.uiTag.LookAt(
                    nameTagInfo.uiTag.position + camera.transform.rotation * vectorForward,
                    camera.transform.rotation * vectorUp
                );

                distance = Vector3.Distance(nameTagInfo.uiTag.position, camera.transform.position);
                scaleFactor = distance / 10f;
                scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1.5f);
                scaleFactorHP = distance / 15f;
                scaleFactorHP = Mathf.Clamp(scaleFactorHP, 0.091f, 0.5f);
                finalScale = nameTagInfo.isHP ? scaleFactorHP : scaleFactor;

                nameTagInfo.uiTag.localScale = vectorOne * finalScale;
            }
        }
    }
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLookAtCamera : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private List<NameTagInfo> nameTagInfoList = new List<NameTagInfo>();

    [Space(2)]

    [Header("Variable declarations and assignments")]
    [SerializeField] private bool getChildElement = false;

    private Camera camera;
    private Vector3 vectorForward = Vector3.forward;
    private Vector3 vectorUp = Vector3.up;
    private Vector3 vectorOne = Vector3.one;
    private float scaleFactor;
    private float scaleFactorHP;
    private float finalScale;
    private float distance;

    private GameObject basicInfoUI;
    private Transform nameAndHPHolder;

    private void Awake() {
        camera = Camera.main;
    }

    private void Start() {
        if (!getChildElement) return;
        if (gameObject.transform.childCount == 0) return;

        foreach (Transform currentObject in gameObject.transform) {
            basicInfoUI = currentObject.transform.Find(Global.BASIC_INFO_UI_V2).gameObject;
            nameAndHPHolder = basicInfoUI.transform.GetChild(0);

            AddOnLookAtCamera(uiInfo: nameAndHPHolder, uiIsHP: false);
        }
    }

    public void AddOnLookAtCamera(Transform uiInfo, bool uiIsHP) {
        nameTagInfoList.Add(new NameTagInfo {
            uiTag = uiInfo,
            isHP = uiIsHP
        });
    }

    public void RemoveFromNameTag(Transform uiInfoToRemove) {
        for (int i = 0; i < nameTagInfoList.Count; i++) {
            if (nameTagInfoList[i].uiTag == uiInfoToRemove) {
                nameTagInfoList.RemoveAt(i);
                return;
            }
        }
    }

    private void LateUpdate() {
        if (nameTagInfoList.Count == 0) return;

        foreach (NameTagInfo nameTagInfo in nameTagInfoList) {
            nameTagInfo.uiTag.LookAt(
                nameTagInfo.uiTag.position + camera.transform.rotation * vectorForward,
                camera.transform.rotation * vectorUp
            );

            distance = Vector3.Distance(nameTagInfo.uiTag.position, camera.transform.position);
            scaleFactor = distance / 10f;
            scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1.5f);
            scaleFactorHP = distance / 15f;
            scaleFactorHP = Mathf.Clamp(scaleFactorHP, 0.091f, 0.5f);
            finalScale = nameTagInfo.isHP ? scaleFactorHP : scaleFactor;

            nameTagInfo.uiTag.localScale = vectorOne * finalScale;
        }
    }
}
