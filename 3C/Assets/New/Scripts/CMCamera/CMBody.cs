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
}