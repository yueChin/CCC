using System;

public class BTObserver : BTDecorator
{
    private System.Action m_OnStart;
    private System.Action<bool> m_OnStop;

    public BTObserver(System.Action onStart, System.Action<bool> onStop, BTNode decoratee) : base("Observer", decoratee)
    {
        this.m_OnStart = onStart;
        this.m_OnStop = onStop;
    }

    protected override void DoStart()
    {
        this.m_OnStart();
        ChildNode.Start();
    }

    protected override void DoStop()
    {
        ChildNode.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        this.m_OnStop(result);
        Stopped(result);
    }
}