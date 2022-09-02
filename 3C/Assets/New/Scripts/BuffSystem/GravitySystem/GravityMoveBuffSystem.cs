public class GravityMoveBuffSystem : BuffSystem
{
    public GravityMoveBuffSystem()
    {
        BtTimeMenter = GameLoop.Instace.GetFixedGameMoudle<BuffSystemManager>().BtContent.BtTimeMenter; 
    }

    ~GravityMoveBuffSystem()
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