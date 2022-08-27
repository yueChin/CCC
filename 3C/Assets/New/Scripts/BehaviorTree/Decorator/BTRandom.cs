public class BTRandom : BTDecorator
{
    private float probability;

    public BTRandom(float probability, BTNode decoratee) : base("Random", decoratee)
    {
        this.probability = probability;
    }

    protected override void DoStart()
    {
        if (UnityEngine.Random.value <= this.probability)
        {
            Decoratee.Start();
        }
        else
        {
            Stopped(false);
        }
    }

    protected override void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Stopped(result);
    }
}