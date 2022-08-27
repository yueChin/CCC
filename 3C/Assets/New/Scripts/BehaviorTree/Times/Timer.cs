class BTTimer
{
    public double ScheduledTime = 0f;
    public int Repeat = 0;
    public bool Used = false;
    public double Delay = 0f;
    public float RandomVariance = 0.0f;

    public void ScheduleAbsoluteTime(double elapsedTime)
    {
        ScheduledTime = elapsedTime + Delay - RandomVariance * 0.5f + RandomVariance * UnityEngine.Random.value;
    }
}