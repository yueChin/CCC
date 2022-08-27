public class BTNode
{
    public enum State
    {
        INACTIVE,
        ACTIVE,
        STOP_REQUESTED,
    }
    
    public string Name { get; private set; }
    public RootNode RootNode { get; protected set; }
    public BTNode Parent { get; protected set; }
    
    protected State m_CurrentState = State.INACTIVE;

    public State CurrentState
    {
        get { return m_CurrentState; }
    }
    
    public BTNode(string name)
    {
        Name = name;
    }

    public virtual void SetRoot(RootNode rootNode)
    {
        this.RootNode = rootNode;
    }

    public void SetParent(BTNode parent)
    {
        this.Parent = parent;
    }

}