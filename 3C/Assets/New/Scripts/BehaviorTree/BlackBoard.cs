public class BlackBoard :ILifeCycle
{
    public BTTimeMenter TimeMenter;

    public BlackBoard(BTTimeMenter time)
    {
        TimeMenter = time;
    }
    
    public void Awake()
    {
        
    }

    public void Destroy()
    {
        TimeMenter = null;
    }
}