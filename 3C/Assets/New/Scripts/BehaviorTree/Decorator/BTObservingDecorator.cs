using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public abstract class BTObservingDecorator : BTDecorator
{
    protected BTStops m_StopsOnChange;
    private bool m_IsObserving;

    public BTObservingDecorator(string name, BTStops m_StopsOnChange, BTNode decoratee) : base(name, decoratee)
    {
        this.m_StopsOnChange = m_StopsOnChange;
        this.m_IsObserving = false;
    }

    protected override void DoStart()
    {
        if (m_StopsOnChange != BTStops.NONE)
        {
            if (!m_IsObserving)
            {
                m_IsObserving = true;
                StartObserving();
            }
        }

        if (!IsConditionMet())
        {
            Stopped(false);
        }
        else
        {
            ChildNode.Start();
        }
    }

    protected override void DoStop()
    {
        ChildNode.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        if (m_StopsOnChange == BTStops.NONE || m_StopsOnChange == BTStops.SELF)
        {
            if (m_IsObserving)
            {
                m_IsObserving = false;
                StopObserving();
            }
        }
        Stopped(result);
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
            if (m_StopsOnChange == BTStops.SELF || m_StopsOnChange == BTStops.BOTH || m_StopsOnChange == BTStops.IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped self ");
                this.Stop();
            }
        }
        else if (!IsActive && IsConditionMet())
        {
            if (m_StopsOnChange == BTStops.LOWER_PRIORITY || m_StopsOnChange == BTStops.BOTH || m_StopsOnChange == BTStops.IMMEDIATE_RESTART || m_StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
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
                    Assert.IsTrue(m_StopsOnChange == BTStops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus BTStops.LOWER_PRIORITY or BTStops.BOTH are unsupported in this context!");
                }

                if (m_StopsOnChange == BTStops.IMMEDIATE_RESTART || m_StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    if (m_IsObserving)
                    {
                        m_IsObserving = false;
                        StopObserving();
                    }
                }

                ((BTComposite)parentNode).StopLowerPriorityChildrenForChild(childNode, m_StopsOnChange == BTStops.IMMEDIATE_RESTART || m_StopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART);
            }
        }
    }

    protected abstract void StartObserving();

    protected abstract void StopObserving();

    protected abstract bool IsConditionMet();

}