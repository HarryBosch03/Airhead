using System;
using Airhead.Runtime.Utility;
using Airhead.Runtime.Vitality;
using UnityEngine;

namespace Airhead.Runtime.Level
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ExplodingBarrel : MonoBehaviour, IDamageable
    {
        public Explosion explosion;
        public int health = 10;
        public float respawnDelay = 5.0f;

        private bool dead;
        private float age;
        private Animator model;

        private void Awake()
        {
            model = transform.Find<Animator>("Model");
        }

        private void Update()
        {
            age += Time.deltaTime;
            if (dead && age >= 0.0f)
            {
                dead = false;
            }
            model.SetBool("dead", dead);
        }

        public void Damage(DamageInstance damage)
        {
            if (dead) return;
            health -= damage.Calculate();
            if (health <= 0) Explode();
        }

        private void Explode()
        {
            dead = true;
            age = -respawnDelay;
            
            explosion.At(transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            explosion.DrawGizmos(transform.position);
        }
    }
}
