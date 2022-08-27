using UnityEngine.Assertions;

public class BTSelector : BTComposite
{
    private int currentIndex = -1;

    public BTSelector(params BTNode[] children) : base("Selector", children)
    {
    }


    protected override void DoStart()
    {
        foreach (BTNode child in Children)
        {
            Assert.AreEqual(child.CurrentState, State.INACTIVE);
        }

        currentIndex = -1;

        ProcessChildren();
    }

    protected override void DoStop()
    {
        Children[currentIndex].Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        if (result)
        {
            Stopped(true);
        }
        else
        {
            ProcessChildren();
        }
    }

    private void ProcessChildren()
    {
        if (++currentIndex < Children.Length)
        {
            if (IsStopRequested)
            {
                Stopped(false);
            }
            else
            {
                Children[currentIndex].Start();
            }
        }
        else
        {
            Stopped(false);
        }
    }

    public override void StopLowerPriorityChildrenForChild(BTNode abortForChild, bool immediateRestart)
    {
        int indexForChild = 0;
        bool found = false;
        foreach (BTNode currentChild in Children)
        {
            if (currentChild == abortForChild)
            {
                found = true;
            }
            else if (!found)
            {
                indexForChild++;
            }
            else if (found && currentChild.IsActive)
            {
                if (immediateRestart)
                {
                    currentIndex = indexForChild - 1;
                }
                else
                {
                    currentIndex = Children.Length;
                }
                currentChild.Stop();
                break;
            }
        }
    }

    public override string ToString()
    {
        return base.ToString() + "[" + this.currentIndex + "]";
    }
}