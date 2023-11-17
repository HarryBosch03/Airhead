using System;
using Airhead.Runtime.Entities;
using Airhead.Runtime.Vitality;
using UnityEngine;
using UnityEngine.Serialization;

namespace Airhead.Runtime.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        public int damageTaken;
        public float damageForce = 0.5f;
        public float damageForceHealthPenalty = 0.3f;
        public float maxDamageForce = 100.0f;

        private BipedController biped;

        private void Awake()
        {
            biped = GetComponent<BipedController>();
        }

        public void Damage(DamageInstance instance)
        {
            var damage = instance.Calculate();
            damageTaken += damage;

            var force = damage * (damageForce + damageForceHealthPenalty * damageTaken / 100.0f);
            force = Mathf.Min(maxDamageForce, force);
            biped.body.AddForce(instance.direction * force, ForceMode.VelocityChange);
        }
    }
}
