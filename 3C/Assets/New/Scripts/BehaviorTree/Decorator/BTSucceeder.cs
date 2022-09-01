public class BTSucceeder : BTDecorator
{
    public BTSucceeder(BTNode childNode) : base("Succeeder", childNode)
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