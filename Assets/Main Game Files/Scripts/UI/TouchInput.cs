using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [Header("Variable Declaration and Adjustment")]
    [SerializeField] private int PointerId;

    [Space(2)]

    [Header("Components")]
    [SerializeField] private CameraControl cameraControl;


    public void OnPointerDown(PointerEventData eventData) {
        PointerId = eventData.pointerId;
        cameraControl.GetSetIsPressed = true;
        cameraControl.GetSetPointerID = PointerId;
    }

    public void OnPointerUp(PointerEventData eventData) {
        cameraControl.GetSetIsPressed = false;
    }
}