using System.Collections.Generic;
using Airhead.Runtime.Vitality;
using UnityEngine;

namespace Airhead.Runtime.Utility
{
    [System.Serializable]
    public class Explosion
    {
        public DamageArgs damage;
        public AnimationCurve falloff;
        public float range;
        public GameObject fxPrefab;

        public void At(Vector3 position)
        {
            var hitList = new List<IDamageable>();

            const int castResolution = 32;
            for (var x = 0; x < castResolution * 2; x++)
            for (var y = 0; y < castResolution - 1; y++)
            {
                var xa = x / (castResolution * 2.0f) * 2.0f * Mathf.PI;
                var ya = (y + 1.0f) / castResolution * Mathf.PI;

                var direction = new Vector3(Mathf.Cos(xa), 0.0f, Mathf.Sin(xa));
                direction = direction * Mathf.Cos(ya) + Vector3.up * Mathf.Sin(ya);
                var ray = new Ray(position, direction);
                var end = ray.GetPoint(range);
                var color = Color.gray.Alpha(0.2f);
                if (Physics.Raycast(ray, out var hit, range))
                {
                    end = hit.point;
                    color = Color.red;

                    var target = hit.collider.GetComponentInParent<IDamageable>();

                    if (!hitList.Contains(target))
                    {
                        hitList.Add(target);

                        if ((Object)target)
                        {
                            var args = damage.Clone();
                            args.damage = Mathf.Ceil(args.damage * falloff.Evaluate(hit.distance / range));
                            target.Damage(new DamageInstance(args, hit.point, direction));
                        }
                    }
                }

                Debug.DrawLine(position, end, color, 5.0f);
            }

            if (fxPrefab) Object.Instantiate(fxPrefab, position, Quaternion.identity);
        }

        public void DrawGizmos(Vector3 center)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, range);
        }
    }
}