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

    public void OnEnable()
    {
        if (controller == null)
        {
            controller = FindObjectOfType<Controller>();
        }
    }

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
        if (controller == null)
            return;
        controller.SetRotationInput(CameraInput);
        controller.SetMovementInput(MoveInput);
        controller.SetJumpInput(JumpInput);
        controller.SetDashInput(DashInput);
    }
}