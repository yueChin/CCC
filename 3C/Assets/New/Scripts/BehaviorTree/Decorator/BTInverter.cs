public class BTInverter : BTDecorator
{
    public BTInverter(BTNode childNode) : base("Inverter", childNode)
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
        Ended(!result);
    }
}