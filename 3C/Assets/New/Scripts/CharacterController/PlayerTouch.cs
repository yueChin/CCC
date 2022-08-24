using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTouch : MonoBehaviour
{
    public bool FirstFingerHold { get; private set; }
    public bool SecendFingerHold { get; private set; }
    public Vector2 FirstFingerInput { get; private set; }
    
    public Vector2 SecondFingerInput { get; private set; }

    public void OnLeftMouseInput(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            FirstFingerHold = true;
        }
        else if (context.canceled)
        {
            FirstFingerHold = false;
        }
    }
    
    public void OnRightMouseInput(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            SecendFingerHold = true;
        }
        else if (context.canceled)
        {
            SecendFingerHold = false;
        }
    }
    
    public void OnFirstFingerMoveEvent(InputAction.CallbackContext context)
    {
        FirstFingerInput = context.ReadValue<Vector2>();
    }
    
    public void OnSecondFingerMoveEvent(InputAction.CallbackContext context)
    {
        SecondFingerInput = context.ReadValue<Vector2>();
    }
}