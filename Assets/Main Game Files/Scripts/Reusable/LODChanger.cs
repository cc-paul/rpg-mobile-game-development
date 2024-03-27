using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class LODChanger : MonoBehaviour {    
    [Header("Variable Declaration")]
    [SerializeField] private float distanceToChange;

    [Space(2)]

    [Header("LOD 0")]
    [SerializeField] private List<GameObject> lodSet0;

    [Space(2)]

    [Header("LOD 1")]
    [SerializeField] private List<GameObject> lodSet1;

    private Camera camera;
    private CoroutineHandle lodCoroutine;
    private float distance;

    private void Awake() {
        camera = Camera.main;

        lodCoroutine = Timing.RunCoroutine(ChangeLOD());
        Timing.PauseCoroutines(lodCoroutine);
    }

    private void OnEnable() {
        Timing.ResumeCoroutines(lodCoroutine);
        SetDefaultLOD();
    }

    private void OnDisable() {
        Timing.PauseCoroutines(lodCoroutine);
    }

    private IEnumerator<float> ChangeLOD() {
        while (true) {
            distance = Vector3.Distance(camera.transform.position, transform.position);

            ChangeLOD0_Visibility(state: distance < distanceToChange);
            ChangeLOD1_Visibility(state: distance > distanceToChange);

            yield return Timing.WaitForOneFrame;
        }
    }

    public void SetDefaultLOD() {
        ChangeLOD0_Visibility(state: true);
        ChangeLOD1_Visibility(state: false);
    }

    private void ChangeLOD0_Visibility(bool state) {
        for (int lodIndex = 0; lodIndex < lodSet0.Count; lodIndex++) {
            lodSet0[lodIndex].SetActive(state);
        }
    }

    private void ChangeLOD1_Visibility(bool state) {
        for (int lodIndex = 0; lodIndex < lodSet1.Count; lodIndex++) {
            lodSet1[lodIndex].SetActive(state);
        }
    }
}