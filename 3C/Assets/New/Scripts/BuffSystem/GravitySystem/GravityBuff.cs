using UnityEngine;

public class GravityBuff :Buff<CMBody>
{
    public GravityBuff(int id) : base(id)
    {
    }

    public override void OnEnable()
    {
        base.OnEnable();
        BlackBoard bb = new BlackBoard(GameLoop.Instace.GetFixedGameMoudle<BTContent>().BtTimeMenter);
        BTNode action = new BTAction(DoGravity);
        BTNode rept = new BTRepeater(-1, action);
        RootNode = new BTRootNode(bb,rept);
        RootNode.Start();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        RootNode.Stop();
    }

    public void DoGravity()
    {
    }

    public override void Tick(float timeDelta)
    {
        base.Tick(timeDelta);
    }


}