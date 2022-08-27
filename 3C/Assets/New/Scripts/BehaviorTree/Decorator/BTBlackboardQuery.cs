namespace NPBehave
{
    public class BTBlackboardQuery : BTObservingDecorator
    {
        private string[] keys;
        private System.Func<bool> query;

        public BTBlackboardQuery(string[] keys, BTStops stopsOnChange, System.Func<bool> query, BTNode decoratee) : base("BlackboardQuery", stopsOnChange, decoratee)
        {
            this.keys = keys;
            this.query = query;
        }

        protected override void StartObserving()
        {
            foreach (string key in this.keys)
            {
                this.RootNode.BlackBoard.AddObserver(key, onValueChanged);
            }
        }

        protected override void StopObserving()
        {
            foreach (string key in this.keys)
            {
                this.RootNode.BlackBoard.RemoveObserver(key, onValueChanged);
            }
        }

        private void onValueChanged(BlackBoard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return this.query();
        }

        public override string ToString()
        {
            string keys = "";
            foreach (string key in this.keys)
            {
                keys += " " + key;
            }
            return Name + keys;
        }
    }
}