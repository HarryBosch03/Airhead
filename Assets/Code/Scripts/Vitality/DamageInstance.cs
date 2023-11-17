
using UnityEngine;

namespace Airhead.Runtime.Vitality
{
    public class DamageInstance
    {
        public DamageArgs args;
        public Vector3 point;
        public Vector3 direction;
        public float locationalDamage = 1.0f;

        public DamageInstance(DamageArgs args, Vector3 point, Vector3 direction)
        {
            this.args = args;
            this.point = point;
            this.direction = direction;
        }
        
        
        public int Calculate()
        {
            var damage = args.damage;

            if (!args.ignoreLocationalDamage) damage *= locationalDamage; 
            
            return Mathf.Max(1, Mathf.FloorToInt(damage));
        }
    }
}