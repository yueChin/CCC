using UnityEngine.Assertions;

public class BTTimeMax : BTDecorator
{
    private float m_Limit = 0.0f;
    private float m_RandomVariation;
    private bool m_WaitForChildButFailOnLimitReached = false;
    private bool m_IsLimitReached = false;

    public BTTimeMax(float limit, bool waitForChildButFailOnLimitReached, BTNode decoratee) : base("TimeMax", decoratee)
    {
        this.m_Limit = limit;
        this.m_RandomVariation = limit * 0.05f;
        this.m_WaitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }

    public BTTimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, BTNode decoratee) : base("TimeMax", decoratee)
    {
        this.m_Limit = limit;
        this.m_RandomVariation = randomVariation;
        this.m_WaitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }

    protected override void DoStart()
    {
        this.m_IsLimitReached = false;
        BTTimeMenter.AddTimer(m_Limit, m_RandomVariation, 0, TimeoutReached);
        ChildNode.Start();
    }

    protected override void DoStop()
    {
        BTTimeMenter.RemoveTimer(TimeoutReached);
        if (ChildNode.IsActive)
        {
            ChildNode.Stop();
        }
        else
        {
            Stopped(false);
        }
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        BTTimeMenter.RemoveTimer(TimeoutReached);
        if (m_IsLimitReached)
        {
            Stopped(false);
        }
        else
        {
            Stopped(result);
        }
    }

    private void TimeoutReached()
    {
        if (!m_WaitForChildButFailOnLimitReached)
        {
            ChildNode.Stop();
        }
        else
        {
            m_IsLimitReached = true;
            Assert.IsTrue(ChildNode.IsActive);
        }
    }
}