public abstract class BTDecorator : BTContainer
{
    protected BTNode Decoratee;

    public BTDecorator(string name, BTNode decoratee) : base(name)
    {
        this.Decoratee = decoratee;
        this.Decoratee.SetParent(this);
    }

    public override void SetRoot(RootNode rootNode)
    {
        base.SetRoot(rootNode);
        Decoratee.SetRoot(rootNode);
    }

    public override void ParentCompositeStopped(BTComposite composite)
    {
        base.ParentCompositeStopped(composite);
        Decoratee.ParentCompositeStopped(composite);
    }
}