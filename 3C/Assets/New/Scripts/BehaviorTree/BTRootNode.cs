using System.Collections.Generic;
using UnityEngine.Assertions;

public class BTRootNode : BTDecorator , ILifeCycle
{
    public BlackBoard BlckBoard { get; private set; }
    public BTNode BTEntryNode;
    public BTTimeMenter TimeMenter { get; private set; }

    public BTRootNode(BlackBoard bb,BTNode node ) : base("Root",node)
    {
        BTEntryNode = node;
        BlckBoard = bb;
        TimeMenter = GameLoop.Instace.GetGameMoudle<BTContent>().BtTimeMenter;
        ChildNode.SetRoot(this);
    } 
    
    public override void Awake()
    {
        base.Awake();
    }

    public override void Destroy()
    {
        base.Destroy();
        
        BTEntryNode = null;
        TimeMenter = null;
    }

    protected override void DoStart()
    {
        base.DoStart();
        BTEntryNode.Start();
        BlckBoard.Enable();
    }

    protected override void DoChildStopped(BTNode child, bool succeeded)
    {
        if (!IsStopRequested)
        {
            this.TimeMenter.AddTimer(0, 0, this.Start);
        }
        else
        {
            this.BlckBoard.Disable();
            Stopped(succeeded);
        }
    }
}