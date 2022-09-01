using UnityEngine.Assertions;

public abstract class BTNode : ILifeCycle
{
    public enum State
    {
        INACTIVE,
        ACTIVE,
        STOP_REQUESTED,
    }
    
    public string Name { get; protected set; }
    public BTRootNode RootNode { get; protected set; }
    public BTContainer Parent { get; protected set; }
    
    protected State m_CurrentState = State.INACTIVE;

    public State CurrentState => m_CurrentState;
    public bool IsStopRequested => this.m_CurrentState == State.STOP_REQUESTED;
    public bool IsActive => this.m_CurrentState == State.ACTIVE;

    public virtual BlackBoard BlackBoard => RootNode.BlackBoard;
    public virtual BTTimeMenter BTTimeMenter => RootNode.TimeMenter;

    public BTNode(string name)
    {
        Name = name;
    }

    public virtual void SetRoot(BTRootNode rootNode)
    {
        this.RootNode = rootNode;
    }

    public void SetParent(BTContainer parent)
    {
        this.Parent = parent;
    }

    public virtual void Awake()
    {
        
    }

    public virtual void Destroy()
    {
        RootNode = null;
        Parent = null;
    }
    
    public void Start()
    {
        Assert.AreEqual(this.m_CurrentState, State.INACTIVE, "can only start inactive nodes");
        this.m_CurrentState = State.ACTIVE;
        OnEnable();
    }

    public void End()
    {
        Assert.AreEqual(this.m_CurrentState, State.ACTIVE, "can only stop active nodes, tried to stop");
        this.m_CurrentState = State.STOP_REQUESTED;
        OnDisable();
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Ended(bool success)
    {
        Assert.AreNotEqual(this.m_CurrentState, State.INACTIVE, "Called 'Stopped' while in state INACTIVE, something is wrong!");
        this.m_CurrentState = State.INACTIVE;
        if (this.Parent != null)
        {
            this.Parent.ChildStopped(this, success);
        }
    }

    public virtual void ParentCompositeStopped(BTComposite composite)
    {
        DoParentCompositeStopped(composite);
    }

    /// THIS IS CALLED WHILE YOU ARE INACTIVE, IT's MEANT FOR DECORATORS TO REMOVE ANY PENDING
    /// OBSERVERS
    protected virtual void DoParentCompositeStopped(BTComposite composite)
    {
        /// be careful with this!
    }
}