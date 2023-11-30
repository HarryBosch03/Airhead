using System;
using Airhead.Runtime.Core.StateMachines;
using Airhead.Runtime.Entities;
using Airhead.Runtime.Player.States;
using Airhead.Runtime.Vitality;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class PlayerAvatar : MonoBehaviour
    {
        private StateMachine<PlayerAvatar> stateMachine = new();
        public PlayerController Controller { get; set; }
        public BipedController Biped { get; private set; }
        public PlayerHealth Health { get; private set; }
        public PlayerWeaponManager WeaponManager { get; private set; }
        public PlayerUI PlayerUI { get; private set; }

        private void Awake()
        {
            Biped = GetComponent<BipedController>();
            Health = GetComponent<PlayerHealth>();
            WeaponManager = GetComponent<PlayerWeaponManager>();
            PlayerUI = GetComponent<PlayerUI>();
        }

        private void OnEnable()
        {
            stateMachine.Target = this;
            Biped.enabled = false;
            Health.enabled = false;
            WeaponManager.enabled = false;
            PlayerUI.enabled = false;
            
            stateMachine.ChangeState(new SmPlayerDefault());
        }

        private void Update() => stateMachine.Update();
        private void FixedUpdate() => stateMachine.FixedUpdate();
        private void LateUpdate() => stateMachine.LateUpdate();

        public void Respawn()
        {
            Health.Respawn();
        }
    }
}