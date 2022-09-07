
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldRule : IGameMoudle
{
    public Dictionary<IWorldBody, CMEaseMove> GravityDict = new Dictionary<IWorldBody, CMEaseMove>();
    public List<KeyValuePair<IWorldBody,CMEaseMove>> GravityList = new List<KeyValuePair<IWorldBody,CMEaseMove>>();
    
    public void Awake()
    {
        GravityDict = new Dictionary<IWorldBody, CMEaseMove>();
        GravityList = new List<KeyValuePair<IWorldBody,CMEaseMove>>();
    }

    public void Destroy()
    {
        GravityDict.Clear();
        GravityList.Clear();
        GravityDict = null;
        GravityList = null;
    }

    public void Tick(float delta)
    {
        FixedUpdate();
    }
    
    public void SetActiveBody(IWorldBody body)
    {
        if (!GravityDict.ContainsKey(body))
        {
            GravityDict.Add(body,new CMEaseMove());
        }
    }

    public void ActiveBody(IWorldBody body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            if (!value.IsRunning)
            {
                value.Enter(0, -0.035f, Vector3.down);
                GravityList.Add(new KeyValuePair<IWorldBody, CMEaseMove>(body,value));
            }
        }
    }

    public void AddGravityBuff(CMBody body)
    {
        if (!body.HasBuff(1))
        {
            //Debug.LogError("增加重力");
            GravityMoveBuffSystem buffSystem = GameLoop.Instace.GetFixedGameMoudle<BuffSystemManager>().FetchSystem<GravityMoveBuffSystem>();
            GravityMoveBuff gravityBuff = new GravityMoveBuff(1,buffSystem,body);
            gravityBuff.SetT(body);
            buffSystem.AddBuff(gravityBuff);
        }
    }

    public void RemoveGravityBuff(CMBody body)
    {
        if (body.HasBuff(1))
        {
            Debug.LogError("移除重力");
            GravityMoveBuffSystem buffSystem = GameLoop.Instace.GetFixedGameMoudle<BuffSystemManager>().FetchSystem<GravityMoveBuffSystem>();
            buffSystem.RemoveBuff(1);
        }
    }
    
    public void NegtiveBody(IWorldBody body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            if (value.IsRunning)
            {
                value.Exit();
                value.Power = 0;
            }
            GravityList.Clear();
        }
    }

    public Vector3 GetGravity(IWorldBody body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            return value.EaseVelocity;
        }
        return Vector3.zero;
    }
    
    public void FixedUpdate()
    {
        if (GravityList.Count > 0)
        {
            for (int i = GravityList.Count - 1; i >= 0; i--)
            {
                GravityList[i].Value.FixedUpdate();
                GravityList[i].Key.AffectByWorldRule(GravityList[i].Value.EaseVelocity);
                if (!GravityList[i].Value.IsRunning)
                {
                    GravityList.RemoveAt(i);
                }
            }
        }
    }
}