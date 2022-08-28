public class GenericFSM<T> : FSM
{
    public T FSMData { get; private set; }

    public void SetT(T t)
    {
        FSMData = t;
    }
}