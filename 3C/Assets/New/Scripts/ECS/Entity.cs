using System;

public class Entity
{
    public Type Type;

    public virtual void EType()
    {
        Type = this.GetType();
    }
}