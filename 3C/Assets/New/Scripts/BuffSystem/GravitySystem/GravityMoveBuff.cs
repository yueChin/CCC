using System;
using NPBehave;
using UnityEngine;

public class GravityMoveBuff :Buff<CMBody>
{
    private Type m_GroudType;
    private Type m_SkyType;
    private CMEaseMove m_GrivityEase;
    public GravityMoveBuff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
        id = 1;
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
        m_GrivityEase.Enter(0, -0.035f, Vector3.down);
        
        BTWaitUntilStopped stopped = new BTWaitUntilStopped();
        BTService service = new BTService(DoGravity, stopped);
        BlackBoard bb = new BlackBoard(BuffSystem.BtTimeMenter);
        RootNode = new BTRootNode(bb,service);
        RootNode.Start();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public void DoGravity()
    {
        if(BuffData1.BodyFSM.CurtState.Type() == m_SkyType)
        {
            //Debug.LogError(m_GrivityEase.EaseVelocity + "重力给向量");
            m_GrivityEase.FixedUpdate();
            BuffData1.Move(m_GrivityEase.EaseVelocity);
        }
        else if (BuffData1.BodyFSM.CurtState.Type() == m_GroudType)
        {
            if (m_GrivityEase.IsRunning)
            {
                m_GrivityEase.Exit();
                m_GrivityEase.Power = 0;
            }
            //Debug.LogError("重力结束移除");
            this.End();
        }
    }
}