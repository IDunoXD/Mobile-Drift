using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    [SerializeField] WaypointManager waypointManager;
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    public Vector2 ScreenPoint;
    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }
    private void OnEnable() {
        touchPressAction.performed += TouchPressed;
    }
    private void OnDisable() {
        touchPressAction.performed -= TouchPressed;
    }
    private void TouchPressed(InputAction.CallbackContext context) {
        ScreenPoint = touchPositionAction.ReadValue<Vector2>();
        if(OnClicked != null)
            OnClicked();
    }
}
