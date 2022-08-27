using System;
using System.Collections.Generic;

public class BlackBoard :ILifeCycle
{
    public enum Type
    {
        ADD,
        REMOVE,
        CHANGE
    }
    private struct Notification
    {
        public string Key;
        public Type Type;
        public object Value;
        public Notification(string key, Type type, object value)
        {
            this.Key = key;
            this.Type = type;
            this.Value = value;
        }
    }
    
    private Dictionary<string, object> m_DataDict;
    private Dictionary<string, List<System.Action<Type, object>>> m_ObserverDict;
    private bool m_IsNotifiyng = false;
    private Dictionary<string, List<System.Action<Type, object>>> m_AddObserverDict;
    private Dictionary<string, List<System.Action<Type, object>>> m_RemoveObserverDict;
    
    private List<Notification> m_NotificationList;
    private List<Notification> m_NotificationsDispatchList;
    
    public BTTimeMenter TimeMenter { get; private set; }
    private BlackBoard m_Parent;
    private HashSet<BlackBoard> m_ChildrenSet;
    public BlackBoard(BTTimeMenter time)
    {
        TimeMenter = time;
    }
    
    public BlackBoard(BlackBoard parent, BTTimeMenter time)
    {
        this.TimeMenter = time;
        this.m_Parent = parent;
    }
   
    
    public void Awake()
    {
        m_ChildrenSet = new HashSet<BlackBoard>();
        m_DataDict = new Dictionary<string, object>();
        m_ObserverDict = new Dictionary<string, List<System.Action<Type, object>>>();
        m_AddObserverDict = new Dictionary<string, List<System.Action<Type, object>>>();
        m_RemoveObserverDict = new Dictionary<string, List<System.Action<Type, object>>>();
        m_NotificationList = new List<Notification>();
        m_NotificationsDispatchList = new List<Notification>();
    }

    public void Destroy()
    {
        TimeMenter = null;
        m_ChildrenSet = null;
        m_DataDict = null;
        m_ObserverDict = null;
        m_AddObserverDict = null;
        m_RemoveObserverDict = null;
    }
    
    public void Enable()
    {
        if (this.m_Parent != null)
        {
            this.m_Parent.m_ChildrenSet.Add(this);
        }
    }

    public void Disable()
    {
        if (this.m_Parent != null)
        {
            this.m_Parent.m_ChildrenSet.Remove(this);
        }
        if (this.TimeMenter != null)
        {
            this.TimeMenter.RemoveTimer(this.NotifiyObservers);
        }
    }

    public object this[string key]
    {
        get
        {
            return Get(key);
        }
        set
        {
            Set(key, value);
        }
    }

    public T Get<T>(string key)
    {
        object result = Get(key);
        if (result == null)
        {
            return default(T);
        }
        return (T)result;
    }

    public object Get(string key)
    {
        if (this.m_DataDict.ContainsKey(key))
        {
            return m_DataDict[key];
        }
        else if (this.m_Parent != null)
        {
            return this.m_Parent.Get(key);
        }
        else
        {
            return null;
        }
    }
    
    public void Set(string key)
    {
        if (!Isset(key))
        {
            Set(key, null);
        }
    }
    
    public bool Isset(string key)
    {
        return this.m_DataDict.ContainsKey(key) || (this.m_Parent != null && this.m_Parent.Isset(key));
    }
    
    public void Set(string key, object value)
    {
        if (this.m_Parent != null && this.m_Parent.Isset(key))
        {
            this.m_Parent.Set(key, value);
        }
        else
        {
            if (!this.m_DataDict.ContainsKey(key))
            {
                this.m_DataDict[key] = value;
                this.m_NotificationList.Add(new Notification(key, Type.ADD, value));
                this.TimeMenter.AddTimer(0f, 0, NotifiyObservers);
            }
            else
            {
                if ((this.m_DataDict[key] == null && value != null) || (this.m_DataDict[key] != null && !this.m_DataDict[key].Equals(value)))
                {
                    this.m_DataDict[key] = value;
                    this.m_NotificationList.Add(new Notification(key, Type.CHANGE, value));
                    this.TimeMenter.AddTimer(0f, 0, NotifiyObservers);
                }
            }
        }
    }

    public void Unset(string key)
    {
        if (this.m_DataDict.ContainsKey(key))
        {
            this.m_DataDict.Remove(key);
            this.m_NotificationList.Add(new Notification(key, Type.REMOVE, null));
            this.TimeMenter.AddTimer(0f, 0, NotifiyObservers);
        }
    }
    
    public void AddObserver(string key, System.Action<Type, object> observer)
    {
        List<System.Action<Type, object>> observers = GetObserverList(this.m_ObserverDict, key);
        if (!m_IsNotifiyng)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }
        else
        {
            if (!observers.Contains(observer))
            {
                List<System.Action<Type, object>> addObservers = GetObserverList(this.m_AddObserverDict, key);
                if (!addObservers.Contains(observer))
                {
                    addObservers.Add(observer);
                }
            }

            List<System.Action<Type, object>> removeObservers = GetObserverList(this.m_RemoveObserverDict, key);
            if (removeObservers.Contains(observer))
            {
                removeObservers.Remove(observer);
            }
        }
    }

    public void RemoveObserver(string key, System.Action<Type, object> observer)
    {
        List<System.Action<Type, object>> observers = GetObserverList(this.m_ObserverDict, key);
        if (!m_IsNotifiyng)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
        else
        {
            List<System.Action<Type, object>> removeObservers = GetObserverList(this.m_RemoveObserverDict, key);
            if (!removeObservers.Contains(observer))
            {
                if (observers.Contains(observer))
                {
                    removeObservers.Add(observer);
                }
            }

            List<System.Action<Type, object>> addObservers = GetObserverList(this.m_AddObserverDict, key);
            if (addObservers.Contains(observer))
            {
                addObservers.Remove(observer);
            }
        }
    }
    
    private void NotifiyObservers()
    {
        if (m_NotificationList.Count == 0)
        {
            return;
        }

        m_NotificationsDispatchList.Clear();
        m_NotificationsDispatchList.AddRange(m_NotificationList);
        foreach (BlackBoard child in m_ChildrenSet)
        {
            child.m_NotificationList.AddRange(m_NotificationList);
            child.TimeMenter.AddTimer(0f, 0, child.NotifiyObservers);
        }
        m_NotificationList.Clear();

        m_IsNotifiyng = true;
        foreach (Notification notification in m_NotificationsDispatchList)
        {
            if (!this.m_ObserverDict.ContainsKey(notification.Key))
            {
                continue;
            }

            List<System.Action<Type, object>> observers = GetObserverList(this.m_ObserverDict, notification.Key);
            foreach (System.Action<Type, object> observer in observers)
            {
                if (this.m_RemoveObserverDict.ContainsKey(notification.Key) && this.m_RemoveObserverDict[notification.Key].Contains(observer))
                {
                    continue;
                }
                observer(notification.Type, notification.Value);
            }
        }

        foreach (string key in this.m_AddObserverDict.Keys)
        {
            GetObserverList(this.m_ObserverDict, key).AddRange(this.m_AddObserverDict[key]);
        }
        foreach (string key in this.m_RemoveObserverDict.Keys)
        {
            foreach (System.Action<Type, object> action in m_RemoveObserverDict[key])
            {
                GetObserverList(this.m_ObserverDict, key).Remove(action);
            }
        }
        this.m_AddObserverDict.Clear();
        this.m_RemoveObserverDict.Clear();

        m_IsNotifiyng = false;
    }

    private List<System.Action<Type, object>> GetObserverList(Dictionary<string, List<System.Action<Type, object>>> target, string key)
    {
        List<System.Action<Type, object>> observers;
        if (target.ContainsKey(key))
        {
            observers = target[key];
        }
        else
        {
            observers = new List<System.Action<Type, object>>();
            target[key] = observers;
        }
        return observers;
    }
}