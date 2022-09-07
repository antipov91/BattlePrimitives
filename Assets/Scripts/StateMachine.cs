namespace BattlePrimitives.StateMachine
{
    public class StateMachine<T> where T : class
    {
        private State<T> currentState;
        public State<T> CurrentState
        {
            get { return currentState; }
            set
            {
                currentState?.Deactivate();
                currentState = value;
                currentState.Activate();
            }
        }
    }
}