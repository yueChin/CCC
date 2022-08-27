using System;

public class BTObserver : BTDecorator
{
    private System.Action onStart;
    private System.Action<bool> onStop;

    public BTObserver(System.Action onStart, System.Action<bool> onStop, BTNode decoratee) : base("Observer", decoratee)
    {
        this.onStart = onStart;
        this.onStop = onStop;
    }

    protected override void DoStart()
    {
        this.onStart();
        Decoratee.Start();
    }

    protected override void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        this.onStop(result);
        Stopped(result);
    }
}