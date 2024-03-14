using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSetter : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Sprite allyIndicator;
    [SerializeField] private Sprite enemyIndacor;
    [SerializeField] private Sprite lineIndicator;


    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillSettings;

    [Space(10)]

    [Header("Variable Declarations and Other Assignment")]
    [SerializeField] private bool isParentTargetSetter;
    [SerializeField] private bool isLine;

    private SkillReference skillReference;
    private TargetManager targetManager;

    private void Awake() {
        targetManager = skillSettings.GetComponent<TargetManager>();
        skillReference = skillSettings.GetComponent<SkillReference>();
    }

    private void OnTriggerEnter(Collider target) {
        SetTargetIndicator(target: target.gameObject, addIt: true);
    }

    private void OnEnable() {
        bool forAlly = skillReference.GetSkillForAlly(skillID: skillReference.GetSetFinalSkillID);

        gameObject.GetComponent<Image>().sprite = isLine ? lineIndicator : forAlly ? allyIndicator : enemyIndacor;
    }

/*    private void OnTriggerStay(Collider target) {
        SetTargetIndicator(target: target.gameObject, addIt: true);
    }*/

    private void OnTriggerExit(Collider target) {
        SetTargetIndicator(target: target.gameObject, addIt: false);
    }

    private void SetTargetIndicator(GameObject target, bool addIt) {
        bool forAlly = skillReference.GetSkillForAlly(skillID: skillReference.GetSetFinalSkillID);
        List<string> allowedTarget = new List<string>();

        if (forAlly) {
            allowedTarget.Add(Global.PLAYER_OTHERS);
        } else {
            allowedTarget.Add(Global.MOB);
        }

        if (allowedTarget.Contains(target.tag)) {
            if (isParentTargetSetter) {
                if (addIt) {
                    targetManager.AddTargets(target: target, addToParent: true);
                } else {
                    targetManager.RemoveTargets(target, removeToParent: true);
                }
            } else {
                if (addIt) {
                    targetManager.GetSetNearestTarget = target.gameObject;
                    targetManager.AddTargets(target: target, addToParent: false);
                } else {
                    targetManager.RemoveTargets(target: target, removeToParent: false);
                }
            }
        }
    }
}