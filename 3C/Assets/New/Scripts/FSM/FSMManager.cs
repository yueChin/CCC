using System.Collections.Generic;

public class FSMManager : IGameMoudle,ILifeCycle
{
    public List<FSM> LiveingFSMList;

    public void Tick()
    {
        foreach (FSM fsm in LiveingFSMList)
        {
            fsm.Tick();
        }
    }

    public void Awake()
    {
        LiveingFSMList = new List<FSM>();
    }

    public void Destroy()
    {
        foreach (FSM fsm in LiveingFSMList)
        {
            fsm.Destroy();
        }
        LiveingFSMList.Clear();
        LiveingFSMList = null;
    }

    public T FetchFSM<T>() where T :FSM,new()
    {
        T t = new T();
        LiveingFSMList.Add(t);
        t.Awake();
        return t;
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