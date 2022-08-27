public class BTRepeater : BTDecorator
{
    private int loopCount = -1;
    private int currentLoop;

    /// <param name="loopCount">number of times to execute the decoratee. Set to -1 to repeat forever, be careful with endless loops!</param>
    /// <param name="decoratee">Decorated BTNode</param>
    public BTRepeater(int loopCount, BTNode decoratee) : base("Repeater", decoratee)
    {
        this.loopCount = loopCount;
    }

    /// <param name="decoratee">Decorated BTNode, repeated forever</param>
    public BTRepeater(BTNode decoratee) : base("Repeater", decoratee)
    {
    }

    protected override void DoStart()
    {
        if (loopCount != 0)
        {
            currentLoop = 0;
            Decoratee.Start();
        }
        else
        {
            this.Stopped(true);
        }
    }

    protected override void DoStop()
    {
        this.BTTimeMenter.RemoveTimer(restartDecoratee);

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
        if (result)
        {
            if (IsStopRequested || (loopCount > 0 && ++currentLoop >= loopCount))
            {
                Stopped(true);
            }
            else
            {
                this.BTTimeMenter.AddTimer(0, 0, restartDecoratee);
            }
        }
        else
        {
            Stopped(false);
        }
    }

    protected void restartDecoratee()
    {
        Decoratee.Start();
    }
}