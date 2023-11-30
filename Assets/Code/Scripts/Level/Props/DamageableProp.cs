using System;
using System.Collections;
using Airhead.Runtime.Vitality;
using UnityEngine;

namespace Airhead.Runtime.Level.Props
{
    [SelectionBase, DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class DamageableProp : Prop, IDamageable
    {
        public int health = 10;
        public float respawnTime = 5.0f;

        [Space]
        public bool animateIn = true;
        public float animationDuration = 0.6f;
        public AnimationCurve animationCurve;

        private bool dead;
        private DamageableProp next;
        private Rigidbody body;

        public static event Action<DamageableProp> PropDestroyedEvent;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            next = Instantiate(this, transform.parent);
            next.gameObject.SetActive(false);

            if (animateIn)
            {
                StartCoroutine(SpawnAnimation());
            }
        }

        private IEnumerator SpawnAnimation()
        {
            body.isKinematic = true;
            var p = 0.0f;
            while (p < 1.0f)
            {
                transform.localScale = Vector3.one * animationCurve.Evaluate(p);
                p += Time.deltaTime / animationDuration;
                yield return null;
            }
            transform.localScale = Vector3.one;
            
            body.isKinematic = false;
        }

        public virtual void Damage(DamageInstance damage)
        {
            if (dead) return;
            health -= damage.EvaluateDamage();
            if (health <= 0) Die();
        }

        protected virtual void Die()
        {
            dead = true;
            Destroy(gameObject);
            
            PropDestroyedEvent?.Invoke(this);
            PropManager.Instance.SpawnWithDelay(next);
        }
    }
}