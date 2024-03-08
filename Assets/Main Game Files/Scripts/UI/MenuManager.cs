using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [Header("Main UI Buttons")]
    [SerializeField] private Button btnMenuManager;
    [SerializeField] private Button btnClose;
    [SerializeField] private GameObject menuManagerPanel;

    [Space(2)]

    [Header("Skill Window")]
    [SerializeField] private Button btnSkillWindow;
    [SerializeField] private GameObject skillWindowPanel;

    private void Awake() {
        btnMenuManager.onClick.AddListener(() => {
            menuManagerPanel.SetActive(true);
        });

        btnClose.onClick.AddListener(() => {
            menuManagerPanel.SetActive(false);
        });

        btnClose.onClick.AddListener(() => {
            menuManagerPanel.SetActive(false);
        });

        btnSkillWindow.onClick.AddListener(() => {
            skillWindowPanel.SetActive(true);
            menuManagerPanel.SetActive(false);
        });
    }

    private void Start() {
        menuManagerPanel.SetActive(false);
    }
}
