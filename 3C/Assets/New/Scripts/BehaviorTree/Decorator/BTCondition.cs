using System;

public class BTCondition : BTObservingDecorator
{
    private Func<bool> condition;
    private float checkInterval;
    private float checkVariance;

    public BTCondition(Func<bool> condition, BTNode decoratee) : base("Condition", BTStops.NONE, decoratee)
    {
        this.condition = condition;
        this.checkInterval = 0.0f;
        this.checkVariance = 0.0f;
    }

    public BTCondition(Func<bool> condition, BTStops stopsOnChange, BTNode decoratee) : base("Condition", stopsOnChange, decoratee)
    {
        this.condition = condition;
        this.checkInterval = 0.0f;
        this.checkVariance = 0.0f;
    }

    public BTCondition(Func<bool> condition, BTStops stopsOnChange, float checkInterval, float randomVariance, BTNode decoratee) : base("Condition", stopsOnChange, decoratee)
    {
        this.condition = condition;
        this.checkInterval = checkInterval;
        this.checkVariance = randomVariance;
    }

    protected override void StartObserving()
    {
        this.RootNode.TimeMenter.AddTimer(checkInterval, checkVariance, -1, Evaluate);
    }

    protected override void StopObserving()
    {
        this.RootNode.TimeMenter.RemoveTimer(Evaluate);
    }

    protected override bool IsConditionMet()
    {
        return this.condition();
    }
}