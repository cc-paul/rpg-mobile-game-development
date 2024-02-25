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

    private void Awake() {
        btnClose.onClick.AddListener(() => {
            messageBox.SetActive(false);
        });
    }

    public void ShowMessage(string currentMessage) {
        message.SetText(currentMessage);

        if (hideMessageDelay != null) {
            StopCoroutine(hideMessageDelay);
        }

        hideMessageDelay = StartCoroutine(nameof(HideMessageDelay));

        if (!messageBox.activeSelf) {
            messageBox.SetActive(true);
        }
    }

    private void Start() {
        btnClose.onClick.Invoke();
    }

    private IEnumerator HideMessageDelay() {
        yield return new WaitForSeconds(1.5f);
        btnClose.onClick.Invoke();
    }
}
