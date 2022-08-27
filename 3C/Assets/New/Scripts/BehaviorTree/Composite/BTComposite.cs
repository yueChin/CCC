using UnityEngine.Assertions;

public abstract class BTComposite : BTContainer
{
    protected BTNode[] Children;

    public BTComposite(string name, BTNode[] children) : base(name)
    {
        this.Children = children;
        Assert.IsTrue(children.Length > 0, "Composite nodes (Selector, Sequence, Parallel) need at least one child!");

        foreach (BTNode node in Children)
        {
            node.SetParent(this);
        }
    }

    public override void SetRoot(BTRootNode rootNode)
    {
        base.SetRoot(rootNode);

        foreach (BTNode node in Children)
        {
            node.SetRoot(rootNode);
        }
    }

    protected override void Stopped(bool success)
    {
        foreach (BTNode child in Children)
        {
            child.ParentCompositeStopped(this);
        }
        base.Stopped(success);
    }

    public abstract void StopLowerPriorityChildrenForChild(BTNode child, bool immediateRestart);
}