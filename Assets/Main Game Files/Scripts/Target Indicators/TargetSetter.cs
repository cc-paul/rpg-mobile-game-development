using System.Collections.Generic;
using UnityEngine;

public class TargetSetter : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject skillSettings;

    [Space(10)]

    [Header("Variable Declarations and Other Assignment")]
    [SerializeField] private bool isParentTargetSetter;

    private SkillReference skillReference;
    private TargetManager targetManager;

    private void Awake() {
        targetManager = skillSettings.GetComponent<TargetManager>();
        skillReference = skillSettings.GetComponent<SkillReference>();
    }

    private void OnTriggerEnter(Collider target) {
        SetTargetIndicator(target.gameObject, true);
    }

    private void OnTriggerStay(Collider target) {
        SetTargetIndicator(target.gameObject, true);
    }

    private void OnTriggerExit(Collider target) {
        SetTargetIndicator(target.gameObject, false);
    }

    private void SetTargetIndicator(GameObject target, bool addIt) {
        bool forAlly = skillReference.GetSkillForAlly(skillReference.GetSetFinalSkillID);
        List<string> allowedTarget = new List<string>();

        if (forAlly) {
            allowedTarget.Add(Global.PLAYER_OTHERS);
            targetManager.AddTargets(playerModel, true);
        } else {
            allowedTarget.Add(Global.MOB);
        }

        if (allowedTarget.Contains(target.tag)) {
            if (isParentTargetSetter) {
                if (addIt) {
                    targetManager.AddTargets(target, true);
                } else {
                    targetManager.RemoveTargets(target, true);
                }
            } else {
                if (addIt) {
                    targetManager.GetSetNearestTarget = target.gameObject;
                    targetManager.AddTargets(target, false);
                } else {
                    targetManager.RemoveTargets(target, false);
                }
            }
        }
    }
}