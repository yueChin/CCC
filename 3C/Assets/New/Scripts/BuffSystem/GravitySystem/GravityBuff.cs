using System;
using UnityEngine;

public class GravityBuff :Buff<CMBody>
{
    private Type m_GroudType;
    private Type m_SkyType;
    private CMEaseMove m_GrivityEase;
    public GravityBuff(int id) : base(id)
    {
        m_GroudType = typeof(GroundState);
        m_SkyType = typeof(SkyState);
        m_GrivityEase = new CMEaseMove();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        BlackBoard bb = new BlackBoard(GameLoop.Instace.GetFixedGameMoudle<BTContent>().BtTimeMenter);
        BTNode action = new BTAction(DoGravity);
        BTNode rept = new BTRepeater(-1, action);
        RootNode = new BTRootNode(bb,rept);
        RootNode.Start();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        RootNode.Stop();
    }

    public void DoGravity()
    {
        if(BuffData1.BodyFSM.CurtState.Type() == m_SkyType)
        {
            if (m_GrivityEase.IsRunning)
            {
                m_GrivityEase.FixedUpdate();
                BuffData1.Move(m_GrivityEase.EaseVelocity);
            }
            else
            {
                m_GrivityEase.Enter(0, -0.035f, Vector3.down);
            }
        }
        else if (BuffData1.BodyFSM.CurtState.Type() == m_GroudType)
        {
            if (m_GrivityEase.IsRunning)
            {
                m_GrivityEase.Exit();
                m_GrivityEase.Power = 0;
            }
        }
    }

    public override void Tick(float timeDelta)
    {
        base.Tick(timeDelta);
    }
}