using UnityEngine.Assertions;

public class BTSequence : BTComposite
{
    private int currentIndex = -1;

    public BTSequence(params BTNode[] children) : base("Sequence", children)
    {
    }

    protected override void OnEnable()
    {
        foreach (BTNode child in Children)
        {
            Assert.AreEqual(child.CurrentState, State.INACTIVE);
        }

        currentIndex = -1;

        ProcessChildren();
    }

    protected override void OnDisable()
    {
        Children[currentIndex].End();
    }


    protected override void DoChildStopped(BTNode child, bool result)
    {
        if (result)
        {
            ProcessChildren();
        }
        else
        {
            Ended(false);
        }
    }

    private void ProcessChildren()
    {
        if (++currentIndex < Children.Length)
        {
            if (IsStopRequested)
            {
                Ended(false);
            }
            else
            {
                Children[currentIndex].Start();
            }
        }
        else
        {
            Ended(true);
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
                currentChild.End();
                break;
            }
        }
    }

    override public string ToString()
    {
        return base.ToString() + "[" + this.currentIndex + "]";
    }
}