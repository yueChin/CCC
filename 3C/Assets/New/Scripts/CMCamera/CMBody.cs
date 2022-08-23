using System;
using Cinemachine.Utility;
using UnityEngine;

public class CMBody : Body
{
    public float VelocityDamping = 0.5f;

    private Vector3 m_Velocity;

    // protected override void FixedUpdate()
    // {
    //     //base.FixedUpdate();
    // }

    // protected override void Update()
    // {
    //     float dt = Time.deltaTime;
    //     Vector3 deltaVel = m_MoveInput - m_Velocity;
    //     m_Velocity += Damper.Damp(deltaVel, VelocityDamping, dt);
    //     transform.position += m_Velocity * dt;
    // }

    public override void Move(Vector3 velocityInput)
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
}