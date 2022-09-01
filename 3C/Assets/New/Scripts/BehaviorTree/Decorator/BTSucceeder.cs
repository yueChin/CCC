public class BTSucceeder : BTDecorator
{
    public BTSucceeder(BTNode decoratee) : base("Succeeder", decoratee)
    {
    }

    protected override void OnEnable()
    {
        ChildNode.Start();
    }

    protected override void OnDisable()
    {
        ChildNode.End();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Ended(true);
    }
}