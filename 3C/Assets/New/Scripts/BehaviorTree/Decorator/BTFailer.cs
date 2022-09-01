public class BTFailer : BTDecorator
{
    public BTFailer(BTNode childNode) : base("Failer", childNode)
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
        Ended(false);
    }
}