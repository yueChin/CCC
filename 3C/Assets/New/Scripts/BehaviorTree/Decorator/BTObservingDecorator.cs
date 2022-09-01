using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public abstract class BTObservingDecorator : BTDecorator
{
    protected BTStops StopsOnChange;
    private bool m_IsObserving;

    public BTObservingDecorator(string name, BTStops stopsOnChange, BTNode childNode) : base(name, childNode)
    {
        this.StopsOnChange = stopsOnChange;
        this.m_IsObserving = false;
    }

    protected override void OnEnable()
    {
        if (StopsOnChange != BTStops.NONE)
        {
            if (!m_IsObserving)
            {
                m_IsObserving = true;
                StartObserving();
            }
        }

        if (!IsConditionMet())
        {
            Ended(false);
        }
        else
        {
            ChildNode.Start();
        }
    }

    protected override void OnDisable()
    {
        ChildNode.End();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        if (StopsOnChange == BTStops.NONE || StopsOnChange == BTStops.SELF)
        {
            if (m_IsObserving)
            {
                m_IsObserving = false;
                StopObserving();
            }
        }
        Ended(result);
    }

    protected override void DoParentCompositeStopped(BTComposite parentComposite)
    {
        if (m_IsObserving)
        {
            m_IsObserving = false;
            StopObserving();
        }
    }

    protected void Evaluate()
    {
        if (IsActive && !IsConditionMet())
        {
            if (StopsOnChange == BTStops.SELF || StopsOnChange == BTStops.BOTH || StopsOnChange == BTStops.IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped self ");
                this.End();
            }
        }
        else if (!IsActive && IsConditionMet())
        {
            if (StopsOnChange == BTStops.LOWER_PRIORITY || StopsOnChange == BTStops.BOTH || StopsOnChange == BTStops.IMMEDIATE_RESTART || StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped other ");
                BTContainer parentNode = this.Parent;
                BTNode childNode = this;
                while (parentNode != null && !(parentNode is BTComposite))
                {
                    childNode = parentNode;
                    parentNode = parentNode.Parent;
                }
                Assert.IsNotNull(parentNode, "NTBtrStops is only valid when attached to a parent composite");
                Assert.IsNotNull(childNode);
                if (parentNode is BTParallel)
                {
                    Assert.IsTrue(StopsOnChange == BTStops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus BTStops.LOWER_PRIORITY or BTStops.BOTH are unsupported in this context!");
                }

                if (StopsOnChange == BTStops.IMMEDIATE_RESTART || StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    if (m_IsObserving)
                    {
                        m_IsObserving = false;
                        StopObserving();
                    }
                }

                ((BTComposite)parentNode).StopLowerPriorityChildrenForChild(childNode, StopsOnChange == BTStops.IMMEDIATE_RESTART || StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART);
            }
        }
    }

    protected abstract void StartObserving();

    protected abstract void StopObserving();

    protected abstract bool IsConditionMet();

}