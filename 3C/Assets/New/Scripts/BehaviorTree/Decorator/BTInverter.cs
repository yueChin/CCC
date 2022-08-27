public class BTInverter : BTDecorator
{
    public BTInverter(BTNode decoratee) : base("Inverter", decoratee)
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
        Stopped(!result);
    }
}