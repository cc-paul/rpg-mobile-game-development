using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDropAssignment : MonoBehaviour, IDropHandler {
    [Header("Game Object and others")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject skillSettings;

    [Header("UI")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Button btnDelete;

    private int skillID = -1;
    private SkillSetup skillSetup;

    #region GetSet Properties
    public int GetSetSkillID {
        get { return skillID; }
        set { skillID = value; }
    }
    #endregion

    public void Awake() {
        skillSetup = skillSettings.GetComponent<SkillSetup>();

        btnDelete.onClick.AddListener(() => {
            ResetQuickSlot();
        });
    }

    public void OnDrop(PointerEventData eventData) {
        GameObject droppendSkillIcon = eventData.pointerDrag;

        if (droppendSkillIcon.GetComponent<SkillDragAssignment>() != null) {
            SkillDragAssignment draggableItem = droppendSkillIcon.GetComponent<SkillDragAssignment>();

            skillSetup.RemoveExistingSkillID(draggableItem.GetSetSkillID);
            skillID = draggableItem.GetSetSkillID;
            iconImage.sprite = draggableItem.GetComponent<Image>().sprite;
        }
    }

    public void ResetQuickSlot() {
        skillID = -1;
        iconImage.sprite = defaultIcon;   
    }
}
