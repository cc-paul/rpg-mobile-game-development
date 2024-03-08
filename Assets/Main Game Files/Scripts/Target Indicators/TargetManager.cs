using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TargetManager : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject areaTargetContainer;
    [SerializeField] private GameObject lineRangeContainer;
    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject lineSkillLook;

    private List<GameObject> finalTargetList = new List<GameObject>();
    private List<GameObject> parentTargetList = new List<GameObject>();
    private List<GameObject> childTargetList = new List<GameObject>();
    private List<GameObject> shownArrowList = new List<GameObject>();
    private GameObject nearestTarget;
    private GameObject attackTarget;
    private GameObject assistTarget;
    private Vector3 nearestTargetPosition;
    private SkillReference skillReference;

    private float nearestTargetDistance = 0;
    private bool forAlly;
    

    #region GetSet Properties
    public GameObject GetSetNearestTarget {
        get { return nearestTarget; }
        set { nearestTarget = value; }
    }

    public GameObject GetSetLineSkillLook {
        get { return lineSkillLook; }
        set { lineSkillLook = value; }
    }

    public float GetSetNearestTargetDistance {
        get { return nearestTargetDistance; }
        set { nearestTargetDistance = value; }
    }

    public Vector3 GetSetNearestTargetPosition {
        get { return nearestTargetPosition; }
        set { nearestTargetPosition = value; }
    }

    public Vector3 GetSetPlayerPosition {
        get { return controller.transform.position; }
        set { controller.transform.position = value; }
    }

    public List<GameObject> GetSetFinalTargetList {
        get { return finalTargetList; }
        set { finalTargetList = value; }
    }
    #endregion 

    private void Awake() {
        skillReference = skillSettings.GetComponent<SkillReference>();
    }

    public void AddTargets(GameObject target, bool addToParent) {
        if (addToParent) {
            if (parentTargetList.Contains(target)) return;
            parentTargetList.Add(target);
        } else {
            if (childTargetList.Contains(target)) return;
            childTargetList.Add(target);
        }
        CallTargetIndicators();
    }

    public void RemoveTargets(GameObject target, bool removeToParent) {
        if (removeToParent) {
            if (!parentTargetList.Contains(target)) return;
            parentTargetList.Remove(target);
            RemoveLookTarget(target);
        } else {
            if (!childTargetList.Contains(target)) return;
            childTargetList.Remove(target);
            RemoveLookTarget(target);
        }
        CallTargetIndicators();
    }

    public void RemoveLookTarget(GameObject currentTarget) {
        if (ReferenceEquals(currentTarget, nearestTarget)) {
            nearestTarget = null;
            nearestTargetDistance = 0;
            nearestTargetPosition = Vector3.zero;
        }
    }

    public void ClearTargetList(bool includeFinal) {
        childTargetList.Clear();
        parentTargetList.Clear();
        shownArrowList.Clear();
        nearestTargetDistance = 0;
        nearestTarget = null;

        if (includeFinal) {
            finalTargetList.Clear();
        }
    }

    public bool IsThereAnEnemy() {
        return finalTargetList.Count != 0;
    }

    public List<GameObject> GetTargetList() {
        return finalTargetList;
    }

    private void CallTargetIndicators() {
        finalTargetList.Clear();

        if (childTargetList.Count != 0) {
            finalTargetList.Add(childTargetList[0]);
        }

        finalTargetList.AddRange(parentTargetList);
        finalTargetList = finalTargetList.Distinct().ToList();

        int maxTarget = skillReference.GetSkillMaxTarget(skillReference.GetSetFinalSkillID);

        if (finalTargetList.Count > maxTarget) {
            finalTargetList = finalTargetList.Take(maxTarget).ToList();
        }

        if (shownArrowList.Count != 0) {
            foreach (GameObject currentArrow in shownArrowList.ToArray()) {
                if (!finalTargetList.Contains(currentArrow.transform.parent.gameObject)) {
                    if (currentArrow != null) {
                        ShowHideTargetIndicators(currentArrow.transform.parent.gameObject, false);
                    }
                }
            }
        }

        foreach (GameObject currentTarget in finalTargetList.ToArray()) {
            ShowHideTargetIndicators(currentTarget, true);
        }

        if (finalTargetList.Count != 0) {
            GetTheNearestTarget();
        }
    }

    public void ShowHideTargetIndicators(GameObject target, bool showIt) {
        if (target != null) {
            Transform targetIndicator = null;

            if (target.tag.ToString() == Global.MOB) {
               targetIndicator = target.transform.Find(Global.TARGET_INDICATOR);
            } else if (
               target.tag.ToString() == Global.PLAYER_OTHERS ||
               target.tag.ToString() == Global.PLAYER
            ) {
                /*
                    Consider the ff when accepting target
                    1. Party
                    2. Duel
                */

                targetIndicator = target.transform.Find(Global.TARGET_NOTIFY);
                attackTarget = targetIndicator.Find(Global.TARGET_ATTACK).gameObject;
                assistTarget = targetIndicator.Find(Global.TARGET_HELP).gameObject;


                attackTarget.SetActive(false);
                assistTarget.SetActive(true);
            }

            if (targetIndicator != null) {
                targetIndicator.gameObject.SetActive(showIt);

                if (showIt) {
                    if (!shownArrowList.Contains(targetIndicator.gameObject)) {
                        shownArrowList.Add(targetIndicator.gameObject);
                    }
                } else {
                    shownArrowList.Remove(targetIndicator.gameObject);
                }
            }
        }
    }

    public void HideTargetContainer() {
        lineRangeContainer.SetActive(false);
        areaTargetContainer.SetActive(false);
    }

    public void AddToFinalTargetList(GameObject target) {
        if (!finalTargetList.Contains(target)) {
            finalTargetList.Add(target);
        }
    }

    public void HideAllTargetIndicators() {
        if (finalTargetList.Count != 0) {
            foreach (GameObject currentTarget in finalTargetList.ToArray()) {
                ShowHideTargetIndicators(currentTarget, false);
            }
        }
    }

    public void GetTheNearestTarget() {
        Dictionary<GameObject, Vector3> targetPosition = new Dictionary<GameObject, Vector3>();
        Vector3 myCurrentPosition = controller.transform.position;
        nearestTargetDistance = Mathf.Infinity;
        nearestTarget = null;

        foreach (GameObject currentTarget in GetTargetList()) {
            targetPosition.Add(currentTarget, currentTarget.transform.position);
        }

        foreach (var currentTarget_inList in targetPosition) {
            GameObject currentTarget = currentTarget_inList.Key;
            Vector3 currentTargetPosition = currentTarget_inList.Value;
            Vector3 directionToTarget = currentTargetPosition - myCurrentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (!ReferenceEquals(controller,currentTarget)) {
                if (dSqrToTarget < nearestTargetDistance) {
                    nearestTargetDistance = dSqrToTarget;
                    nearestTarget = currentTarget;
                    nearestTargetPosition = currentTargetPosition;
                }
            }
        }
    }

    public void LookAtNearestTarget() {
        if (nearestTarget == null || forAlly) return;

        Vector3 directionToTarget = (nearestTarget.transform.position - controller.transform.position).normalized;

        if (directionToTarget.sqrMagnitude > 0.0001f) {
            Quaternion yOnlyRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
            controller.transform.rotation = yOnlyRotation;
        }
    }
}