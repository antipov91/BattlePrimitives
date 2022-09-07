namespace BattlePrimitives.StateMachine
{
    public abstract class State<T> where T : class
    {
        protected T context;
        protected StateMachine<T> stateMachine;

        public State(T context, StateMachine<T> stateMachine)
        {
            this.context = context;
            this.stateMachine = stateMachine;
        }

        public virtual void Activate() { }

        public virtual void Update() { }

        public virtual void Deactivate() { }
    }
}