using System.Collections.Generic;
using UnityEngine.Assertions;

public class RootNode : BTDecorator , ILifeCycle
{
    public BlackBoard BlckBoard { get; private set; }
    public List<BTNode> NodeList = new List<BTNode>();
    public BTTimeMenter TimeMenter { get; private set; }

    public RootNode(BlackBoard bb,BTNode node ) : base("Root",node)
    {
        BlckBoard = BTContent.GetSharedBlackboard("Root");
        TimeMenter = BTContent.GetTimeMenter();
    } 
    
    public override void Awake()
    {
        base.Awake();
        NodeList = new List<BTNode>();
    }

    public override void Destroy()
    {
        base.Destroy();
        foreach (BTNode btNode in NodeList)
        {
            btNode.Destroy();
        }
        NodeList = null;
        TimeMenter = null;
    }

    protected override void DoStart()
    {
        base.DoStart();
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