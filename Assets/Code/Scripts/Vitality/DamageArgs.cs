
namespace Airhead.Runtime.Vitality
{
    [System.Serializable]
    public class DamageArgs
    {
        public float damage = 1;
        public float force = 1.0f;
        public bool lethal = false;

        public DamageArgs Clone() => (DamageArgs)MemberwiseClone();
    }
}