using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class RandomSequence : BTComposite
{
    static System.Random rng = new System.Random();
    private int currentIndex = -1;
    private int[] m_RandomizedOrder;

    public RandomSequence(params BTNode[] children) : base("Random Sequence", children)
    {
        m_RandomizedOrder = new int[children.Length];
        for (int i = 0; i < Children.Length; i++)
        {
            m_RandomizedOrder[i] = i;
        }
    }

    protected override void OnEnable()
    {
        foreach (BTNode child in Children)
        {
            Assert.AreEqual(child.CurrentState, State.INACTIVE);
        }

        currentIndex = -1;

        // Shuffling
        int n = m_RandomizedOrder.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (m_RandomizedOrder[n], m_RandomizedOrder[k]) = (m_RandomizedOrder[k], m_RandomizedOrder[n]);
        }

        ProcessChildren();
    }

    protected override void OnDisable()
    {
        Children[m_RandomizedOrder[currentIndex]].End();
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
                Children[m_RandomizedOrder[currentIndex]].Start();
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

    public override string ToString()
    {
        return base.ToString() + "[" + this.currentIndex + "]";
    }
}