public class BTFailer : BTDecorator
{
    public BTFailer(BTNode decoratee) : base("Failer", decoratee)
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
        Stopped(false);
    }
}