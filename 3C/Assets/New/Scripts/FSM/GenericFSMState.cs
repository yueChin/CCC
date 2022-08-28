using System;
using System.Reflection;
using Kits.DevlpKit.Helpers.ReflectionHelper;

public class GenericFSMState<T> : FSMState
{
    public T t { get; protected set; }
    public GenericFSMState(int id, string name = null) : base(id, name)
    {
        
    }

    public override void Awake()
    {
        base.Awake();
        object obj = PropertyInfoHelper.GetPropertyValue(FSM, "FSMData");
        if (obj != null)
        {
            t = (T)obj;
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        t = default;
    }
}