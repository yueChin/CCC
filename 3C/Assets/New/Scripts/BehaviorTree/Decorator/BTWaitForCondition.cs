using UnityEngine.Assertions;
using System;

public class BTWaitForCondition : BTDecorator
{
    private Func<bool> m_Condition;
    private float m_CheckInterval;
    private float m_CheckVariance;

    public BTWaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, BTNode decoratee) : base("WaitForCondition", decoratee)
    {
        this.m_Condition = condition;

        this.m_CheckInterval = checkInterval;
        this.m_CheckVariance = randomVariance;

        this.Name = "" + (checkInterval - randomVariance) + "..." + (checkInterval + randomVariance) + "s";
    }

    public BTWaitForCondition(Func<bool> condition, BTNode decoratee) : base("WaitForCondition", decoratee)
    {
        this.m_Condition = condition;
        this.m_CheckInterval = 0.0f;
        this.m_CheckVariance = 0.0f;
        this.Name = "every tick";
    }

    protected override void OnEnable()
    {
        if (!m_Condition.Invoke())
        {
            BTTimeMenter.AddTimer(m_CheckInterval, m_CheckVariance, -1, CheckCondition);
        }
        else
        {
            ChildNode.Start();
        }
    }

    private void CheckCondition()
    {
        if (m_Condition.Invoke())
        {
            BTTimeMenter.RemoveTimer(CheckCondition);
            ChildNode.Start();
        }
    }

    protected override void OnDisable()
    {
        BTTimeMenter.RemoveTimer(CheckCondition);
        if (ChildNode.IsActive)
        {
            ChildNode.End();
        }
        else
        {
            Ended(false);
        }
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        Ended(result);
    }
}