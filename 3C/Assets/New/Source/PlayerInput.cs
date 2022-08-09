using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    public Controller controller;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 CameraInput { get; private set; }
    public bool JumpInput { get; private set; }
    
    public bool DashInput { get; private set; }

    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        MoveInput = moveInput;
    }

    public void OnLookEvent(InputAction.CallbackContext context)
    {
        CameraInput = context.ReadValue<Vector2>();
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
            JumpInput = true;
        }
        else if (context.canceled)
        {
            JumpInput = false;
        }
    }

    public void Update()
    {
        controller.SetMovementInput(MoveInput);
        controller.SetRotationInput(CameraInput);
        controller.SetJumpInput(JumpInput);
        controller.SetDashInput(DashInput);
    }
    

}