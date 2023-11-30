using TMPro;
using UnityEngine;

namespace Airhead.Runtime.Vitality
{
    [SelectionBase, DisallowMultipleComponent]
    public class Dummy : HealthController
    {
        public float delay = 2.0f;
        public TMP_Text damageNumber;
        public ParticleSystem visualiser;

        private int damageAccumulator;
        private int frameDamageAccumulator;
        private int damageFrame;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (damageNumber) damageNumber.text = string.Empty;
        }

        private void Update()
        {
            currentHealth = int.MaxValue;
            maxHealth = int.MaxValue;

            if (damageNumber)
            {
                if (Time.time > LastDamageTime + delay)
                {
                    damageAccumulator = 0;
                    frameDamageAccumulator = 0;
                    damageNumber.text = string.Empty;
                }
                else
                {
                    damageNumber.text = $"{frameDamageAccumulator:N0} [{damageAccumulator:N0}]";
                }
            }
        }

        public override void Damage(DamageInstance instance)
        {
            var damage = instance.EvaluateDamage();
            damageAccumulator += damage;
            if (damageFrame != Time.frameCount) frameDamageAccumulator = 0;

            frameDamageAccumulator += damage;
            damageFrame = Time.frameCount;

            if (visualiser)
            {
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.position = instance.point;
                visualiser.Emit(emitParams, 1);
            }

            base.Damage(instance);
        }

        public override void Die(DamageInstance instance) { }
    }
}