using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FSM: ILifeCycle
{
    public Dictionary<int, FSMState> FsmStateDict;
    public Stack<FSMState> FsmStateStack;

    public FSMState CurtState { get; private set; }
    public string Name { get; private set; }

    public FSM()
    {
        Name = String.Empty;
    }
    
    public FSM(string name = null)
    {
        Name = name;
    }
    
    public void Awake()
    {
        FsmStateDict = new Dictionary<int, FSMState>();
        FsmStateStack = new Stack<FSMState>();
    }

    public void Destroy()
    {
        CurtState = null;
        foreach (var keyValuePair in FsmStateDict)
        {
            keyValuePair.Value.Destroy();
        }
        FsmStateDict = null;
        FsmStateStack = null;
    }
    
    public void Tick()
    {
        CurtState.Tick();
    }
    
    public void AddState(FSMState state)
    {
        int type = state.StateID;
        if (!FsmStateDict.ContainsKey(type))
        {
            FsmStateDict.Add(type,state);
        }
        else
        {
            Debug.LogError("Replace State");
            FsmStateDict[type] = state;
        }
        state.SetFSMController(this);
        state.Awake();
    }

    public void Init()
    {
        int stateId = 0;
        int lowestLayer = int.MaxValue;
        foreach (KeyValuePair<int, FSMState> keyValuePair in FsmStateDict)
        {
            if (keyValuePair.Value.Layer < lowestLayer)
            {
                lowestLayer = keyValuePair.Value.Layer;
                stateId = keyValuePair.Value.StateID;
            }
        }

        CurtState = FsmStateDict[stateId];
    }
    
    
    public void SwitchTo(int id)
    {
        if (!FsmStateDict.TryGetValue(id, out FSMState state)) 
            return;
        if (CurtState != null && CurtState.StateID == id)
            return;
        if (!CheckSwitchCondition(state)) 
            return;
        FsmStateStack.Push(state);
        CurtState = state;
    }

    private bool CheckSwitchCondition(FSMState state)
    {
        if (state.Layer >= CurtState.Layer)
        {
            if (CurtState.MaskIDs != null)
            {
                for (int i = 0; i < CurtState.MaskIDs.Length; i++)
                {
                    if (CurtState.MaskIDs[i] == state.StateID)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
    
    public void ExitCurt()
    {
        if (FsmStateStack.Count == 1)
        {
            return;
        }
        else
        {
            FsmStateStack.Pop();
            CurtState = FsmStateStack.Peek();
        }
    }
}

public class FSM<T> : FSM
{
    public T FSMData { get; private set; }

    public void SetT(T t)
    {
        FSMData = t;
    }
}