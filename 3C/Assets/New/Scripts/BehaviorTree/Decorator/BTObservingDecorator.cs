using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public abstract class BTObservingDecorator : BTDecorator
{
    protected BTStops stopsOnChange;
    private bool isObserving;

    public BTObservingDecorator(string name, BTStops stopsOnChange, BTNode decoratee) : base(name, decoratee)
    {
        this.stopsOnChange = stopsOnChange;
        this.isObserving = false;
    }

    protected override void DoStart()
    {
        if (stopsOnChange != BTStops.NONE)
        {
            if (!isObserving)
            {
                isObserving = true;
                StartObserving();
            }
        }

        if (!IsConditionMet())
        {
            Stopped(false);
        }
        else
        {
            Decoratee.Start();
        }
    }

    protected override void DoStop()
    {
        Decoratee.Stop();
    }

    protected override void DoChildStopped(BTNode child, bool result)
    {
        Assert.AreNotEqual(this.CurrentState, State.INACTIVE);
        if (stopsOnChange == BTStops.NONE || stopsOnChange == BTStops.SELF)
        {
            if (isObserving)
            {
                isObserving = false;
                StopObserving();
            }
        }
        Stopped(result);
    }

    protected override void DoParentCompositeStopped(BTComposite parentComposite)
    {
        if (isObserving)
        {
            isObserving = false;
            StopObserving();
        }
    }

    protected void Evaluate()
    {
        if (IsActive && !IsConditionMet())
        {
            if (stopsOnChange == BTStops.SELF || stopsOnChange == BTStops.BOTH || stopsOnChange == BTStops.IMMEDIATE_RESTART)
            {
                // Debug.Log( this.key + " stopped self ");
                this.Stop();
            }
        }
        else if (!IsActive && IsConditionMet())
        {
            if (stopsOnChange == BTStops.LOWER_PRIORITY || stopsOnChange == BTStops.BOTH || stopsOnChange == BTStops.IMMEDIATE_RESTART || stopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
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
                    Assert.IsTrue(stopsOnChange == BTStops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus BTStops.LOWER_PRIORITY or BTStops.BOTH are unsupported in this context!");
                }

                if (stopsOnChange == BTStops.IMMEDIATE_RESTART || stopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    if (isObserving)
                    {
                        isObserving = false;
                        StopObserving();
                    }
                }

                ((BTComposite)parentNode).StopLowerPriorityChildrenForChild(childNode, stopsOnChange == BTStops.IMMEDIATE_RESTART || stopsOnChange == BTStops.LOWER_PRIORITY_IMMEDIATE_RESTART);
            }
        }
    }

    protected abstract void StartObserving();

    protected abstract void StopObserving();

    protected abstract bool IsConditionMet();

}