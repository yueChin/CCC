public class BTSucceeder : BTDecorator
{
    public BTSucceeder(BTNode decoratee) : base("Succeeder", decoratee)
    {
    }

    protected override void DoStart()
    {
        Decoratee.Start();
    }

    protected override void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Stopped(true);
    }
}