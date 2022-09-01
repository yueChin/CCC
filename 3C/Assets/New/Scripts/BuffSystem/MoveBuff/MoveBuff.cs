using UnityEngine;

public class MoveBuff :Buff<CMBody>
{
    private CMEaseMove m_MoveEase;
    
    public MoveBuff(int id, BuffSystem buffSystem) : base(id, buffSystem)
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
        BTNode stop = new BTAction(Stop);
        BTNode condition = new BtObservingBlackboardCondition("kk", BTOperator.IS_NOT_EQUAL,BTStops.BOTH,stop);
        BTAction move = new BTAction(DoMove);
        BTRepeater repeater = new BTRepeater(-1,move);
        BTParallel parallel = new BTParallel(BTParallel.Policy.ALL,BTParallel.Policy.ALL, repeater,condition);
        BTWaitUntilStopped stopped = new BTWaitUntilStopped();
        BTService service = new BTService(DoMove,stopped);
        
        BlackBoard bb = new BlackBoard(BuffSystem.BtTimeMenter);
        RootNode = new BTRootNode(bb,service);
        RootNode.Start();
    }

    public override void End()
    {
        base.End();
        BuffSystem.RemoveBuff(this);
    }

    private bool IsFinish()
    {
        return !m_MoveEase.IsRunning;
    }

    private void Stop()
    {
        this.End();
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