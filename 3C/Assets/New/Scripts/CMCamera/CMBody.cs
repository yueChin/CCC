using System;
using Cinemachine.Utility;
using UnityEngine;

public class CMBody : Body
{
    public float VelocityDamping = 0.5f;

    private Vector3 m_Velocity;

    private WorldRule m_WorldRule;

    protected override void Awake()
    {
        base.Awake();
        if (m_WorldRule == null)
        {
            m_WorldRule = FindObjectOfType<WorldRule>();
        }

        if (m_WorldRule != null)
        {
            m_WorldRule.SetActiveBody(this);
        }
    }

    protected override void FixedUpdate()
    {
        GravtyUpdate();
        base.FixedUpdate();
    }

    private void GravtyUpdate()
    {
        if (!this.IsGrounded )
        {
            m_WorldRule.ActiveBody(this);
        }
        else if (this.IsGrounded)
        {
            m_WorldRule.NegtiveBody(this);
        }
        Move(m_WorldRule.GetGravity(this));
    }

    public void InputRotate(Vector3 forward,bool isPlayer)
    {
        if (m_MoveInput.sqrMagnitude > 0.01f)
        {
            float fdt = Time.fixedDeltaTime;
            Quaternion qA = transform.rotation;
            Quaternion qB = Quaternion.LookRotation((isPlayer && Vector3.Dot(forward, m_MoveInput) < 0) ? -m_MoveInput : m_MoveInput);
            transform.rotation = Quaternion.Slerp(qA, qB, Damper.Damp(1, VelocityDamping, fdt));
        }
    }
    
    public void InputMove(Vector3 velocityInput)
    {
        float fdt = Time.fixedDeltaTime;
        Vector3 deltaVel = velocityInput - m_Velocity;
        m_Velocity += Damper.Damp(deltaVel, VelocityDamping, fdt);
        this.m_PhysicVelocity += m_Velocity * fdt;
        bool hasMovementInput = velocityInput.sqrMagnitude > 0.0f;

        if (m_HasMoveInput && !hasMovementInput)
        {
            m_LastMoveInput = m_MoveInput;
        }

        m_MoveInput = velocityInput;
        m_HasMoveInput = hasMovementInput;
    }
    
    public override void Move(Vector3 velocityDelta)
    {
        this.m_PhysicVelocity += velocityDelta;
    }

    public override void SetTargetRotation()
    {
        Vector3 movementInput = m_MoveInput;
        if (movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        m_TargetHorizontalSpeedp = movementInput.magnitude * movementSettings.MaxHorizontalSpeed;
        float acceleration = m_HasMoveInput ? movementSettings.Acceleration : movementSettings.Decceleration;

        m_HorizontalSpeed = Mathf.MoveTowards(m_HorizontalSpeed, m_TargetHorizontalSpeedp, acceleration * Time.deltaTime);

        Vector3 moveDir = m_HasMoveInput ? m_MoveInput : m_LastMoveInput;
        if (moveDir.sqrMagnitude > 1f)
        {
            moveDir.Normalize();
        }
        //Debug.LogError(moveDir);
        // if (moveDir.Equals(Vector3.zero) || moveDir.sqrMagnitude < 0.01)
        // {
        //     return;
        // }

        Vector3 horizontalMovement = moveDir.SetY(0.0f); //+ this.velocity.y * Vector3.up;
        if (horizontalMovement.sqrMagnitude < 0.01f)
            return;

        float rotationSpeed = Mathf.Lerp(rotationSettings.MaxRotationSpeed, rotationSettings.MinRotationSpeed, m_HorizontalSpeed / m_TargetHorizontalSpeedp);

        Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
        m_Transform.rotation = Quaternion.RotateTowards(m_Transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}