public class Buff : ILifeCycle
{
    public CMBody CmBodyComponent;

    public Buff()
    {
        
    }

    public void Awake()
    {
    }

    public void Destroy()
    {
        CmBodyComponent = null;
    }

    public void Tick()
    {
        
    }
}