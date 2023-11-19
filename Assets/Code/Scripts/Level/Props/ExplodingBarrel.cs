using Airhead.Runtime.Utility;
using UnityEngine;

namespace Airhead.Runtime.Level.Props
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ExplodingBarrel : DamageableProp
    {
        public Explosion explosion;
        
        protected override void Die()
        {
            base.Die();
            explosion.At(transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            explosion.DrawGizmos(transform.position);
        }
    }
}
