public class BTService : BTDecorator
{
    private System.Action serviceMethod;

    private float interval = -1.0f;
    private float randomVariation;

    public BTService(float interval, float randomVariation, System.Action service, BTNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
        this.interval = interval;
        this.randomVariation = randomVariation;

        this.Name = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
    }

    public BTService(float interval, System.Action service, BTNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
        this.interval = interval;
        this.randomVariation = interval * 0.05f;
        this.Name = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
    }

    public BTService(System.Action service, BTNode decoratee) : base("Service", decoratee)
    {
        this.serviceMethod = service;
        this.Name = "every tick";
    }

    protected override void DoStart()
    {
        if (this.interval <= 0f)
        {
            this.BTTimeMenter.AddUpdateObserver(serviceMethod);
            serviceMethod();
        }
        else if (randomVariation <= 0f)
        {
            this.BTTimeMenter.AddTimer(this.interval, -1, serviceMethod);
            serviceMethod();
        }
        else
        {
            InvokeServiceMethodWithRandomVariation();
        }
        Decoratee.Start();
    }

    protected override void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        if (this.interval <= 0f)
        {
            this.BTTimeMenter.RemoveUpdateObserver(serviceMethod);
        }
        else if (randomVariation <= 0f)
        {
            this.BTTimeMenter.RemoveTimer(serviceMethod);
        }
        else
        {
            this.BTTimeMenter.RemoveTimer(InvokeServiceMethodWithRandomVariation);
        }
        Stopped(result);
    }

    private void InvokeServiceMethodWithRandomVariation()
    {
        serviceMethod();
        this.BTTimeMenter.AddTimer(interval, randomVariation, 0, InvokeServiceMethodWithRandomVariation);
    }
}