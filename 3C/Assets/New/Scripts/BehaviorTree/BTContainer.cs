using UnityEngine.Assertions;

public abstract class BTContainer : BTNode
{
    private bool collapse = false;
    public bool Collapse
    {
        get => collapse;
        set => collapse = value;
    }

    public BTContainer(string name) : base(name)
    {
    }

    public void ChildStopped(BTNode child, bool succeeded)
    {
        Assert.AreNotEqual(this.m_CurrentState, State.INACTIVE, "A Child of a Container was stopped while the container was inactive.");
        this.DoChildStopped(child, succeeded);
    }

    protected abstract void DoChildStopped(BTNode child, bool succeeded);
}