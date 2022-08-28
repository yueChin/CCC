using System.Collections.Generic;

public class BuffSystem : IGameMoudle
{
    public List<Buff> BuffList;
    public void Awake()
    {
        BuffList = new List<Buff>();
    }

    public void Destroy()
    {
        foreach (Buff buff in BuffList)
        {
            buff.Destroy();
        }

        BuffList = null;
    }

    public void Tick()
    {
        foreach (Buff buff in BuffList)
        {
            buff.Tick();
        }
    }
}