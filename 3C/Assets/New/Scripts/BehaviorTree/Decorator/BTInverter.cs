public class BTInverter : BTDecorator
{
    public BTInverter(BTNode decoratee) : base("Inverter", decoratee)
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
        Stopped(!result);
    }
}