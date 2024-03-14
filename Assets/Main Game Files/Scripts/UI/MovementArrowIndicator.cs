using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrowIndicator : MonoBehaviour {
    [Header("Game Object and others")]
    [SerializeField] private GameObject arrowHolder;

    private float horizontalInput;
    private float verticalInput;
    private float targetAngle;
    private Quaternion targetRotation;
    private Transform arrowTransform;

    private void Awake() {
        arrowTransform = arrowHolder.transform;
    }

    private void Start () {
        HideShowArrow(showArrow: false);
    }

    public void RotateArrow(MovementJoystick movementJoystick) {
        horizontalInput = movementJoystick.Horizontal;
        verticalInput = movementJoystick.Vertical;

        // Calculate the angle from the input
        targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;

        // Create a rotation quaternion around the Z-axis
        targetRotation = Quaternion.Euler(0f, 0f, -targetAngle);

        // Set the rotation directly
        arrowTransform.rotation = targetRotation;
    }

    public void HideShowArrow(bool showArrow) {
        arrowHolder.SetActive(showArrow);
    }
}