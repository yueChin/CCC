using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    public Vector2 WASDInput { get; private set; }
    public bool WASDHold { get; private set; }
    public Vector2 MouseMoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool LeftMouseInput { get; private set; }
    public bool RightMouseInput { get; private set; }
    
    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        WASDInput = moveInput;
    }

    public void OnLookEvent(InputAction.CallbackContext context)
    {
        MouseMoveInput = context.ReadValue<Vector2>();
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            JumpInput = true;
        }
        else if (context.canceled)
        {
            JumpInput = false;
        }
    }
    
    public void OnDashEvent(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            DashInput = true;
        }
        else if (context.canceled)
        {
            DashInput = false;
        }
    }

    public void OnWASDHoldEvent(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            WASDHold = true;
        }
        else if (context.canceled)
        {
            WASDHold = false;
        }
    }
    
    public void OnLeftMouseInput(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            LeftMouseInput = true;
        }
        else if (context.canceled)
        {
            LeftMouseInput = false;
        }
    }
    
    public void OnRightMouseInput(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            RightMouseInput = true;
        }
        else if (context.canceled)
        {
            RightMouseInput = false;
        }
    }
}