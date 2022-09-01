public class BTService : BTDecorator
{
    private System.Action m_ServiceMethod;

    private float m_Interval = -1.0f;
    private float m_RandomVariation;

    public BTService(float interval, float randomVariation, System.Action service, BTNode childNode) : base("Service", childNode)
    {
        this.m_ServiceMethod = service;
        this.m_Interval = interval;
        this.m_RandomVariation = randomVariation;

        this.Name = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
    }

    public BTService(float interval, System.Action service, BTNode childNode) : base("Service", childNode)
    {
        this.m_ServiceMethod = service;
        this.m_Interval = interval;
        this.m_RandomVariation = interval * 0.05f;
        this.Name = "" + (interval - m_RandomVariation) + "..." + (interval + m_RandomVariation) + "s";
    }

    public BTService(System.Action service, BTNode childNode) : base("Service", childNode)
    {
        this.m_ServiceMethod = service;
        this.Name = "every tick";
    }

    protected override void OnEnable()
    {
        if (this.m_Interval <= 0f)
        {
            this.BTTimeMenter.AddUpdateObserver(m_ServiceMethod);
            m_ServiceMethod();
        }
        else if (m_RandomVariation <= 0f)
        {
            this.BTTimeMenter.AddTimer(this.m_Interval, -1, m_ServiceMethod);
            m_ServiceMethod();
        }
        else
        {
            InvokeServiceMethodWithRandomVariation();
        }
        ChildNode.Start();
    }

    protected override void OnDisable()
    {
        ChildNode.End();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        if (this.m_Interval <= 0f)
        {
            this.BTTimeMenter.RemoveUpdateObserver(m_ServiceMethod);
        }
        else if (m_RandomVariation <= 0f)
        {
            this.BTTimeMenter.RemoveTimer(m_ServiceMethod);
        }
        else
        {
            this.BTTimeMenter.RemoveTimer(InvokeServiceMethodWithRandomVariation);
        }
        Ended(result);
    }

    private void InvokeServiceMethodWithRandomVariation()
    {
        m_ServiceMethod();
        this.BTTimeMenter.AddTimer(m_Interval, m_RandomVariation, 0, InvokeServiceMethodWithRandomVariation);
    }
}