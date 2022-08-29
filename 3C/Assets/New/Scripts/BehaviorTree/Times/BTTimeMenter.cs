using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class BTTimeMenter :ILifeCycle
{
    private List<System.Action> m_UpdateObserverList;
    private Dictionary<System.Action, BTTimer> m_TimerDict ;
    private HashSet<System.Action> m_RemoveObserverSet ;
    private HashSet<System.Action> m_AddObserverSet ;
    private HashSet<System.Action> m_RemoveTimerSet ;
    private Dictionary<System.Action, BTTimer> m_AddTimerDict ;
    private List<BTTimer> m_TimerPoolList;

    private bool m_IsInUpdate = false;
    private double m_ElapsedTime = 0f;
    private int m_CurrentTimerPoolIndex = 0;

    public int NumUpdateObservers => m_UpdateObserverList.Count;

    public int NumTimers => m_TimerDict.Count;

    public double ElapsedTime => m_ElapsedTime;

    /// <summary>Register a timer function</summary>
    /// <param name="time">time in milliseconds</param>
    /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
    /// <param name="action">method to invoke</param>
    public void AddTimer(float time, int repeat, System.Action action)
    {
        AddTimer(time, 0f, repeat, action);
    }

    /// <summary>Register a timer function with random variance</summary>
    /// <param name="time">time in milliseconds</param>
    /// <param name="randomVariance">deviate from time on a random basis</param>
    /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
    /// <param name="action">method to invoke</param>
    public void AddTimer(float delay, float randomVariance, int repeat, System.Action action)
    {
		
		BTTimer timer = null;

        if (!m_IsInUpdate)
        {
            if (!this.m_TimerDict.ContainsKey(action))
            {
				this.m_TimerDict[action] = GetTimerFromPool();
            }
			timer = this.m_TimerDict[action];
        }
        else
        {
            if (!this.m_AddTimerDict.ContainsKey(action))
            {
				this.m_AddTimerDict[action] = GetTimerFromPool();
            }
			timer = this.m_AddTimerDict [action];

            if (this.m_RemoveTimerSet.Contains(action))
            {
                this.m_RemoveTimerSet.Remove(action);
            }
        }

		Assert.IsTrue(timer.Used);
		timer.Delay = delay;
		timer.RandomVariance = randomVariance;
		timer.Repeat = repeat;
		timer.ScheduleAbsoluteTime(m_ElapsedTime);
    }

    public void RemoveTimer(System.Action action)
    {
        if (!m_IsInUpdate)
        {
            if (this.m_TimerDict.ContainsKey(action))
            {
                m_TimerDict[action].Used = false;
                this.m_TimerDict.Remove(action);
            }
        }
        else
        {
            if (this.m_TimerDict.ContainsKey(action))
            {
                this.m_RemoveTimerSet.Add(action);
            }
            if (this.m_AddTimerDict.ContainsKey(action))
            {
                Assert.IsTrue(this.m_AddTimerDict[action].Used);
                this.m_AddTimerDict[action].Used = false;
                this.m_AddTimerDict.Remove(action);
            }
        }
    }

    public bool HasTimer(System.Action action)
    {
        if (!m_IsInUpdate)
        {
            return this.m_TimerDict.ContainsKey(action);
        }
        else
        {
            if (this.m_RemoveTimerSet.Contains(action))
            {
                return false;
            }
            else if (this.m_AddTimerDict.ContainsKey(action))
            {
                return true;
            }
            else
            {
                return this.m_TimerDict.ContainsKey(action);
            }
        }
    }

    /// <summary>Register a function that is called every frame</summary>
    /// <param name="action">function to invoke</param>
    public void AddUpdateObserver(System.Action action)
    {
        if (!m_IsInUpdate)
        {
            this.m_UpdateObserverList.Add(action);
        }
        else
        {
            if (!this.m_UpdateObserverList.Contains(action))
            {
                this.m_AddObserverSet.Add(action);
            }
            if (this.m_RemoveObserverSet.Contains(action))
            {
                this.m_RemoveObserverSet.Remove(action);
            }
        }
    }

    public void RemoveUpdateObserver(System.Action action)
    {
        if (!m_IsInUpdate)
        {
            this.m_UpdateObserverList.Remove(action);
        }
        else
        {
            if (this.m_UpdateObserverList.Contains(action))
            {
                this.m_RemoveObserverSet.Add(action);
            }
            if (this.m_AddObserverSet.Contains(action))
            {
                this.m_AddObserverSet.Remove(action);
            }
        }
    }

    public bool HasUpdateObserver(System.Action action)
    {
        if (!m_IsInUpdate)
        {
            return this.m_UpdateObserverList.Contains(action);
        }
        else
        {
            if (this.m_RemoveObserverSet.Contains(action))
            {
                return false;
            }
            else if (this.m_AddObserverSet.Contains(action))
            {
                return true;
            }
            else
            {
                return this.m_UpdateObserverList.Contains(action);
            }
        }
    }

    public void Update(float deltaTime)
    {
        this.m_ElapsedTime += deltaTime;

        this.m_IsInUpdate = true;

        foreach (System.Action action in m_UpdateObserverList)
        {
            if (!m_RemoveObserverSet.Contains(action))
            {
                action.Invoke();
            }
        }

        Dictionary<System.Action, BTTimer>.KeyCollection keys = m_TimerDict.Keys;
		foreach (System.Action callback in keys)
        {
            if (this.m_RemoveTimerSet.Contains(callback))
            {
                continue;
            }

			BTTimer timer = m_TimerDict[callback];
            if (timer.ScheduledTime <= this.m_ElapsedTime)
            {
                if (timer.Repeat == 0)
                {
                    RemoveTimer(callback);
                }
                else if (timer.Repeat >= 0)
                {
                    timer.Repeat--;
                }
                callback.Invoke();
				timer.ScheduleAbsoluteTime(m_ElapsedTime);
            }
        }

        foreach (System.Action action in this.m_AddObserverSet)
        {
            this.m_UpdateObserverList.Add(action);
        }
        foreach (System.Action action in this.m_RemoveObserverSet)
        {
            this.m_UpdateObserverList.Remove(action);
        }
        foreach (System.Action action in this.m_AddTimerDict.Keys)
        {
            if (this.m_TimerDict.ContainsKey(action))
            {
                Assert.AreNotEqual(this.m_TimerDict[action], this.m_AddTimerDict[action]);
                this.m_TimerDict[action].Used = false;
            }
            Assert.IsTrue(this.m_AddTimerDict[action].Used);
            this.m_TimerDict[action] = this.m_AddTimerDict[action];
        }
        foreach (System.Action action in this.m_RemoveTimerSet)
        {
            Assert.IsTrue(this.m_TimerDict[action].Used);
            m_TimerDict[action].Used = false;
            this.m_TimerDict.Remove(action);
        }
        this.m_AddObserverSet.Clear();
        this.m_RemoveObserverSet.Clear();
        this.m_AddTimerDict.Clear();
        this.m_RemoveTimerSet.Clear();

        this.m_IsInUpdate = false;
    }

    private BTTimer GetTimerFromPool()
    {
        int i = 0;
        int l = m_TimerPoolList.Count;
        BTTimer timer = null;
        while (i < l)
        {
            int timerIndex = (i + m_CurrentTimerPoolIndex) % l;
            if (!m_TimerPoolList[timerIndex].Used)
            {
                m_CurrentTimerPoolIndex = timerIndex;
                timer = m_TimerPoolList[timerIndex];
                break;
            }
            i++;
        }

        if (timer == null)
        {
            timer = new BTTimer();
            m_CurrentTimerPoolIndex = 0;
            m_TimerPoolList.Add(timer);
        }

        timer.Used = true;
        return timer;
    }

    public int DebugPoolSize
    {
        get
        {
            return this.m_TimerPoolList.Count;
        }
    }

    public void Awake()
    {
        m_UpdateObserverList = new List<System.Action>();
        m_TimerDict = new Dictionary<System.Action, BTTimer>();
        m_RemoveObserverSet = new HashSet<System.Action>();
        m_AddObserverSet = new HashSet<System.Action>();
        m_RemoveTimerSet = new HashSet<System.Action>();
        m_AddTimerDict = new Dictionary<System.Action, BTTimer>();
        m_TimerPoolList = new List<BTTimer>();
        m_IsInUpdate = false;
        m_ElapsedTime = 0f;
        m_CurrentTimerPoolIndex = 0;
    }

    public void Destroy()
    {
        m_UpdateObserverList.Clear();
        m_TimerDict.Clear();
        m_RemoveObserverSet.Clear();
        m_AddObserverSet.Clear();
        m_AddTimerDict.Clear();
        m_TimerPoolList.Clear();
        
        m_UpdateObserverList = null;
        m_TimerDict = null;
        m_RemoveObserverSet = null;
        m_AddObserverSet = null;
        m_AddTimerDict = null;
        m_TimerPoolList = null;
        
        m_IsInUpdate = false;
        m_ElapsedTime = 0f;
        m_CurrentTimerPoolIndex = 0;
    }

    public void Tick(float timeDelta)
    {
        this.Update(timeDelta);
    }
}