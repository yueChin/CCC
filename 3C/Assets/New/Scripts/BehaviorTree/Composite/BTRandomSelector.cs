using UnityEngine.Assertions;


public class BTRandomSelector : BTComposite
{
    static System.Random rng = new System.Random();

    private int m_CurrentIndex = -1;
    private int[] m_RandomizedOrder;

    public BTRandomSelector(params BTNode[] children) : base("Random Selector", children)
    {
        m_RandomizedOrder = new int[children.Length];
        for (int i = 0; i < Children.Length; i++)
        {
            m_RandomizedOrder[i] = i;
        }
    }


    protected override void DoStart()
    {
        foreach (BTNode child in Children)
        {
            Assert.AreEqual(child.CurrentState, State.INACTIVE);
        }

        m_CurrentIndex = -1;

        // Shuffling
        int n = m_RandomizedOrder.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (m_RandomizedOrder[n], m_RandomizedOrder[k]) = (m_RandomizedOrder[k], m_RandomizedOrder[n]);
        }

        ProcessChildren();
    }



    protected override void DoStop()
    {
        Children[m_RandomizedOrder[m_CurrentIndex]].Stop();
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
        if (++m_CurrentIndex < Children.Length)
        {
            if (IsStopRequested)
            {
                Stopped(false);
            }
            else
            {
                Children[m_RandomizedOrder[m_CurrentIndex]].Start();
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
                    m_CurrentIndex = indexForChild - 1;
                }
                else
                {
                    m_CurrentIndex = Children.Length;
                }
                currentChild.Stop();
                break;
            }
        }
    }

    public override string ToString()
    {
        return base.ToString() + "[" + this.m_CurrentIndex + "]";
    }
}
