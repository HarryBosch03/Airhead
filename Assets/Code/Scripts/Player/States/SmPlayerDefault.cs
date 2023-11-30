using System;
using Airhead.Runtime.Core.StateMachines;

namespace Airhead.Runtime.Player.States
{
    [Serializable]
    public class SmPlayerDefault : SmState<PlayerAvatar>
    {
        protected override void OnEnter()
        {
            Target.Movement.enabled = true;
            Target.Health.enabled = true;
            Target.WeaponManager.enabled = true;
            Target.PlayerUI.enabled = true;
            Target.Respawn();
        }

        public override void FixedUpdate()
        {
            if (Target.Health.Dead)
            {
                StateMachine.ChangeState(new SmPlayerDead());
            }
        }

        protected override void OnExit()
        {
            Target.Movement.enabled = false;
            Target.Health.enabled = false;
            Target.WeaponManager.enabled = false;
            Target.PlayerUI.enabled = false;
        }
    }
}