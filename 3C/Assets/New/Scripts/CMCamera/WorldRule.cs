
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldRule : MonoBehaviour
{
    public Dictionary<Body, CMEaseMove> GravityDict = new Dictionary<Body, CMEaseMove>();
    public List<CMEaseMove> GravityList = new List<CMEaseMove>();
    protected CustomEaseMove m_GravityMove;
    
    public float Gravity
    {
        get { return this.m_GravityMove.Power; }
    }
    
    public void SetActiveBody(Body body)
    {
        if (!GravityDict.ContainsKey(body))
        {
            GravityDict.Add(body,new CMEaseMove());
        }
    }

    public void ActiveBody(Body body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            if (!value.IsRunning)
            {
                value.Enter(0, -0.035f, Vector3.down);
                GravityList.Add(value);
            }
        }
    }

    public void NegtiveBody(Body body)
    {
        if (GravityDict.TryGetValue(body, out CMEaseMove value))
        {
            if (value.IsRunning)
            {
                value.Exit();
                value.Power = 0;
            }
            GravityList.Remove(value);
        }
    }

    public Vector3 GetGravity(Body body)
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
                GravityList[i].FixedUpdate();
                if (!GravityList[i].IsRunning)
                {
                    GravityList.RemoveAt(i);
                }
            }
        }
    }
}