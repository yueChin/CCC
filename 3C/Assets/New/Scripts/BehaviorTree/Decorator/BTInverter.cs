public class BTInverter : BTDecorator
{
    public BTInverter(BTNode decoratee) : base("Inverter", decoratee)
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