using System;
using System.Linq;
using Airhead.Runtime.Vitality;
using UnityEngine;
using UnityEngine.Serialization;

namespace Airhead.Runtime.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public DamageArgs damage;
        public float startSpeed = 100.0f;
        public float distance = 20.0f;
        public GameObject hitFxPrefab;
        public float trailSustain = 2.0f;

        private float age;
        private float lifetime;
        private Vector3 velocity;
        private Vector3 force;
        private Transform trail;

        public GameObject owner;

        public void Spawn(Vector3 position, Vector3 direction, GameObject owner)
        {
            var instance = Instantiate(this, position, Quaternion.LookRotation(direction));
            instance.owner = owner;
        }

        private void Awake()
        {
            trail = transform.Find("Trail");
            lifetime = distance / startSpeed;
        }

        private void Start()
        {
            velocity = transform.forward * startSpeed;
        }

        private void FixedUpdate()
        {
            Collide();
            Integrate();

            Age();
        }

        private void Age()
        {
            age += Time.deltaTime;
            if (age > lifetime)
            {
                Destroy();
            }
        }

        private void Collide()
        {
            var step = velocity.magnitude * Time.deltaTime;

            var ray = new Ray(transform.position, velocity);
            var hits = Physics.RaycastAll(ray, step * 1.01f).OrderBy(e => e.distance);
            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(owner.transform)) continue;
                
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(new DamageInstance(damage, hit.point, ray.direction));
                }

                if (hitFxPrefab) Instantiate(hitFxPrefab, hit.point, Quaternion.LookRotation(Vector3.Reflect(ray.direction, hit.normal)));
                Destroy();
                break;
            }
        }

        public void Destroy()
        {
            trail.SetParent(null);
            Destroy(trail.gameObject, trailSustain);
            Destroy(gameObject);
        }

        private void Integrate()
        {
            transform.position += velocity * Time.deltaTime;
            velocity += force * Time.deltaTime;
            force = Physics.gravity;
        }
    }
}