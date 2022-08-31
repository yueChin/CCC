public class BTRepeater : BTDecorator
{
    private int m_LoopCount = -1;
    private int m_CurrentLoop;

    /// <param name="loopCount">number of times to execute the decoratee. Set to -1 to repeat forever, be careful with endless loops!</param>
    /// <param name="decoratee">Decorated BTNode</param>
    public BTRepeater(int loopCount, BTNode decoratee) : base("Repeater", decoratee)
    {
        this.m_LoopCount = loopCount;
    }

    /// <param name="decoratee">Decorated BTNode, repeated forever</param>
    public BTRepeater(BTNode decoratee) : base("Repeater", decoratee)
    {
    }

    protected override void DoStart()
    {
        if (m_LoopCount != 0)
        {
            m_CurrentLoop = 0;
            ChildNode.Start();
        }
        else
        {
            this.Stopped(true);
        }
    }

    protected override void DoStop()
    {
        this.BTTimeMenter.RemoveTimer(RestartDecoratee);

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
        if (result)
        {
            if (IsStopRequested || (m_LoopCount > 0 && ++m_CurrentLoop >= m_LoopCount))
            {
                Stopped(true);
            }
            else
            {
                this.BTTimeMenter.AddTimer(0, 0, RestartDecoratee);
            }
        }
        else
        {
            Stopped(false);
        }
    }

    protected void RestartDecoratee()
    {
        ChildNode.Start();
    }
}