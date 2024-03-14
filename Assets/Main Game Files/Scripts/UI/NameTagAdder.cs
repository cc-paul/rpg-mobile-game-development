using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTagAdder : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private List<NameTagInfo> nameTagInfoList = new List<NameTagInfo>();

    private TagLookAtCamera tagLookAtCamera;
    private GameObject basicInfoUI;
    private Transform nameAndHPHolder;

    private void Awake() {
        tagLookAtCamera = FindAnyObjectByType<TagLookAtCamera>();
    }

    private void OnEnable() {
        LoopAndAddToCamera();
    }

    private void LoopAndAddToCamera() {
        foreach (NameTagInfo nameTagInfo in nameTagInfoList) {
            if (!tagLookAtCamera.GetSetNameTagInfoList.Exists(tagInfo => tagInfo.uiTag == nameTagInfo.uiTag)) {
                AddOnLookAtCamera(uiInfo: nameTagInfo.uiTag, uiIsHP: false);
            }
        }
    }

    public void AddOnLookAtCamera(Transform uiInfo, bool uiIsHP) {
        tagLookAtCamera.GetSetNameTagInfoList.Add(new NameTagInfo {
            uiTag = uiInfo,
            isHP = uiIsHP
        });
    }

    public void RemoveFromNameTag(Transform uiInfoToRemove) {
        for (int i = 0; i < tagLookAtCamera.GetSetNameTagInfoList.Count; i++) {
            if (tagLookAtCamera.GetSetNameTagInfoList[i].uiTag == uiInfoToRemove) {
                tagLookAtCamera.GetSetNameTagInfoList[i].uiTag.gameObject.SetActive(false);
                return;
            }
        }
    }
}
