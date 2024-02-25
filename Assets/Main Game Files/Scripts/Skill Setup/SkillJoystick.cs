using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    [Header("Game Objects and otherts")]
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject skillSettings;

    [Space(2)]

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
    [SerializeField] private Button buttonCancelSkill;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private SkillSetup skillSetup;

    private PlayerStatsManager playerStatsManager;
    private TargetManager targetManager;
    private SkillCommand skillCommand;
    private SkillReference skillReference;
    private SkillBaseCast skillBaseCast;
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
        playerStatsManager = generalSettings.GetComponent<PlayerStatsManager>();
        skillReference = skillSettings.GetComponent<SkillReference>();
        targetManager = skillSettings.GetComponent<TargetManager>();
        skillBaseCast = skillSettings.GetComponent<SkillBaseCast>();
        skillCommand = GetComponent<SkillCommand>();
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
        handle.gameObject.SetActive(false);
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        if (skillCommand.GetSetSkillID == -1) return;
        if (playerStatsManager.GetSetIsPlayerDead) {
            skillCommand.ResetTargetting();
            ResetSkillJoyStick();
            return;
        }
        if (skillCommand.GetSetIsCoolingDown) {
            skillCommand.GetSetMessageBoxManager.ShowMessage(currentMessage: Global.MESSAGE_COOLDOWN);
            return;
        }
        if (playerStatsManager.MP.Value < skillReference.GetSetMPConsumption(skillID: skillCommand.GetSetSkillID)) {
            skillCommand.GetSetMessageBoxManager.ShowMessage(currentMessage: Global.MESSAGE_NO_MANA);
            return;
        }
        if (skillReference.GetRequiresWeapon(skillID: skillCommand.GetSetSkillID) && !playerStatsManager.GetSetHasWeapon) {
            skillCommand.GetSetMessageBoxManager.ShowMessage(currentMessage: Global.MESSAGE_NO_WEAPON);
            return;
        }
        if (skillBaseCast.GetSetIsCastingSkill) return;
        
        skillSetup.SetPrimarySkillButton(gameObject.name.ToString());

        if (gameObject.name.ToString() == skillSetup.GetSetButtonSkillName) {
            skillReference.GetSetCancelSkill = false;
            targetManager.ClearTargetList(includeFinal: true);
            skillCommand.OnBeforeCast();
            OnDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (skillCommand.GetSetSkillID == -1) return;
        if (playerStatsManager.GetSetIsPlayerDead) {
            skillCommand.ResetTargetting();
            ResetSkillJoyStick();
            return;
        }
        if (skillCommand.GetSetIsCoolingDown) return;
        if (skillBaseCast.GetSetIsCastingSkill) return;

        if (gameObject.name.ToString() != skillSetup.GetSetButtonSkillName) {
            skillSetup.SetPrimarySkillButton(skillSetup.GetSetButtonSkillName);
        }

        if (gameObject.name == skillSetup.GetSetButtonSkillName) {
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
            radius = background.sizeDelta / 2;
            Input = (eventData.position - position) / (radius * canvas.scaleFactor);
            FormatInput();
            HandleInput(Input.magnitude, Input.normalized, radius, cam);
            handle.anchoredPosition = Input * radius * handleRange;
            handle.gameObject.SetActive(true);
            buttonCancelSkill.gameObject.SetActive(true);
            skillCommand.OnCastingSkill(skillJoystick: this);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        if (skillCommand.GetSetSkillID == -1) return;
        if (playerStatsManager.GetSetIsPlayerDead) {
            skillCommand.ResetTargetting();
            ResetSkillJoyStick();
            return;
        }
        if (skillCommand.GetSetIsCoolingDown) return;
        if (skillBaseCast.GetSetIsCastingSkill) return;

        if (gameObject.name.ToString() == skillSetup.GetSetButtonSkillName) {
            ResetSkillJoyStick();
            skillSetup.GetSetButtonSkillName = "";

            if (!targetManager.IsThereAnEnemy() && !skillReference.GetSetCancelSkill) {
                skillCommand.GetSetMessageBoxManager.ShowMessage(currentMessage: Global.MESSAGE_NO_TARGET);
            };

            if (
                !skillReference.GetSetCancelSkill && 
                targetManager.IsThereAnEnemy() &&
                !skillBaseCast.GetSetIsCastingSkill &&
                playerStatsManager.MP.Value > skillReference.GetSetMPConsumption(skillID: skillCommand.GetSetSkillID)
            ) {
                skillReference.GetSetCancelSkill = false;
                skillCommand.OnSkillCasted();
            }

            skillCommand.ResetTargetting();
        }
    }

    private void ResetSkillJoyStick() {
        Input = zero;
        handle.anchoredPosition = zero;
        handle.gameObject.SetActive(false);
        buttonCancelSkill.gameObject.SetActive(false);
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