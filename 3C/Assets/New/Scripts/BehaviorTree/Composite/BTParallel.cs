using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

    public class BTParallel : BTComposite
    {
        public enum Policy
        {
            ONE,
            ALL,
        }

        // public enum Wait
        // {
        //     NEVER,
        //     ON_FAILURE,
        //     ON_SUCCESS,
        //     BOTH
        // }

        // private Wait waitForPendingChildrenRule;
        private Policy m_FailurePolicy;
        private Policy m_SuccessPolicy;
        private int m_ChildrenCount = 0;
        private int m_RunningCount = 0;
        private int m_SucceededCount = 0;
        private int m_FailedCount = 0;
        private Dictionary<BTNode, bool> m_ChildrenResultDict;
        private bool m_IsSuccessState;
        private bool m_IsChildrenAborted;

        public BTParallel(Policy successPolicy, Policy failurePolicy, /*Wait waitForPendingChildrenRule,*/ params BTNode[] children) : base("Parallel", children)
        {
            this.m_SuccessPolicy = successPolicy;
            this.m_FailurePolicy = failurePolicy;
            // this.waitForPendingChildrenRule = waitForPendingChildrenRule;
            this.m_ChildrenCount = children.Length;
            this.m_ChildrenResultDict = new Dictionary<BTNode, bool>();
        }

        protected override void OnEnable()
        {
            foreach (BTNode child in Children)
            {
                Assert.AreEqual(child.CurrentState, State.INACTIVE);
            }

            m_IsChildrenAborted = false;
            m_RunningCount = 0;
            m_SucceededCount = 0;
            m_FailedCount = 0;
            foreach (BTNode child in this.Children)
            {
                m_RunningCount++;
                child.Start();
            }
        }

        protected override void OnDisable()
        {
            Assert.IsTrue(m_RunningCount + m_SucceededCount + m_FailedCount == m_ChildrenCount);

            foreach (BTNode child in this.Children)
            {
                if (child.IsActive)
                {
                    child.End();
                }
            }
        }

        protected override void DoChildStopped(BTNode child, bool result)
        {
            m_RunningCount--;
            if (result)
            {
                m_SucceededCount++;
            }
            else
            {
                m_FailedCount++;
            }
            this.m_ChildrenResultDict[child] = result;

            bool allChildrenStarted = m_RunningCount + m_SucceededCount + m_FailedCount == m_ChildrenCount;
            if (allChildrenStarted)
            {
                if (m_RunningCount == 0)
                {
                    if (!this.m_IsChildrenAborted) // if children got aborted because rule was evaluated previously, we don't want to override the successState 
                    {
                        if (m_FailurePolicy == Policy.ONE && m_FailedCount > 0)
                        {
                            m_IsSuccessState = false;
                        }
                        else if (m_SuccessPolicy == Policy.ONE && m_SucceededCount > 0)
                        {
                            m_IsSuccessState = true;
                        }
                        else if (m_SuccessPolicy == Policy.ALL && m_SucceededCount == m_ChildrenCount)
                        {
                            m_IsSuccessState = true;
                        }
                        else
                        {
                            m_IsSuccessState = false;
                        }
                    }
                    Ended(m_IsSuccessState);
                }
                else if (!this.m_IsChildrenAborted)
                {
                    Assert.IsFalse(m_SucceededCount == m_ChildrenCount);
                    Assert.IsFalse(m_FailedCount == m_ChildrenCount);

                    if (m_FailurePolicy == Policy.ONE && m_FailedCount > 0/* && waitForPendingChildrenRule != Wait.ON_FAILURE && waitForPendingChildrenRule != Wait.BOTH*/)
                    {
                        m_IsSuccessState = false;
                        m_IsChildrenAborted = true;
                    }
                    else if (m_SuccessPolicy == Policy.ONE && m_SucceededCount > 0/* && waitForPendingChildrenRule != Wait.ON_SUCCESS && waitForPendingChildrenRule != Wait.BOTH*/)
                    {
                        m_IsSuccessState = true;
                        m_IsChildrenAborted = true;
                    }

                    if (m_IsChildrenAborted)
                    {
                        foreach (BTNode currentChild in this.Children)
                        {
                            if (currentChild.IsActive)
                            {
                                currentChild.End();
                            }
                        }
                    }
                }
            }
        }

        public override void StopLowerPriorityChildrenForChild(BTNode abortForChild, bool immediateRestart)
        {
            if (immediateRestart)
            {
                Assert.IsFalse(abortForChild.IsActive);
                if (m_ChildrenResultDict[abortForChild])
                {
                    m_SucceededCount--;
                }
                else
                {
                    m_FailedCount--;
                }
                m_RunningCount++;
                abortForChild.Start();
            }
            else
            {
                throw new SystemException("On Parallel Nodes all children have the same priority, thus the method does nothing if you pass false to 'immediateRestart'!");
            }
        }
    }