public class BTRandom : BTDecorator
{
    private float m_Probability;

    public BTRandom(float probability, BTNode childNode) : base("Random", childNode)
    {
        this.m_Probability = probability;
    }

    protected override void OnEnable()
    {
        if (UnityEngine.Random.value <= this.m_Probability)
        {
            ChildNode.Start();
        }
        else
        {
            Ended(false);
        }
    }

    protected override void OnDisable()
    {
        ChildNode.End();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Ended(result);
    }
}