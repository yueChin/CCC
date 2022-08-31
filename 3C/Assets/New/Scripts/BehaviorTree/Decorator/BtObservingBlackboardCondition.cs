using UnityEngine;

namespace NPBehave
{
    public class BtObservingBlackboardCondition : BTObservingDecorator
    {
        private string m_Key;
        private object m_Value;
        private BTOperator m_Op;

        public string Key
        {
            get
            {
                return m_Key;
            }
        }

        public object Value
        {
            get
            {
                return m_Value;
            }
        }

        public BTOperator BTOperator
        {
            get
            {
                return m_Op;
            }
        }

        public BtObservingBlackboardCondition(string key, BTOperator op, object value, BTStops stopsOnChange, BTNode decoratee) : base("BlackboardCondition", stopsOnChange, decoratee)
        {
            this.m_Op = op;
            this.m_Key = key;
            this.m_Value = value;
            this.m_StopsOnChange = stopsOnChange;
        }
        
        public BtObservingBlackboardCondition(string key, BTOperator op, BTStops stopsOnChange, BTNode decoratee) : base("BlackboardCondition", stopsOnChange, decoratee)
        {
            this.m_Op = op;
            this.m_Key = key;
            this.m_StopsOnChange = stopsOnChange;
        }


        protected override void StartObserving()
        {
            this.RootNode.BlackBoard.AddObserver(m_Key, onValueChanged);
        }

        protected override void StopObserving()
        {
            this.RootNode.BlackBoard.RemoveObserver(m_Key, onValueChanged);
        }

        private void onValueChanged(BlackBoard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            if (m_Op == BTOperator.ALWAYS_TRUE)
            {
                return true;
            }

            if (!this.RootNode.BlackBoard.Isset(m_Key))
            {
                return m_Op == BTOperator.IS_NOT_SET;
            }

            object o = this.RootNode.BlackBoard.Get(m_Key);

            switch (this.m_Op)
            {
                case BTOperator.IS_SET: return true;
                case BTOperator.IS_EQUAL: return object.Equals(o, m_Value);
                case BTOperator.IS_NOT_EQUAL: return !object.Equals(o, m_Value);

                case BTOperator.IS_GREATER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o >= (float)this.m_Value;
                    }
                    else if (o is int)
                    {
                        return (int)o >= (int)this.m_Value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_GREATER:
                    if (o is float)
                    {
                        return (float)o > (float)this.m_Value;
                    }
                    else if (o is int)
                    {
                        return (int)o > (int)this.m_Value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_SMALLER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o <= (float)this.m_Value;
                    }
                    else if (o is int)
                    {
                        return (int)o <= (int)this.m_Value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_SMALLER:
                    if (o is float)
                    {
                        return (float)o < (float)this.m_Value;
                    }
                    else if (o is int)
                    {
                        return (int)o < (int)this.m_Value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                default: return false;
            }
        }

        public override string ToString()
        {
            return "(" + this.m_Op + ") " + this.m_Key + " ? " + this.m_Value;
        }
    }
}