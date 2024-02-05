using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject model;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private MovementJoystick movementJoystick;

    private CharacterController characterController;
    private StatsManager statsManager;
    private BasicAnimation basicAnimation;

    private Vector2 input;
    private Vector2 inputDir;
    private Vector3 velocity;
    private Vector3 down = Vector3.down;
    private Transform cameraTransform;
    private Ray ray;
    private RaycastHit hitInfo;
    private Coroutine playerMovementCourotine;
    private float ySpeed;
    private float turnSmoothTime = 0.0f;
    private float turnSmoothVelocity;
    private float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float currentSpeed;
    private float targetRotation;
    private float targetSpeed;
    private bool isRunning;

    #region GetSet Properties
    public bool GetSetIsRunning {
        get { return isRunning; }
        set { isRunning = value; }
    }
    #endregion

    private void Awake() {
        characterController = controller.GetComponent<CharacterController>();
        basicAnimation = GetComponent<BasicAnimation>();
        statsManager = GetComponent<StatsManager>();
        cameraTransform = Camera.main.transform;
    }

    public void InitiatePlayerMovement() {
        if (playerMovementCourotine != null) {
            StopCoroutine(nameof(PlayerStartMoving));
        }

        playerMovementCourotine  = StartCoroutine(nameof(PlayerStartMoving));
    }

    private IEnumerator PlayerStartMoving() {
        while (true) {
            input = new Vector2(
                movementJoystick.Horizontal,
                movementJoystick.Vertical
            );

            inputDir = input.normalized;

            if (inputDir == Vector2.zero) {
                basicAnimation.PlayBasicAnimation(Global.AnimationCategory.Idle);
                break;
            }

            ySpeed += Physics.gravity.y * Time.deltaTime;

            targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            controller.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                controller.transform.eulerAngles.y,
                targetRotation, ref turnSmoothVelocity,
                turnSmoothTime
            );

            targetSpeed = statsManager.Speed.Value * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(
                currentSpeed,
                targetSpeed,
                ref speedSmoothVelocity,
                speedSmoothTime
            );

            velocity = controller.transform.forward * currentSpeed;
            velocity = AdjustVelocityToSlope(velocity);
            velocity.y += ySpeed + 1f;

            characterController.Move(velocity * Time.deltaTime);
            basicAnimation.PlayBasicAnimation(isRunning ? Global.AnimationCategory.Run : Global.AnimationCategory.Walk);

            yield return null;
        }
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity) {
        ray = new Ray(controller.transform.position, down);

        Debug.DrawRay(ray.origin, hitInfo.normal, Color.green);

        if (Physics.Raycast(ray, out hitInfo, 0.05f)) {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0) {
                return adjustedVelocity;
            }
        }

        return velocity;
    }
}