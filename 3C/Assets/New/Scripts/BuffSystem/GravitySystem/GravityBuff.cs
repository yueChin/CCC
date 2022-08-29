using UnityEngine;

public class GravityBuff :Buff<CMBody>
{
    public GravityBuff(int id) : base(id)
    {
    }
    
    public override void Awake()
    {
        base.Awake();
        BlackBoard bb = new BlackBoard(GameLoop.Instace.GetFixedGameMoudle<BTContent>().BtTimeMenter);
        BTNode node = new BTAction(DoGravity);
        RootNode = new BTRootNode(bb,node);
        RootNode.Start();
    }

    public void DoGravity()
    {
        Debug.LogError("DoGravity");
    }

    public override void Tick(float timeDelta)
    {
        base.Tick(timeDelta);
    }


}