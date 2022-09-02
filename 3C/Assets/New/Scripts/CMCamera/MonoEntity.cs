using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoEntity : MonoBehaviour
{
    public readonly Dictionary<int,Buff> OwnBuffDict = new Dictionary<int,Buff>();

    protected virtual void Awake()
    {
        
    }

    public bool HasBuff(int id)
    {
        return OwnBuffDict.ContainsKey(id);
    }
    

    public void AddBuff(Buff buff)
    {
        // if(buff.BuffId == 1)
        //     Debug.LogError("增加");
        OwnBuffDict.TryAdd(buff.BuffId, buff);
    }

    public bool RemoveBuff(int id)
    {
        // if(id == 1)
        //     Debug.LogError("移除");
        if (OwnBuffDict.ContainsKey(id))
        {
            OwnBuffDict.Remove(id);
            return true;
        }
        return false;
    }
}