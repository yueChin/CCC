public class BTSucceeder : BTDecorator
{
    public BTSucceeder(BTNode decoratee) : base("Succeeder", decoratee)
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
        Stopped(true);
    }
}