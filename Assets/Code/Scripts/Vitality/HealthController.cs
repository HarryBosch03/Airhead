using UnityEngine;

namespace Airhead.Runtime.Vitality
{
    [SelectionBase, DisallowMultipleComponent]
    public class HealthController : MonoBehaviour, IDamageable
    {
        public int currentHealth;
        public int maxHealth;

        private Rigidbody body;

        public float LastDamageTime { get; private set; }

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody>();
        }
        
        protected virtual void OnEnable()
        {
            currentHealth = maxHealth;
        }

        public virtual void Damage(DamageInstance instance)
        {
            var damage = Mathf.Max(1, Mathf.FloorToInt(instance.EvaluateDamage()));
            currentHealth -= damage;

            if (body)
            {
                body.AddForce(instance.EvaluateForce());
            }
            
            LastDamageTime = Time.time;
            
            if (currentHealth <= 0)
            {
                Die(instance);
            }
        }

        public virtual void Die(DamageInstance instance)
        {
            gameObject.SetActive(false);
        }
    }
}