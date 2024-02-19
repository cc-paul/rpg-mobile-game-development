using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTarget : MonoBehaviour {
    [Header("Game Objects or Others")]
    [SerializeField] private GameObject myCamera;
    [SerializeField] private RectTransform lineRange;

    private BoxCollider lineTargetCollider;
    
    private void Awake() {
        lineTargetCollider = lineRange.GetComponent<BoxCollider>();
    }

    public void ResizeTheCollider() {
        Vector3 size = lineTargetCollider.size;
        size.x = lineRange.rect.width;
        size.y = lineRange.rect.height;
        lineTargetCollider.size = size;
    }


    public void ControlTheChildTarget(SkillJoystick skillJoystick) {
        Vector3 inputVector = new Vector3(skillJoystick.Horizontal, 0, skillJoystick.Vertical).normalized;       // Calculate the joystick input vector.
        Vector3 inputInCameraSpace = myCamera.transform.TransformDirection(inputVector);                         // Transform the joystick input into the camera's local space.
        float rotationAngle = Mathf.Atan2(inputInCameraSpace.x, inputInCameraSpace.z) * Mathf.Rad2Deg;           // Calculate the desired rotation angle based on the transformed joystick input.

        
        transform.rotation = Quaternion.Euler(90.0f, 0.0f, -rotationAngle);                                      // Rotate the line target around the center (0, 0, 0) of your circle.
    }
}
