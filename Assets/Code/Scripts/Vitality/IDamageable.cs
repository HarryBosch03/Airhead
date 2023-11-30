using System.Collections.Generic;

namespace Airhead.Runtime.Vitality
{
    public interface IDamageable
    {
        void Damage(DamageInstance damage);

        public static readonly List<IDamageable> All = new();

        public static void Register(IDamageable damageable) => All.Add(damageable);

        public static void Deregister(IDamageable damageable) => All.Remove(damageable);
    }
}