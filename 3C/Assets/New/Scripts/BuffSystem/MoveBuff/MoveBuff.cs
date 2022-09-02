using UnityEngine;

public class MoveBuff :Buff<CMBody>
{
    private CMEaseMove m_MoveEase;
    
    public MoveBuff(int id, BuffSystem buffSystem,MonoEntity entity) : base(id, buffSystem,entity)
    {
        m_MoveEase = new CMEaseMove();
    }

    public void SetEaseEnter(float power, float speed, Vector3 direction, bool safe = false)
    {
        m_MoveEase.Enter(power, speed, direction,safe);
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        m_MoveEase.Enter(0.7f, 0.05f, Vector3.up);
        BTWaitUntilStopped stopped = new BTWaitUntilStopped();
        BTService service = new BTService(DoMove,stopped);
        BlackBoard bb = new BlackBoard(BuffSystem.BtTimeMenter);
        RootNode = new BTRootNode(bb,service);
        RootNode.Start();
    }

    public override void End()
    {
        base.End();
    }

    
    private void DoMove()
    {
        if (m_MoveEase.IsRunning)
        {
            m_MoveEase.FixedUpdate();
            BuffData1.Move(m_MoveEase.EaseVelocity);
        }
        else
        {
            this.End();
        }
    }
}