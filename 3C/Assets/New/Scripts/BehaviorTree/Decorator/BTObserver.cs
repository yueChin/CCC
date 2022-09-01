using System;

public class BTObserver : BTDecorator
{
    private System.Action m_OnStart;
    private System.Action<bool> m_OnStop;

    public BTObserver(System.Action onStart, System.Action<bool> onStop, BTNode childNode) : base("Observer", childNode)
    {
        this.m_OnStart = onStart;
        this.m_OnStop = onStop;
    }

    protected override void OnEnable()
    {
        this.m_OnStart();
        ChildNode.Start();
    }

    protected override void OnDisable()
    {
        ChildNode.End();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        this.m_OnStop(result);
        Ended(result);
    }
}