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
        Invoke(nameof(ReturnToPool), useClipTransitionDuration ? AnimationLength() : duration);
    }

    private void ReturnToPool() {
        gameObject.SetActive(false);
    }

    public float AnimationLength() {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        AnimationClip currentClip = clips[0];
        return currentClip.length;
    }
}

