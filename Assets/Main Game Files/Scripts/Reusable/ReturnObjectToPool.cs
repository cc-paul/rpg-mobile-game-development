using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnObjectToPool : MonoBehaviour {
    [Header("Variable Declarations and Other Assignments")]
    [SerializeField] private float duration = 10f;
    [SerializeField] private bool useClipTransitionDuration;
    
    private Animator animator;

    public void Awake() {
        animator = GetComponent<Animator>();
    }

    public void InitializeReturn(GameObject spawnedObject) {
        StartCoroutine(ReturnToPool(spawnedObject: spawnedObject));
    }

    public IEnumerator ReturnToPool(GameObject spawnedObject,TagLookAtCamera tagLookAtCamera = null) {
        yield return new WaitForSeconds(useClipTransitionDuration ? AnimationLength() : duration);
        spawnedObject.SetActive(false);
    }

    public float AnimationLength() {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        AnimationClip currentClip = clips[0];
        return currentClip.length;
    }
}
