public class BTRepeater : BTDecorator
{
    private int m_LoopCount = -1;
    private int m_CurrentLoop;

    /// <param name="loopCount">number of times to execute the decoratee. Set to -1 to repeat forever, be careful with endless loops!</param>
    /// <param name="childNode">Decorated BTNode</param>
    public BTRepeater(int loopCount, BTNode childNode) : base("Repeater", childNode)
    {
        this.m_LoopCount = loopCount;
    }

    /// <param name="childNode">Decorated BTNode, repeated forever</param>
    public BTRepeater(BTNode childNode) : base("Repeater", childNode)
    {
    }

    protected override void OnEnable()
    {
        if (m_LoopCount != 0)
        {
            m_CurrentLoop = 0;
            ChildNode.Start();
        }
        else
        {
            this.Ended(true);
        }
    }

    protected override void OnDisable()
    {
        this.BTTimeMenter.RemoveTimer(RestartDecoratee);

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
        if (result)
        {
            if (IsStopRequested || (m_LoopCount > 0 && ++m_CurrentLoop >= m_LoopCount))
            {
                Ended(true);
            }
            else
            {
                this.BTTimeMenter.AddTimer(0, 0, RestartDecoratee);
            }
        }
        else
        {
            Ended(false);
        }
    }

    protected void RestartDecoratee()
    {
        ChildNode.Start();
    }
}