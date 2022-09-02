public class Buff : ILifeCycle,ILifeCycleable
{
    public int BuffId;
    public int Priority;
    public BTRootNode RootNode;
    public BuffSystem BuffSystem { get; protected set; }
    public MonoEntity MonoEntity;
    public bool IsRunning { get; private set; }
    public Entity Entity;
    public Buff(int id,BuffSystem buffSystem,MonoEntity entity,int priority = 0)
    {
        BuffId = id;
        BuffSystem = buffSystem;
        Priority = priority;
        MonoEntity = entity;
        MonoEntity.AddBuff(this);
        //BuffSystem.AddBuff(this);
    }

    public virtual void Awake()
    {
        //TODO Fetch init 
        //BuffSystem.AddBuff(this);
    }

    public virtual void Destroy()
    {
        RootNode.End();
        RootNode.Destroy();
        RootNode = null;
        MonoEntity.RemoveBuff(this.BuffId);
        MonoEntity = null;
    }

    public virtual void Tick(float timeDelta)
    {
        
    }

    public virtual void OnEnable()
    {
        IsRunning = true;
    }

    public virtual void Start()
    {
        
    }

    public virtual void OnDisable()
    {
        IsRunning = false;
    }

    public virtual void End()
    {
        BuffSystem.RemoveBuff(this);
    }
}

public class Buff<T> : Buff
{
    public T BuffData1;

    public void SetT(T data)
    {
        BuffData1 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2> : Buff<T1>
{
    public T2 BuffData2;
    
    public void SetT(T2 data)
    {
        BuffData2 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3> : Buff<T1,T2>
{
    public T3 BuffData3;
    
    public void SetT(T3 data)
    {
        BuffData3 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3,T4> : Buff<T1,T2,T3>
{
    public T4 BuffData4;
    
    public void SetT(T4 data)
    {
        BuffData4 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3,T4,T5> : Buff<T1,T2,T3,T4>
{
    public T5 BuffData5;
    
    public void SetT(T5 data)
    {
        BuffData5 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3,T4,T5,T6> : Buff<T1,T2,T3,T4,T5>
{
    public T6 BuffData6;
    
    public void SetT(T6 data)
    {
        BuffData6 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3,T4,T5,T6,T7> : Buff<T1,T2,T3,T4,T5,T6>
{
    public T7 BuffData7;
    
    public void SetT(T7 data)
    {
        BuffData7 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}

public class Buff<T1,T2,T3,T4,T5,T6,T7,T8> : Buff<T1,T2,T3,T4,T5,T6,T7>
{
    public T8 BuffData8;
    
    public void SetT(T8 data)
    {
        BuffData8 = data;
    }

    public Buff(int id,BuffSystem buffSystem,MonoEntity entity) : base(id,buffSystem,entity)
    {
    }
}