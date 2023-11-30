using System.Collections.Generic;

namespace Airhead.Runtime.Core.StateMachines
{
    public abstract class SmState<T>
    {
        public T Target => StateMachine.Target;
        public StateMachine<T> StateMachine { get; private set; }

        protected virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void LateUpdate() { }
        protected virtual void OnExit() { }

        public void Enter(StateMachine<T> sm)
        {
            StateMachine = sm;
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
            StateMachine = null;
        }
    }
}