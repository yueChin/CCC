public class BTRandom : BTDecorator
{
    private float m_Probability;

    public BTRandom(float probability, BTNode decoratee) : base("Random", decoratee)
    {
        this.m_Probability = probability;
    }

    protected override void DoStart()
    {
        if (UnityEngine.Random.value <= this.m_Probability)
        {
            ChildNode.Start();
        }
        else
        {
            Stopped(false);
        }
    }

    protected override void DoStop()
    {
        ChildNode.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Stopped(result);
    }
}