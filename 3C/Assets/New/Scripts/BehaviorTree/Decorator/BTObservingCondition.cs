using System;

public class BTObservingCondition : BTObservingDecorator
{
    private Func<bool> m_Condition;
    private float m_CheckInterval;
    private float m_CheckVariance;

    public BTObservingCondition(Func<bool> condition, BTNode childNode) : base("Condition", BTStops.NONE, childNode)
    {
        this.m_Condition = condition;
        this.m_CheckInterval = 0.0f;
        this.m_CheckVariance = 0.0f;
    }

    public BTObservingCondition(Func<bool> condition, BTStops stopsOnChange, BTNode childNode) : base("Condition", stopsOnChange, childNode)
    {
        this.m_Condition = condition;
        this.m_CheckInterval = 0.0f;
        this.m_CheckVariance = 0.0f;
    }

    public BTObservingCondition(Func<bool> condition, BTStops stopsOnChange, float checkInterval, float randomVariance, BTNode childNode) : base("Condition", stopsOnChange, childNode)
    {
        this.m_Condition = condition;
        this.m_CheckInterval = checkInterval;
        this.m_CheckVariance = randomVariance;
    }

    protected override void StartObserving()
    {
        this.RootNode.TimeMenter.AddTimer(m_CheckInterval, m_CheckVariance, -1, Evaluate);
    }

    protected override void StopObserving()
    {
        this.RootNode.TimeMenter.RemoveTimer(Evaluate);
    }

    protected override bool IsConditionMet()
    {
        return this.m_Condition();
    }
}