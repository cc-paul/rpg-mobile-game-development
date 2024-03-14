using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour {
    [Header("Game Object and Others")]
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject targetIndicators;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private MovementJoystick movementJoystick;

    [Space(2)]

    [Header("UI")]
    [SerializeField] private Button btnRunAndWalk;
    [SerializeField] private Image imgRun;
    [SerializeField] private Image imgWalk;

    private CharacterController characterController;
    private PlayerStatsManager playerStatsManager;
    private BasicAnimation basicAnimation;
    private TargetPositioning skillTargetPositioning;
    private SkillBaseCast skillBaseCast;

    private WaitForSeconds movementNullWait = new WaitForSeconds(0f);
    private Vector2 input;
    private Vector2 inputDir;
    private Vector3 velocity;
    private Vector3 adjustedVelocity;
    private Vector3 down = Vector3.down;
    private Vector3 up = Vector3.up;
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
    private Transform controllerTransform;
    private Quaternion slopeRotation;

    private StatModifier basicAddedSpeed;

    #region GetSet Properties
    public bool GetSetIsRunning {
        get { return isRunning; }
        set { isRunning = value; }
    }
    #endregion

    private void Awake() {
        skillTargetPositioning = targetIndicators.GetComponent<TargetPositioning>();
        characterController = controller.GetComponent<CharacterController>();
        basicAnimation = GetComponent<BasicAnimation>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        skillBaseCast = skillSettings.GetComponent<SkillBaseCast>();
        cameraTransform = Camera.main.transform;
        controllerTransform = controller.transform;
    }

    private void Start() {
        basicAddedSpeed = new StatModifier(4, Global.StatModType.Flat, this);

        SetupRunAndWalkButton();
    }

    private void OnEnable() {
        btnRunAndWalk.onClick.AddListener(() => {
            isRunning = !isRunning;

            if (isRunning) {
                playerStatsManager.Speed.AddModifier(basicAddedSpeed);
            } else {
                playerStatsManager.Speed.RemoveModifier(basicAddedSpeed);
            }

            SetupRunAndWalkButton();
        });
    }

    public void InitiatePlayerMovement() {
        if (playerMovementCourotine == null) {
            playerMovementCourotine = StartCoroutine(nameof(PlayerStartMoving));
        }
    }

    private IEnumerator PlayerStartMoving() {
        while (true) {
            input = new Vector2(
                movementJoystick.Horizontal,
                movementJoystick.Vertical
            );

            inputDir = input.normalized;

            if ((
                skillBaseCast.GetSetIsCastingSkill && !skillBaseCast.GetSetEnableCancelingSkill) ||
                playerStatsManager.GetSetIsPlayerDead
            ) {
                playerMovementCourotine = null;
                break;
            }

            if (inputDir == Vector2.zero) {
                basicAnimation.PlayBasicAnimation(_animationCategory: Global.AnimationCategory.Idle);
                skillTargetPositioning.RepositionTargetIndicator();
                playerMovementCourotine = null;
                break;
            }

            ySpeed += Physics.gravity.y * Time.deltaTime;

            targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            controllerTransform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(
                controllerTransform.eulerAngles.y,
                targetRotation, ref turnSmoothVelocity,
                turnSmoothTime
            );

            targetSpeed = playerStatsManager.Speed.Value * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(
                currentSpeed,
                targetSpeed,
                ref speedSmoothVelocity,
                speedSmoothTime
            );

            velocity = controllerTransform.forward * currentSpeed;
            velocity = AdjustVelocityToSlope(_velocity: velocity);
            velocity.y += ySpeed + 1f;

            characterController.Move(velocity * Time.deltaTime);
            basicAnimation.PlayBasicAnimation(_animationCategory: isRunning ? Global.AnimationCategory.Run : Global.AnimationCategory.Walk);
            skillBaseCast.GetSetIsCastingSkill = false;
            skillTargetPositioning.RepositionTargetIndicator();
            

            yield return movementNullWait;
        }
    }

    private Vector3 AdjustVelocityToSlope(Vector3 _velocity) {
        ray = new Ray(controllerTransform.position, down);

        Debug.DrawRay(ray.origin, hitInfo.normal, Color.green);

        if (Physics.Raycast(ray, out hitInfo, 0.05f)) {
            slopeRotation = Quaternion.FromToRotation(up, hitInfo.normal);
            adjustedVelocity = slopeRotation * _velocity;

            if (adjustedVelocity.y < 0) {
                return adjustedVelocity;
            }
        }

        return _velocity;
    }

    private void SetupRunAndWalkButton() {
        imgRun.gameObject.SetActive(!isRunning);
        imgWalk.gameObject.SetActive(isRunning);
    }
}