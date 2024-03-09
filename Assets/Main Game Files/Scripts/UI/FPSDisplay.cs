using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtFps;

    private float deltaTime = 0f;
    private float fps;
    private float msec;
    private int targetFPS = 61;
    private int roundedFps;
    private int roundedMsec;
    private string textFps;

    private void Awake() {
        Application.targetFrameRate = targetFPS;
    }

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.01f;

        msec = deltaTime * 1000f;
        fps = 1.0f / deltaTime;


        roundedFps = Mathf.RoundToInt(fps);
        roundedMsec = Mathf.RoundToInt(msec);

        textFps = string.Format($"{roundedMsec} MS ({roundedFps} FPS)");
        txtFps.text = textFps;
    }
}