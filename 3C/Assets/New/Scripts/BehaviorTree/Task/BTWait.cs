public class BTWait : BTTask
{
    private System.Func<float> m_Function = null;
    private string m_BlackBoardKey = null;
    private float m_Seconds = -1f;
    private float m_RandomVariance;

    public float RandomVariance
    {
        get
        {
            return m_RandomVariance;
        }
        set
        {
            m_RandomVariance = value;
        }
    }

    public BTWait(float seconds, float randomVariance) : base("Wait")
    {
        UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
        this.m_Seconds = seconds;
        this.m_RandomVariance = randomVariance;
    }

    public BTWait(float seconds) : base("Wait")
    {
        this.m_Seconds = seconds;
        this.m_RandomVariance = this.m_Seconds * 0.05f;
    }

    public BTWait(string blackboardKey, float randomVariance = 0f) : base("Wait")
    {
        this.m_BlackBoardKey = blackboardKey;
        this.m_RandomVariance = randomVariance;
    }

    public BTWait(System.Func<float> function, float randomVariance = 0f) : base("Wait")
    {
        this.m_Function = function;
        this.m_RandomVariance = randomVariance;
    }

    protected override void OnEnable()
    {
        float seconds = this.m_Seconds;
        if (seconds < 0)
        {
            if (this.m_BlackBoardKey != null)
            {
                seconds = BlackBoard.Get<float>(this.m_BlackBoardKey);
            }
            else if (this.m_Function != null)
            {
                seconds = this.m_Function();
            }
        }
        if (seconds < 0)
        {
            seconds = 0;
        }

        if (m_RandomVariance >= 0f)
        {
            BTTimeMenter.AddTimer(seconds, m_RandomVariance, 0, OnTimer);
        }
        else
        {
            BTTimeMenter.AddTimer(seconds, 0, OnTimer);
        }
    }

    protected override void OnDisable()
    {
        BTTimeMenter.RemoveTimer(OnTimer);
        this.Ended(false);
    }

    private void OnTimer()
    {
        BTTimeMenter.RemoveTimer(OnTimer);
        this.Ended(true);
    }
}