public interface ILifeCycle
{
    public void Awake();
    public void Destroy();
}

public interface ILifeCycle<T> : ILifeCycle
{
    public void Awake(T t);
}

public interface ILifeCycle<T1,T2> : ILifeCycle<T1>
{
    public void Awake(T1 t,T2 t2);
}

public interface ILifeCycle<T1,T2,T3> : ILifeCycle<T1,T2>
{
    public void Awake(T1 t,T2 t2,T3 t3);
}

public interface ILifeCycle<T1,T2,T3,T4> : ILifeCycle<T1,T2,T3>
{
    public void Awake(T1 t,T2 t2,T3 t3,T4 t4);
}

public interface ILifeCycle<T1,T2,T3,T4,T5> : ILifeCycle<T1,T2,T3,T4>
{
    public void Awake(T1 t,T2 t2,T3 t3,T4 t4,T5 t5);
}

public interface ILifeCycle<T1,T2,T3,T4,T5,T6> : ILifeCycle<T1,T2,T3,T4,T5>
{
    public void Awake(T1 t,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6);
}

public interface ILifeCycle<T1,T2,T3,T4,T5,T6,T7> : ILifeCycle<T1,T2,T3,T4,T5,T6>
{
    public void Awake(T1 t,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7);
}

public interface ILifeCycle<T1,T2,T3,T4,T5,T6,T7,T8> : ILifeCycle<T1,T2,T3,T4,T5,T6,T7>
{
    public void Awake(T1 t,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8);
}