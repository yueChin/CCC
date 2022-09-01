public class BTWaitUntilStopped : BTTask
{
    private bool m_SucessWhenStopped;
    public BTWaitUntilStopped(bool sucessWhenStopped = false) : base("WaitUntilStopped")
    {
        this.m_SucessWhenStopped = sucessWhenStopped;
    }

    protected override void OnDisable()
    {
        this.Ended(m_SucessWhenStopped);
    }
}