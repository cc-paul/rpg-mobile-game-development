using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private Button btnQuickSlot;
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnSkillWindow;

    [Space(2)]

    [Header("Game Object and Others")]
    [SerializeField] private GameObject quickSlotPanel;
    [SerializeField] private GameObject skillWindowPanel;

    private void Awake() {
        btnQuickSlot.onClick.AddListener(() => {
            quickSlotPanel.gameObject.SetActive(true);
        });

        btnClose.onClick.AddListener(() => {
            quickSlotPanel.gameObject.SetActive(false);
        });

        btnSkillWindow.onClick.AddListener(() => {
            skillWindowPanel.gameObject.SetActive(true);
            quickSlotPanel.gameObject.SetActive(false);
        });
    }
}
