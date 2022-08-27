using UnityEngine.Assertions;

public class BTTimeMax : BTDecorator
{
    private float limit = 0.0f;
    private float randomVariation;
    private bool waitForChildButFailOnLimitReached = false;
    private bool isLimitReached = false;

    public BTTimeMax(float limit, bool waitForChildButFailOnLimitReached, BTNode decoratee) : base("TimeMax", decoratee)
    {
        this.limit = limit;
        this.randomVariation = limit * 0.05f;
        this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }

    public BTTimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, BTNode decoratee) : base("TimeMax", decoratee)
    {
        this.limit = limit;
        this.randomVariation = randomVariation;
        this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
        Assert.IsTrue(limit > 0f, "limit has to be set");
    }

    protected override void DoStart()
    {
        this.isLimitReached = false;
        BTTimeMenter.AddTimer(limit, randomVariation, 0, TimeoutReached);
        Decoratee.Start();
    }

    protected override void DoStop()
    {
        BTTimeMenter.RemoveTimer(TimeoutReached);
        if (Decoratee.IsActive)
        {
            Decoratee.Stop();
        }
        else
        {
            Stopped(false);
        }
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        BTTimeMenter.RemoveTimer(TimeoutReached);
        if (isLimitReached)
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
        if (!waitForChildButFailOnLimitReached)
        {
            Decoratee.Stop();
        }
        else
        {
            isLimitReached = true;
            Assert.IsTrue(Decoratee.IsActive);
        }
    }
}