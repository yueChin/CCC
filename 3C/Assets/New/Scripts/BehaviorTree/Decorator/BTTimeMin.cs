using UnityEngine.Assertions;

public class BTTimeMin : BTDecorator
{
    private float m_Limit = 0.0f;
    private float m_RandomVariation;
    private bool m_WaitOnFailure = false;
    private bool m_IsLimitReached = false;
    private bool m_IsDecorateeDone = false;
    private bool m_IsDecorateeSuccess = false;

    public BTTimeMin(float limit, BTNode childNode) : base("TimeMin", childNode)
    {
        this.m_Limit = limit;
        this.m_RandomVariation = this.m_Limit * 0.05f;
        this.m_WaitOnFailure = false;
        Assert.IsTrue(m_Limit > 0f, "m_Limit has to be set");
    }

    public BTTimeMin(float limit, bool waitOnFailure, BTNode decoratee) : base("TimeMin", decoratee)
    {
        this.m_Limit = limit;
        this.m_RandomVariation = this.m_Limit * 0.05f;
        this.m_WaitOnFailure = waitOnFailure;
        Assert.IsTrue(m_Limit > 0f, "m_Limit has to be set");
    }

    public BTTimeMin(float limit, float randomVariation, bool waitOnFailure, BTNode decoratee) : base("TimeMin", decoratee)
    {
        this.m_Limit = limit;
        this.m_RandomVariation = randomVariation;
        this.m_WaitOnFailure = waitOnFailure;
        Assert.IsTrue(m_Limit > 0f, "m_Limit has to be set");
    }

    protected override void OnEnable()
    {
        m_IsDecorateeDone = false;
        m_IsDecorateeSuccess = false;
        m_IsLimitReached = false;
        BTTimeMenter.AddTimer(m_Limit, m_RandomVariation, 0, TimeoutReached);
        ChildNode.Start();
    }

    protected override void OnDisable()
    {
        if (ChildNode.IsActive)
        {
            BTTimeMenter.RemoveTimer(TimeoutReached);
            m_IsLimitReached = true;
            ChildNode.End();
        }
        else
        {
            BTTimeMenter.RemoveTimer(TimeoutReached);
            Ended(false);
        }
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        m_IsDecorateeDone = true;
        m_IsDecorateeSuccess = result;
        if (m_IsLimitReached || (!result && !m_WaitOnFailure))
        {
            BTTimeMenter.RemoveTimer(TimeoutReached);
            Ended(m_IsDecorateeSuccess);
        }
        else
        {
            Assert.IsTrue(BTTimeMenter.HasTimer(TimeoutReached));
        }
    }

    private void TimeoutReached()
    {
        m_IsLimitReached = true;
        if (m_IsDecorateeDone)
        {
            Ended(m_IsDecorateeSuccess);
        }
        else
        {
            Assert.IsTrue(ChildNode.IsActive);
        }
    }
}