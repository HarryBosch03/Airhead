using System;
using Airhead.Runtime.Vitality;
using UnityEngine;

namespace Airhead.Runtime.Level
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class KillPlane : MonoBehaviour
    {
        public DamageArgs args;
        
        private void FixedUpdate()
        {
            var list = FindObjectsOfType<Rigidbody>();
            foreach (var e in list)
            {
                var dot = Vector3.Dot(e.position - transform.position, transform.up);
                if (dot > 0.0f) continue;
                
                var damageable = e.GetComponent<IDamageable>();
                if (damageable == null) continue;
                damageable.Damage(new DamageInstance(args, e.position, transform.up));
            }
        }
    }
}
