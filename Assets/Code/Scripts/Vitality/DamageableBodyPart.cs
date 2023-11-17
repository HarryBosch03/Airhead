using UnityEngine;

namespace Airhead.Runtime.Vitality
{
    [SelectionBase, DisallowMultipleComponent]
    public class DamageableBodyPart : MonoBehaviour, IDamageable
    {
        public BodyPart bodyPart;
        
        private HealthController health;

        private void Awake()
        {
            health = GetComponentInParent<HealthController>();
        }

        public void Damage(DamageInstance damage)
        {
            damage.locationalDamage = BodyPartScaleMap[(int)bodyPart];
            health.Damage(damage);
        }

        public static readonly float[] BodyPartScaleMap =
        {
            1.0f,
            0.75f,
            3.0f,
        };
        
        public enum BodyPart
        {
            Body = default,
            Limb,
            Head
        }
    }
}