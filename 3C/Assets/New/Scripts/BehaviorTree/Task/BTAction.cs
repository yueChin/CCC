using UnityEngine.Assertions;

public class BTAction : BTTask
{
    public enum Result
    {
        SUCCESS,
        FAILED,
        BLOCKED,
        PROGRESS
    }

    public enum Request
    {
        START,
        UPDATE,
        CANCEL,
    }

    private readonly System.Func<bool> m_SingleFrameFunc = null;
    private readonly System.Func<bool, Result> m_MultiFrameFunc = null;
    private readonly System.Func<Request, Result> m_MultiFrameFunc2 = null;
    private readonly System.Action m_Action = null;
    private bool m_WasBlocked = false;

    public BTAction(System.Action action) : base("Action")
    {
        this.m_Action = action;
    }

    public BTAction(System.Func<bool, Result> multiframeFunc) : base("Action")
    {
        this.m_MultiFrameFunc = multiframeFunc;
    }

    public BTAction(System.Func<Request, Result> multiframeFunc2) : base("Action")
    {
        this.m_MultiFrameFunc2 = multiframeFunc2;
    }


    public BTAction(System.Func<bool> singleFrameFunc) : base("Action")
    {
        this.m_SingleFrameFunc = singleFrameFunc;
    }

    protected override void DoStart()
    {
        if (this.m_Action != null)
        {
            this.m_Action.Invoke();
            this.Stopped(true);
        }
        else if (this.m_MultiFrameFunc != null)
        {
            Result result = this.m_MultiFrameFunc.Invoke(false);
            if (result == Result.PROGRESS)
            {
                this.RootNode.TimeMenter.AddUpdateObserver(OnUpdateFunc);
            }
            else if (result == Result.BLOCKED)
            {
                this.m_WasBlocked = true;
                this.RootNode.TimeMenter.AddUpdateObserver(OnUpdateFunc);
            }
            else
            {
                this.Stopped(result == Result.SUCCESS);
            }
        }
        else if (this.m_MultiFrameFunc2 != null)
        {
            Result result = this.m_MultiFrameFunc2.Invoke(Request.START);
            if (result == Result.PROGRESS)
            {
                this.RootNode.TimeMenter.AddUpdateObserver(OnUpdateFunc2);
            }
            else if (result == Result.BLOCKED)
            {
                this.m_WasBlocked = true;
                this.RootNode.TimeMenter.AddUpdateObserver(OnUpdateFunc2);
            }
            else
            {
                this.Stopped(result == Result.SUCCESS);
            }
        }
        else if (this.m_SingleFrameFunc != null)
        {
            this.Stopped(this.m_SingleFrameFunc.Invoke());
        }
    }

    private void OnUpdateFunc()
    {
        Result result = this.m_MultiFrameFunc.Invoke(false);
        if (result != Result.PROGRESS && result != Result.BLOCKED)
        {
            this.RootNode.TimeMenter.RemoveUpdateObserver(OnUpdateFunc);
            this.Stopped(result == Result.SUCCESS);
        }
    }

    private void OnUpdateFunc2()
    {
        Result result = this.m_MultiFrameFunc2.Invoke(m_WasBlocked ? Request.START : Request.UPDATE);

        if (result == Result.BLOCKED)
        {
            m_WasBlocked = true;
        }
        else if (result == Result.PROGRESS)
        {
            m_WasBlocked = false;
        }
        else
        {
            this.RootNode.TimeMenter.RemoveUpdateObserver(OnUpdateFunc2);
            this.Stopped(result == Result.SUCCESS);
        }
    }

    protected override void DoStop()
    {
        if (this.m_MultiFrameFunc != null)
        {
            Result result = this.m_MultiFrameFunc.Invoke(true);
            Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS, Result.FAILED/BLOCKED after beeing cancelled!");
            this.RootNode.TimeMenter.RemoveUpdateObserver(OnUpdateFunc);
            this.Stopped(result == Result.SUCCESS);
        }
        else if (this.m_MultiFrameFunc2 != null)
        {
            Result result = this.m_MultiFrameFunc2.Invoke(Request.CANCEL);
            Assert.AreNotEqual(result, Result.PROGRESS, "The Task has to return Result.SUCCESS or Result.FAILED/BLOCKED after beeing cancelled!");
            this.RootNode.TimeMenter.RemoveUpdateObserver(OnUpdateFunc2);
            this.Stopped(result == Result.SUCCESS);
        }
        else
        {
            Assert.IsTrue(false, "DoStop called for a single frame action on " + this);
        }
    }
}