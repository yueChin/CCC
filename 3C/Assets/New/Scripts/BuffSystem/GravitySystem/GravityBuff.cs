using System;
using NPBehave;
using UnityEngine;

public class GravityBuff :Buff<CMBody>
{
    private Type m_GroudType;
    private Type m_SkyType;
    private CMEaseMove m_GrivityEase;
    public GravityBuff(int id,BuffSystem buffSystem) : base(id,buffSystem)
    {
        m_GroudType = typeof(GroundState);
        m_SkyType = typeof(SkyState);
        m_GrivityEase = new CMEaseMove();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // BTNode groundAction = new BTAction(DoGround);
        // BTNode rept1 = new BTRepeater(-1, groundAction);
        // BTNode skyAction = new BTAction(DoSky);
        // BTNode rept2 = new BTRepeater(-1, skyAction);
        // BTObservingCondition bc1 = new BTObservingCondition(IsGround,BTStops.BOTH,rept1);
        // BTObservingCondition bc2 = new BTObservingCondition(IsSky,BTStops.BOTH,rept2);
        
        BlackBoard bb = new BlackBoard(BuffSystem.BtTimeMenter);
        BTNode gravity = new BTAction(DoGravity);
        BTNode rept = new BTRepeater(-1, gravity);
        RootNode = new BTRootNode(bb,rept);
        RootNode.Start();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    private bool IsGround()
    {
        return BuffData1.BodyFSM.CurtState.Type() == m_GroudType;
    }

    private bool IsSky()
    {
        return BuffData1.BodyFSM.CurtState.Type() == m_SkyType;
    }
    
    public void DoSky()
    {
        
    }

    private void DoGround()
    {
        
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