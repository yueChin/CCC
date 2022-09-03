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
            //Debug.LogError(m_MoveEase.EaseVelocity + " 移动向量");
            m_MoveEase.FixedUpdate();
            //Debug.LogError("跳跃给移动向量" + m_MoveEase.EaseVelocity);
            BuffData1.Move(m_MoveEase.EaseVelocity);
        }
        else
        {
            this.End();
        }
    }
}