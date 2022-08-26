
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldRule : MonoBehaviour
{
    public Dictionary<IWorldBody, CMEaseMove> GravityDict = new Dictionary<IWorldBody, CMEaseMove>();
    public List<KeyValuePair<IWorldBody,CMEaseMove>> GravityList = new List<KeyValuePair<IWorldBody,CMEaseMove>>();
    protected CustomEaseMove m_GravityMove;
    
    public float Gravity
    {
        get { return this.m_GravityMove.Power; }
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

    public void NegtiveBody(IWorldBody body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            if (value.IsRunning)
            {
                value.Exit();
                value.Power = 0;
            }
            GravityList.Remove(new KeyValuePair<IWorldBody, CMEaseMove>(body,value));
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