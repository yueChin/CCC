using System;
using System.Collections.Generic;

public class FSMState :ILifeCycle
{
    public virtual int Layer { get; protected set; } = 0;
    public virtual string StateName { get; protected set; } 
    public virtual int StateID { get; protected set; }
    public virtual int[] MaskIDs { get; protected set; } = null;
    public FSM FSM { get; protected set; }
    
    public FSMState(int id,string name = null)
    {
        StateID = id;
        StateName = name;
    }

    public void SetIdMask(int[] maskIds)
    {
        MaskIDs = maskIds;
    }
    
    public void SetFSMController(FSM fsm)
    {
        this.FSM = fsm;
    }
    
    public virtual void Tick()
    {
        
    }
    
    public void Exit()
    {
        this.FSM.ExitCurt();
    }

    public virtual void Awake()
    {
    }

    public virtual void Destroy()
    {
        FSM = null;
    }
}