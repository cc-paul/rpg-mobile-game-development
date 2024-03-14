using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxManager : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private GameObject messageBox;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button btnClose;

    private Coroutine hideMessageDelay;
    private float expectedCooldown = 1.5f;
    private float timer = 0f;

    private void Awake() {
        btnClose.onClick.AddListener(() => {
            messageBox.SetActive(false);
        });
    }

    public void ShowMessage(string currentMessage) {
        message.SetText(currentMessage);
        timer = 0f;

        if (!messageBox.activeSelf) {
            StartCoroutine(nameof(HideMessageDelay));
            messageBox.SetActive(true);
        }
    }

    private void Start() {
        btnClose.onClick.Invoke();
    }

    private IEnumerator HideMessageDelay() {
        while (timer < expectedCooldown) {
            timer += Time.deltaTime;
            yield return null;
        }

        btnClose.onClick.Invoke();
    }
}
