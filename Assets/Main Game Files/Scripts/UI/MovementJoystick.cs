using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    [Header("Variable Declaration and Adjustment")]
    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private float moveThreshold = 1;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;

    [Space(2)]

    [Header("UI")]
    [SerializeField] private Global.AxisOptions axisOptions = Global.AxisOptions.Both;
    [SerializeField] private RectTransform background = null;
    [SerializeField] private RectTransform handle = null;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private MovementController movementController;

    private MovementArrowIndicator movementArrowIndicator;
    private RectTransform baseRect = null;
    private Canvas canvas;
    private Camera cam;
    private Vector2 input = Vector2.zero;
    private Vector2 fixedPosition = Vector2.zero;
    private Vector2 center;
    private Vector2 zero = Vector2.zero;
    private Vector2 position;
    private Vector2 radius;
    private Vector2 localPoint;
    private Vector2 pivotOffset;
    private float angle;

    #region Joystick GetSet Properties
    public float Horizontal {
        get { return (snapX) ? SnapFloat(Input.x, Global.AxisOptions.Horizontal) : Input.x; }
    }

    public float Vertical {
        get { return (snapY) ? SnapFloat(Input.y, Global.AxisOptions.Vertical) : Input.y; }
    }

    public float MoveThreshold {
        get { return moveThreshold; }
        set { moveThreshold = Mathf.Abs(value); }
    }

    public Vector2 Direction {
        get { return new Vector2(Horizontal, Vertical); }
    }

    public float HandleRange {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public Global.AxisOptions AxisOptions {
        get { return AxisOptions; }
        set { axisOptions = value; }
    }

    public bool SnapX {
        get { return snapX; }
        set { snapX = value; }
    }

    public bool SnapY {
        get { return snapY; }
        set { snapY = value; }
    }

    public Vector2 Input {
        get { return input; }
        set { input = value; }
    }
    #endregion

    private void Awake() {
        movementArrowIndicator = GetComponent<MovementArrowIndicator>();
    }


    protected virtual void Start() {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = zero;
        fixedPosition = background.anchoredPosition;
        background.anchoredPosition = fixedPosition;
        background.gameObject.SetActive(true);
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        radius = background.sizeDelta / 2;
        Input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(Input.magnitude, Input.normalized, radius, cam);
        handle.anchoredPosition = Input * radius * handleRange;

        movementController.InitiatePlayerMovement();
        movementArrowIndicator.HideShowArrow(showArrow: true);
        movementArrowIndicator.RotateArrow(movementJoystick: this);
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        Input = zero;
        handle.anchoredPosition = zero;

        movementController.InitiatePlayerMovement();
        movementArrowIndicator.HideShowArrow(false);
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam) {
        if (magnitude > deadZone) {
            if (magnitude > 1)
                Input = normalised;
        } else
            Input = zero;
    }

    private void FormatInput() {
        if (axisOptions == Global.AxisOptions.Horizontal)
            Input = new Vector2(Input.x, 0f);
        else if (axisOptions == Global.AxisOptions.Vertical)
            Input = new Vector2(0f, Input.y);
    }

    private float SnapFloat(float value, Global.AxisOptions snapAxis) {
        if (value == 0)
            return value;

        if (axisOptions == Global.AxisOptions.Both) {
            angle = Vector2.Angle(Input, Vector2.up);
            if (snapAxis == Global.AxisOptions.Horizontal) {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            } else if (snapAxis == Global.AxisOptions.Vertical) {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        } else {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition) {
        localPoint = zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint)) {
            pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return zero;
    }
}