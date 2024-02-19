using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDragAssignment : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private GameObject skillIconParent;
    private GameObject tempSkillIconParent;
    private int skillID = 0;
    private Image image;

    #region GetSet Properties
    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }

    public GameObject GetSetSkillIconParent {
        get { return skillIconParent; }
        set { skillIconParent = value; }
    }

    public GameObject GetSetTempSkillIconParent {
        get { return tempSkillIconParent; }
        set { tempSkillIconParent = value; }
    }
    #endregion

    private void Awake() {
        image = GetComponent<Image>();
        skillIconParent = transform.parent.gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.SetParent(tempSkillIconParent.transform);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(skillIconParent.transform);
        image.raycastTarget = true;
    }
}
