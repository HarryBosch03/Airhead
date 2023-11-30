using Airhead.Runtime.Entities;
using Airhead.Runtime.Vitality;
using UnityEngine;

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

        private PlayerMovement biped;

        public bool Dead { get; private set; }
        
        private void Awake()
        {
            biped = GetComponent<PlayerMovement>();
        }

        public void Damage(DamageInstance instance)
        {
            if (Dead) return;
            
            var damage = instance.EvaluateDamage();
            damageTaken += damage;

            var penalty = 1.0f + damageForceHealthPenalty * damageTaken / 100.0f;
            biped.body.AddForce(instance.EvaluateForce() * penalty);

            if (instance.args.lethal)
            {
                Die(instance);
            }
        }

        public void Die(DamageInstance instance)
        {
            Dead = true;
        }

        public void Respawn()
        {
            Dead = false;
            
            var sp = SpawnPoint.GetSpawnPoint();
            var body = biped.body;

            body.position = sp.position;
            body.rotation = sp.rotation;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            
            damageTaken = 0;
        }
    }
}
