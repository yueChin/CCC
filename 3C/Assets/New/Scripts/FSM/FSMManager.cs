using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : IGameMoudle
{
    public List<FSM> LiveingFSMList;
    public Dictionary<Type,FSM> FSMDict;

    public void Tick(float deltaTime)
    {
        foreach (FSM fsm in LiveingFSMList)
        {
            fsm.Tick();
        }
    }

    public void Awake()
    {
        LiveingFSMList = new List<FSM>();
        FSMDict = new Dictionary<Type, FSM>();
    }

    public void Destroy()
    {
        foreach (FSM fsm in FSMDict.Values)
        {
            fsm.Destroy();
        }
        LiveingFSMList.Clear();
        LiveingFSMList = null;
        FSMDict.Clear();
        FSMDict = null;
    }

    public T FetchFSM<T>() where T : FSM,new()
    {
        Type type = typeof(T);
        foreach (FSM fsm in LiveingFSMList)
        {
            if (fsm.GetType() == type)
            {
                return fsm as T;
            }
        }
        
        if (FSMDict.ContainsKey(type))
        {
            T fsmT = FSMDict[type] as T;
            LiveingFSMList.Add(fsmT);
            return fsmT;
        }
        else
        {
            T fsmT = new T();
            FSMDict.Add(type,fsmT);
            LiveingFSMList.Add(fsmT);
            fsmT.Awake();
            return fsmT;
        }
    }
    
    public void ShutDownFSM(FSM fsm)
    {
        for (int i = LiveingFSMList.Count - 1; i >= 0; i--)
        {
            if (fsm == LiveingFSMList[i])
            {
                LiveingFSMList.RemoveAt(i);
                break;
            }
        }
    }
}