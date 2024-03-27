using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {
    [Header("Zoom Buttons")]
    public Button zoomInButton;
    public Button zoomOutButton;

    [Space(2)]

    [Header("Target and Camera")]
    [SerializeField] private Transform target;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera UiCamera;

    [Space(2)]

    [Header("Zoom")]
    [SerializeField] private float minFOV;
    [SerializeField] private float maxFOV;
    [SerializeField] private float zoomSpeed = 5f;

    [Space(2)]

    [Header("Distance")]
    [SerializeField] private float distanceFromTarget = 0f;

    [Space(2)]

    [Header("Rotation Sensitivity")]
    [SerializeField] private float sensitivityX = 4f;
    [SerializeField] private float sensitivityY = 1f;

    [Space(2)]

    [Header("Clamp and Offset")]
    [SerializeField] private Vector2 clampY = new(5, 89);              //x is minimum, y is maximum
    [SerializeField] private Vector3 offset = new(0, 5, -5);

    [Space(2)]

    [Header("Joystick")]
    [SerializeField] private MovementJoystick movementJoystick;

    private bool isPressed;
    private int pointerID;
    private float currentX = 0f;
    private float currentY = 0f;
    private float currentFOV;
    private float prevTouchDeltaMag;
    private float touchDeltaMag;
    private float fov;
    private float deltaMagnitudeDiff;

    private Vector3 position;
    private Quaternion rotation;
    private Touch touch;
    private Touch touchZero;
    private Touch touchOne;
    private Vector2 touchZeroPrevPos;
    private Vector2 touchOnePrevPos;
    private Transform cameraTransform;

    #region CameraControl GetSet Properties
    public bool GetSetIsPressed {
        get { return isPressed; }
        set { isPressed = value; }
    }

    public int GetSetPointerID {
        get { return pointerID; }
        set { pointerID = value; }
    }
    #endregion

    private void Awake() {
        cameraTransform = transform;
        UpdateTarget(target);
        StartControllingTheCamera();

        zoomInButton.onClick.AddListener(ZoomIn);
        zoomOutButton.onClick.AddListener(ZoomOut);
    }

    public void UpdateTarget(Transform _target) {
        target = _target;
        cameraTransform.position = target.position - transform.forward * distanceFromTarget;
        cameraTransform.localEulerAngles = new Vector3(currentX, -currentY, 0);
    }

    /*private void LateUpdate() {
        if (target == null) return;

        #if !UNITY_EDITOR
            HandleTouch();
        #else
            HandleMouse();
        #endif

        rotation = Quaternion.Euler(currentY, currentX, 0);
        position = rotation * new Vector3(0f, 0f, -distanceFromTarget - currentFOV) + target.position + offset;

        cameraTransform.rotation = rotation;
        cameraTransform.position = position;
    }*/

    public void StartControllingTheCamera() {
        #if !UNITY_EDITOR
            HandleTouch();
        #else
            HandleMouse();
        #endif

        rotation = Quaternion.Euler(currentY, currentX, 0);
        position = rotation * new Vector3(0f, 0f, -distanceFromTarget - currentFOV) + target.position + offset;

        cameraTransform.rotation = rotation;
        cameraTransform.position = position;
    }


    private void HandleTouch() {
        if (!isPressed) return;

        //Rotating Camera whne only 1 finger is touching the screen
        if (Input.touchCount == 1) {
            touch = Input.GetTouch(0);

            currentX += touch.deltaPosition.x * sensitivityX / 100;
            currentY -= touch.deltaPosition.y * sensitivityY / 100;

            currentY = Mathf.Clamp(currentY, clampY.x, clampY.y);
        }
        //Zooming camera when 2 fingers are touching the screen
        if (Input.touchCount == 2) {
            if (movementJoystick.Input == Vector2.zero) {
                touchZero = Input.GetTouch(0);
                touchOne = Input.GetTouch(1);

                touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                fov = currentFOV + -(deltaMagnitudeDiff * zoomSpeed / 100);
                fov = Mathf.Clamp(fov, maxFOV, minFOV);
                currentFOV = fov;
            } else {
                touch = Input.GetTouch(pointerID);

                currentX += touch.deltaPosition.x * sensitivityX / 100;
                currentY -= touch.deltaPosition.y * sensitivityY / 100;

                currentY = Mathf.Clamp(currentY, clampY.x, clampY.y);
            }
        }
    }

    private void HandleMouse() {
        distanceFromTarget = 3f;

        //Zoom with scrollwheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (currentFOV > maxFOV) {
                currentFOV -= 1 * zoomSpeed;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (currentFOV < minFOV) {
                currentFOV += 1 * zoomSpeed;
            }
        }

        if (!isPressed) return;

        //Rotate Camera
        if (Input.GetMouseButton(0)) {
            

            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;

            currentY = Mathf.Clamp(currentY, clampY.x, clampY.y);
        }
    }

    private void ZoomIn() {
        currentFOV -= zoomSpeed;
        currentFOV = Mathf.Clamp(currentFOV, maxFOV, minFOV);
        StartControllingTheCamera();
    }

    private void ZoomOut() {
        currentFOV += zoomSpeed;
        currentFOV = Mathf.Clamp(currentFOV, maxFOV, minFOV);
        StartControllingTheCamera();
    }
}