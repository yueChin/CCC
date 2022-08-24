public class Ease : Gear
{
    public float From;
    public float To;
    public float Current;
    public float EaseDelta;
    public int EaseFwdDirection;
    public bool IsRound;

    public float Progress
    {
        get
        {
            float v = this.From < this.To ? this.To : this.From;

            return this.Current / v;
        }
    }

    public void Update(float rate = 1)
    {
        if (!this.IsRunning)
        {
            return;
        }

        this.Current += this.EaseDelta * this.EaseFwdDirection * rate;

        if (this.EaseDelta > 0 && ((this.EaseFwdDirection == 1 && this.Current >= this.To) || (this.EaseFwdDirection == -1 && this.Current <= this.To)))
        {
            this.Current = this.To;

            if (this.IsRound)
            {
                this.Enter(this.To, this.From, this.EaseDelta, this.IsRound);
            }
            else
            {
                this.Exit();
            }
        }
    }

    public void Enter(float from, float to, float easeDelta, bool isRound = false)
    {
        base.Enter();

        this.From = from;
        this.To = to;
        this.Current = from;
        this.EaseDelta = easeDelta;
        this.IsRound = isRound;
        this.EaseFwdDirection = this.From < this.To ? 1 : -1;
    }
}