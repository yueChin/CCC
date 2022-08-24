using System;
using Cinemachine.Utility;
using UnityEngine;

public class CustomBody : Body
{
    protected CustomEaseMove m_GravityMove;
    
    public float Gravity
    {
        get { return this.m_GravityMove.Power; }
    }

    protected override void Awake()
    {
        base.Awake();
        this.m_GravityMove = new CustomEaseMove(this);
    }

    protected override void FixedUpdate()
    {
        GravtyUpdate();
        base.FixedUpdate();
    }

    private void GravtyUpdate()
    {
        if (!this.IsGrounded && !this.m_GravityMove.IsRunning)
        {
            this.m_GravityMove.Enter(0, -0.035f, Vector3.down);
        }
        else if (this.IsGrounded && this.m_GravityMove.IsRunning)
        {
            this.m_GravityMove.Exit();
            this.m_GravityMove.Power = 0;
        }
        this.m_GravityMove.FixedUpdate();
    }
}