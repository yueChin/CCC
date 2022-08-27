using UnityEngine;

namespace NPBehave
{
    public class BtBlackboardCondition : BTObservingDecorator
    {
        private string key;
        private object value;
        private BTOperator op;

        public string Key
        {
            get
            {
                return key;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }

        public BTOperator BTOperator
        {
            get
            {
                return op;
            }
        }

        public BtBlackboardCondition(string key, BTOperator op, object value, BTStops stopsOnChange, BTNode decoratee) : base("BlackboardCondition", stopsOnChange, decoratee)
        {
            this.op = op;
            this.key = key;
            this.value = value;
            this.stopsOnChange = stopsOnChange;
        }
        
        public BtBlackboardCondition(string key, BTOperator op, BTStops stopsOnChange, BTNode decoratee) : base("BlackboardCondition", stopsOnChange, decoratee)
        {
            this.op = op;
            this.key = key;
            this.stopsOnChange = stopsOnChange;
        }


        protected override void StartObserving()
        {
            this.RootNode.BlackBoard.AddObserver(key, onValueChanged);
        }

        protected override void StopObserving()
        {
            this.RootNode.BlackBoard.RemoveObserver(key, onValueChanged);
        }

        private void onValueChanged(BlackBoard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            if (op == BTOperator.ALWAYS_TRUE)
            {
                return true;
            }

            if (!this.RootNode.BlackBoard.Isset(key))
            {
                return op == BTOperator.IS_NOT_SET;
            }

            object o = this.RootNode.BlackBoard.Get(key);

            switch (this.op)
            {
                case BTOperator.IS_SET: return true;
                case BTOperator.IS_EQUAL: return object.Equals(o, value);
                case BTOperator.IS_NOT_EQUAL: return !object.Equals(o, value);

                case BTOperator.IS_GREATER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o >= (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o >= (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_GREATER:
                    if (o is float)
                    {
                        return (float)o > (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o > (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_SMALLER_OR_EQUAL:
                    if (o is float)
                    {
                        return (float)o <= (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o <= (int)this.value;
                    }
                    else
                    {
                        Debug.LogError("Type not compareable: " + o.GetType());
                        return false;
                    }

                case BTOperator.IS_SMALLER:
                    if (o is float)
                    {
                        return (float)o < (float)this.value;
                    }
                    else if (o is int)
                    {
                        return (int)o < (int)this.value;
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
            return "(" + this.op + ") " + this.key + " ? " + this.value;
        }
    }
}