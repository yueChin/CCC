using System;
using System.Collections.Generic;

public class BuffSystem
{
    public int Priority;
    public Dictionary<int, Buff> AllBuffDict;
    private Dictionary<int, Buff> m_StartedBuffDict;
    private Dictionary<int, Buff> m_EndedBuffDict;
    private List<Buff> m_RunBuffList;
    private HashSet<Buff> m_AddBuffSet;
    private HashSet<Buff> m_RemoveBuffSet;
    private HashSet<Buff> m_EnableBuffSet;
    private HashSet<Buff> m_DisableBuffList;
    private bool m_IsChange = false;

    public BuffSystem()
    {
        
    }
    
    public BuffSystem(int priority = 0)
    {
        Priority = priority;
    }

    public void SetPriority(int priority)
    {
        Priority = priority;
    }
    
    public void Awake()
    {
        AllBuffDict = new Dictionary<int, Buff>();
        m_StartedBuffDict = new Dictionary<int, Buff>();
        m_EndedBuffDict = new Dictionary<int, Buff>();
        m_RunBuffList = new List<Buff>();
        m_AddBuffSet = new HashSet<Buff>();
        m_RemoveBuffSet = new HashSet<Buff>();
        m_EnableBuffSet = new HashSet<Buff>();
        m_DisableBuffList = new HashSet<Buff>();
    }

    public void Destroy()
    {
        foreach (Buff buff in AllBuffDict.Values)
        {
            buff.Destroy();
        }
        AllBuffDict.Clear();
        m_StartedBuffDict.Clear();
        m_EndedBuffDict.Clear();
        m_AddBuffSet.Clear();
        m_RemoveBuffSet.Clear();
        m_RunBuffList.Clear();
        m_EnableBuffSet.Clear();
        m_DisableBuffList.Clear();
        
        AllBuffDict = null;
        m_AddBuffSet = null;
        m_StartedBuffDict = null;
        m_EndedBuffDict = null;
        m_RemoveBuffSet = null;
        m_RunBuffList = null;
        m_EnableBuffSet = null;
        m_DisableBuffList = null;
    }

    public void Tick(float timeDelta)
    {
        foreach (Buff buff in m_RunBuffList)
        {
            if(m_RemoveBuffSet.Contains(buff))
                continue;
            if(m_DisableBuffList.Contains(buff))
                continue;
            buff.Tick(timeDelta);
        }

        foreach (Buff buff in m_AddBuffSet)
        {
            if(m_RemoveBuffSet.Contains(buff))
                continue;
            if(m_DisableBuffList.Contains(buff))
                continue;
            if (!AllBuffDict.ContainsKey(buff.BuffId))
            {
                AllBuffDict.Add(buff.BuffId, buff);
            }
            buff.Awake();
            m_EnableBuffSet.Add(buff);
        }
        m_AddBuffSet.Clear();
        
        foreach (Buff buff in m_EnableBuffSet)
        {
            if(m_RemoveBuffSet.Contains(buff))
                continue;
            if(m_DisableBuffList.Contains(buff))
                continue;
            if (!m_StartedBuffDict.ContainsKey(buff.BuffId))
            {
                buff.Start();
                m_StartedBuffDict.Add(buff.BuffId, buff);
            }
            buff.OnEnable();
            m_RunBuffList.Add(buff);
        }
        m_EnableBuffSet.Clear();

        foreach (Buff buff in m_DisableBuffList)
        {
            if(m_RemoveBuffSet.Contains(buff))
                continue;
            if (!m_EndedBuffDict.ContainsKey(buff.BuffId))
            {
                buff.End();
                m_EndedBuffDict.Add(buff.BuffId,buff);
            }
            buff.OnDisable();
            m_RunBuffList.Remove(buff);
        }
        m_DisableBuffList.Clear();
        
        foreach (Buff buff in m_RemoveBuffSet)
        {
            m_RunBuffList.Remove(buff);
            AllBuffDict.Remove(buff.BuffId);
            m_StartedBuffDict.Remove(buff.BuffId);
            m_EndedBuffDict.Remove(buff.BuffId);
            buff.Destroy();
        }
        m_RemoveBuffSet.Clear();
        
        if (m_IsChange)
            m_RunBuffList.Sort(SortBuff);
        m_IsChange = false;
    }

    public int SortBuff(Buff buff1,Buff buff2)
    {
        if (buff1.Priority > buff2.Priority)
        {
            return 1;
        }
        else if(buff1.Priority < buff2.Priority)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    
    public void AddBuff(Buff buff)
    {
        if (AllBuffDict.TryGetValue(buff.BuffId, out Buff outBuff))
        {
            //TODO repeat 
        }
        else
        {
            m_AddBuffSet.Add(buff);
        }
        m_IsChange = true;
    }

    public void RemoveBuff(Buff buff)
    {
        if (!m_RemoveBuffSet.Contains(buff) && m_RunBuffList.Contains(buff))
        {
            m_IsChange = true;
            m_RemoveBuffSet.Add(buff);
        }
    }

    public void EnableBuff(int id)
    {
        if (AllBuffDict.TryGetValue(id, out Buff buff) && !m_RunBuffList.Contains(buff))
        {
            m_IsChange = true;
            m_EnableBuffSet.Add(buff);
        }
    }

    public void DisableBuff(int id)
    {
        if (AllBuffDict.TryGetValue(id, out Buff buff) && m_RunBuffList.Contains(buff))
        {
            m_IsChange = true;
            m_DisableBuffList.Add(buff);
        }
    }
}