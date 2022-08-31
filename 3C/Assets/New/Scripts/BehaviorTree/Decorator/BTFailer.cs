public class BTFailer : BTDecorator
{
    public BTFailer(BTNode decoratee) : base("Failer", decoratee)
    {
    }

    protected override void DoStart()
    {
        ChildNode.Start();
    }

    protected override void DoStop()
    {
        ChildNode.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Stopped(false);
    }
}