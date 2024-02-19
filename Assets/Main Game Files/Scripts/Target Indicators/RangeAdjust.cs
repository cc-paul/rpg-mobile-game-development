using UnityEngine;

public class RangeAdjust : MonoBehaviour {
    [Header("Variable Declaration and Adjustment")]
    [SerializeField] private float range = 1f;
    [SerializeField] private float childRange = 1f;
    [SerializeField] private bool childScaleWithParent;

    private Vector3 initialChildScale;
    private Transform childTransform;

    public float GetSetRange {
        get { return range; }
        set { range = value; }
    }

    private void Awake() {
        // Find the child object and store its initial scale
        childTransform = transform.GetChild(0);
        initialChildScale = childTransform.localScale * childRange;
    }

    public void DoTheRescaling() {
        transform.localScale = new Vector3(range, range, range);

        if (childScaleWithParent) {
            // Calculate the scaling factor for the parent
            float scaleFactor = childRange / transform.localScale.x;

            // Apply the scaling factor to the child's initial scale
            Vector3 newChildScale = new Vector3(
                initialChildScale.x * scaleFactor,
                initialChildScale.y * scaleFactor,
                initialChildScale.z * scaleFactor
            );

            // Apply the new scale to the child object
            childTransform.localScale = newChildScale;
        }
    }
}
