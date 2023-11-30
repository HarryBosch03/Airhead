using UnityEngine;

namespace Airhead.Runtime.Core.StateMachines
{
    public class StateMachine<T>
    {
        public T Target { get; set; }

        public SmState<T> CurrentState { get; private set; }
        private SmState<T> nextState;

        public void Update() => CurrentState?.Update();
        public void LateUpdate() => CurrentState?.LateUpdate();
        public void FixedUpdate()
        {
            if (nextState != null)
            {
                CurrentState?.Exit();
                CurrentState = nextState;
                CurrentState?.Enter(this);
                nextState = null;
            }

            CurrentState?.FixedUpdate();
        }
        
        public void ChangeState(SmState<T> state)
        {
            nextState = state;
        }

        public override string ToString()
        {
            var stateName = CurrentState?.GetType().Name ?? "No State";
            return $"State Machine [{stateName}]";
        }
    }
}