namespace NPBehave
{
    public class BTObservingBlackboardQuery : BTObservingDecorator
    {
        private string[] m_Keys;
        private System.Func<bool> m_Query;

        public BTObservingBlackboardQuery(string[] keys, BTStops stopsOnChange, System.Func<bool> query, BTNode childNode) : base("BlackboardQuery", stopsOnChange, childNode)
        {
            this.m_Keys = keys;
            this.m_Query = query;
        }

        protected override void StartObserving()
        {
            foreach (string key in this.m_Keys)
            {
                this.RootNode.BlackBoard.AddObserver(key, OnValueChanged);
            }
        }

        protected override void StopObserving()
        {
            foreach (string key in this.m_Keys)
            {
                this.RootNode.BlackBoard.RemoveObserver(key, OnValueChanged);
            }
        }

        private void OnValueChanged(BlackBoard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return this.m_Query();
        }

        public override string ToString()
        {
            string keys = "";
            foreach (string key in this.m_Keys)
            {
                keys += " " + key;
            }
            return Name + keys;
        }
    }
}