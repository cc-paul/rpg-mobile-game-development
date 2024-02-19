using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrowIndicator : MonoBehaviour {
    [Header("Game Object and others")]
    [SerializeField] private GameObject arrowHolder;

    private void Start () {
        HideShowArrow(showArrow: false);
    }

    public void RotateArrow(MovementJoystick movementJoystick) {
        float horizontalInput = movementJoystick.Horizontal;
        float verticalInput = movementJoystick.Vertical;

        // Calculate the angle from the input
        float targetAngle = Mathf.Atan2(horizontalInput, verticalInput) * Mathf.Rad2Deg;

        // Create a rotation quaternion around the Z-axis
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -targetAngle);

        // Set the rotation directly
        arrowHolder.transform.rotation = targetRotation;
    }

    public void HideShowArrow(bool showArrow) {
        arrowHolder.SetActive(showArrow);
    }
}