using System.Collections.Generic;

public class BuffSystemManager : IGameMoudle
{
    public List<BuffSystem> BuffSystemList;
    private HashSet<BuffSystem> m_AddBuffSystemSet;
    private HashSet<BuffSystem> m_RemoveBuffSystemSet;
    private bool m_IsChange = false;
    
    public void Awake()
    {
        BuffSystemList = new List<BuffSystem>();
        m_AddBuffSystemSet = new HashSet<BuffSystem>();
        m_RemoveBuffSystemSet = new HashSet<BuffSystem>();
    }

    public void Destroy()
    {
        foreach (BuffSystem buffSystem in BuffSystemList)
        {
            buffSystem.Destroy();
        }

        BuffSystemList = null;
    }

    public void Tick(float deltaTime)
    {
        foreach (BuffSystem buffSystem in BuffSystemList)
        {
            if (m_RemoveBuffSystemSet.Contains(buffSystem))
            {
                continue;
            }
            buffSystem.Tick(deltaTime);
        }

        foreach (BuffSystem buffSystem in m_AddBuffSystemSet)
        {
            if (m_RemoveBuffSystemSet.Contains(buffSystem))
            {
                continue;
            }
            BuffSystemList.Add(buffSystem);
        }
        m_AddBuffSystemSet.Clear();
        
        foreach (BuffSystem buffSystem in m_RemoveBuffSystemSet)
        {
            BuffSystemList.Remove(buffSystem);
        }
        m_RemoveBuffSystemSet.Clear();
        
        if (m_IsChange)
        {
            BuffSystemList.Sort(SortBuffSystem);
        }
        m_IsChange = false;
    }
    
    public int SortBuffSystem(BuffSystem buff1,BuffSystem buff2)
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
    
    public void AddBuff(BuffSystem buffSystem)
    {
        if (!m_AddBuffSystemSet.Contains(buffSystem) && !BuffSystemList.Contains(buffSystem))
        {
            m_AddBuffSystemSet.Add(buffSystem);
            m_IsChange = true;
        }
    }

    public void RemoveBuff(BuffSystem buffSystem)
    {
        if (!m_RemoveBuffSystemSet.Contains(buffSystem) && BuffSystemList.Contains(buffSystem))
        {
            m_RemoveBuffSystemSet.Add(buffSystem);
            m_IsChange = true;
        }
    }
}