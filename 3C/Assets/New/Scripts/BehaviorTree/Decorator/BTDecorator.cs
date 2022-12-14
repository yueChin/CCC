public abstract class BTDecorator : BTContainer
{
    protected BTNode ChildNode;

    public BTDecorator(string name, BTNode childNode) : base(name)
    {
        this.ChildNode = childNode;
        this.ChildNode.SetParent(this);
    }

    public override void SetRoot(BTRootNode rootNode)
    {
        base.SetRoot(rootNode);
        ChildNode.SetRoot(rootNode);
    }

    public override void ParentCompositeStopped(BTComposite composite)
    {
        base.ParentCompositeStopped(composite);
        ChildNode.ParentCompositeStopped(composite);
    }
}