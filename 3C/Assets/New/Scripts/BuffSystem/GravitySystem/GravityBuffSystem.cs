public class GravityBuffSystem : BuffSystem
{
    public GravityBuffSystem()
    {
        BtTimeMenter = new BTTimeMenter();
    }

    ~GravityBuffSystem()
    {
        BtTimeMenter = null;
    }
    
    public override void Awake()
    {
        BtTimeMenter.Awake();
        base.Awake();
    }

    public override void Destroy()
    {
        BtTimeMenter.Destroy();
        base.Destroy();
    }

    public override void Tick(float timeDelta)
    {
        BtTimeMenter.Tick(timeDelta);
        base.Tick(timeDelta);
    }
}