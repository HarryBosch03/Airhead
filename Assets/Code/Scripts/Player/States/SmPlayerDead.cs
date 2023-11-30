using Airhead.Runtime.Core.StateMachines;
using UnityEngine;

namespace Airhead.Runtime.Player.States
{
    public class SmPlayerDead : SmState<PlayerAvatar>
    {
        private float timer;

        protected override void OnEnter()
        {
            timer = 0.0f;
        }

        public override void FixedUpdate()
        {
            timer += Time.deltaTime;
            if (timer > 2.0f)
            {
                StateMachine.ChangeState(new SmPlayerDefault());
            }
        }
    }
}