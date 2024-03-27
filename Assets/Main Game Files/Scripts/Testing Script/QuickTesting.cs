using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class QuickTesting : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject menuManager;
    [SerializeField] private GameObject testMenuManager;

    [Space(2)]

    [Header("Buttons")]
    [SerializeField] private Button btnTestButton;
    [SerializeField] private Button btnCloseTestWindow;
    [SerializeField] private Button btnAddMobs;

    [Space(2)]

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI mobsCounterText;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private EnemyAIManager enemyAIManager;

    private int mobsCounter;

    private void Awake() {
        btnTestButton.onClick.AddListener(OpenTestWindow);
        btnCloseTestWindow.onClick.AddListener(CloseTestWindow);
        btnAddMobs.onClick.AddListener(AddTestMobs);
    }

    private void OpenTestWindow() {
        menuManager.SetActive(false);
        testMenuManager.SetActive(true);
    }

    private void CloseTestWindow() {
        testMenuManager.SetActive(false);
    }

    private void AddTestMobs() {
        mobsCounter++;
        mobsCounterText.text = mobsCounter.ToString();
        Timing.RunCoroutine(enemyAIManager.TestSpawnMobs());
    }
}
