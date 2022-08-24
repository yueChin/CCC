using System;
using UnityEngine;

public class CMEaseMove : Gear
{
    private Ease m_Ease;

    public Action OnExit;
    public Vector3 Direction;
    public bool Safe;

    public Vector3 EaseVelocity
    {
        get
        {
            if (this.m_Ease.IsRunning)
            {
                return this.GetForward(1);
            }
            return Vector3.zero;
        }
    }
    
    public float Power
    {
        get { return this.m_Ease.Current; }
        set { this.m_Ease.Current = value; }
    }

    public float EaseDelta
    {
        get { return this.m_Ease.EaseDelta; }
        set { this.m_Ease.EaseDelta = value; }
    }

    public float Progress
    {
        get { return this.m_Ease.Progress; }
    }

    public CMEaseMove()
    {
        this.m_Ease = new Ease();
    }

    public void FixedUpdate(float rate = 1)
    {
        if (!this.IsRunning)
        {
            return;
        }

        this.m_Ease.Update(rate);

        if (!this.m_Ease.IsRunning)
        {
            this.Exit();
        }
    }

    public void Enter(float power, float speed, Vector3 direction, bool safe = false, Action OnExit = null)
    {
        base.Enter();

        this.m_Ease.Enter(power, 0, speed);
        this.Direction = direction.normalized;
        this.Safe = safe;
        this.OnExit = OnExit == null ? this.OnExit : OnExit;
    }

    public override void Exit()
    {
        base.Exit();

        if (this.OnExit != null)
        {
            this.OnExit();
        }
    }

    public void Exit(bool slient)
    {
        base.Exit();

        if (!slient && this.OnExit != null)
        {
            this.OnExit();
        }
    }

    private Vector3 GetForward(float rate)
    {
        return this.Direction * this.m_Ease.Current * rate;
    }
}