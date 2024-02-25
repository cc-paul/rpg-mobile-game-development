using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTarget : MonoBehaviour {
    [Header("Game object and others")]
    [SerializeField] private GameObject myCamera;
    [SerializeField] private GameObject generalSettings;
    [SerializeField] private GameObject skillSettings;
    [SerializeField] private GameObject bottomPosition;
    [SerializeField] private RectTransform parentAreaTarget;
    [SerializeField] private RectTransform childAreaTarget;
    

    [Space(2)]

    [Header("Variable Declaration and Adjustment")]
    [SerializeField] private float childTargetSpeed = 60f;

    private float radius;
    private RangeAdjust rangeAdjust;
    private CapsuleCollider parentCapsuleCollider;
    private CapsuleCollider childCapsuleCollider;
    private TargetManager targetManager;

    private void Awake() {
        parentCapsuleCollider = parentAreaTarget.GetComponent<CapsuleCollider>();
        childCapsuleCollider = childAreaTarget.GetComponent<CapsuleCollider>();
        rangeAdjust = gameObject.transform.parent.gameObject.GetComponent<RangeAdjust>();
        targetManager = skillSettings.GetComponent<TargetManager>();
    }

    public void ControlTheChildTarget(SkillJoystick skillJoystick) {
        radius = rangeAdjust.GetSetRange * Global.RANGE_OFFSET;

        Vector3 joystickInput = Quaternion.Euler(0, myCamera.transform.localEulerAngles.y, 0) * new Vector3(skillJoystick.Horizontal, 0, skillJoystick.Vertical);
        Vector3 newPosition = transform.parent.position + joystickInput * radius;
        newPosition = Vector3.ClampMagnitude(newPosition - transform.parent.position, radius) + transform.parent.position;

        transform.position = newPosition;
    }
    
    public void ResizeTheCollider() {
        parentCapsuleCollider.radius = parentAreaTarget.rect.width / 2f - 0.5f;
        childCapsuleCollider.radius = childAreaTarget.rect.width / 2f;
    }

    public IEnumerator SetTargetToTheNearestEnemy(SkillJoystick skillJoystick) {
        yield return new WaitForSeconds(0.001f);

        Vector3 newPosition = targetManager.GetSetNearestTargetPosition;
        Vector3 myCurrentPosition = targetManager.GetSetPlayerPosition;
        Vector3 direction = newPosition - myCurrentPosition;
        float distance = direction.magnitude;
        radius = rangeAdjust.GetSetRange * Global.RANGE_OFFSET;

        if (distance > radius || targetManager.GetSetNearestTarget == null) {
            ControlTheChildTarget(skillJoystick: skillJoystick);
        } else {
            transform.position = newPosition;
        }
    }
}