using System;
using Airhead.Runtime.Utility;
using Airhead.Runtime.Vitality;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Airhead.Runtime.Player
{
    public class PlayerGun : PlayerWeapon
    {
        public DamageArgs damage;
        public bool singleFire = false;
        public float fireRate = 180.0f;
        public int projectilesPerShot = 1;
        public float spread;
        public float maxRange = 100.0f;

        [Space]
        public int magazineSize;
        public float reloadTime;
        [ReadOnly] public int currentMagazine;
        
        [Space]
        public GameObject hitFX;
        public ParticleSystem trailFX;
        public float trailDensity = 16.0f;
        public Vector3 muzzleOffset;

        private bool shootFlag;
        private float lastFireTime;

        private ParticleSystem flash;

        public override string AmmoString => $"{currentMagazine}/{magazineSize}";
        public override bool IsReloading => currentMagazine < magazineSize;
        public override float ReloadPercent => currentMagazine < magazineSize ? (Time.time - lastFireTime) / reloadTime : 1.0f;

        public event System.Action ShootEvent; 

        protected override void Awake()
        {
            base.Awake();
            var viewport = transform.Find("Viewport");

            foreach (var t in viewport.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = ViewportLayer;
            }

            flash = viewport.Find<ParticleSystem>("Flash");
        }

        private void OnEnable()
        {
            currentMagazine = magazineSize;
        }

        private void Update()
        {
            if (singleFire)
            {
                if (Player.ShootAction.WasPressedThisFrame()) shootFlag = true;
            }
            else shootFlag = Player.ShootAction.IsPressed();
        }

        private void FixedUpdate()
        {
            if (shootFlag)
            {
                Shoot();
            }
            
            if (IsReloading && ReloadPercent >= 1.0f)
            {
                currentMagazine = magazineSize;
            }

            ResetFlags();
        }

        private void Shoot()
        {
            if (Time.time < lastFireTime + 60.0f / fireRate) return;
            if (currentMagazine <= 0) return;

            for (var i = 0; i < projectilesPerShot; i++)
            {
                var random = Random.insideUnitCircle;
                var direction = MainCam.transform.TransformDirection(random.x * spread, random.y * spread, 10.0f).normalized;
                var point = MainCam.transform.position;

                var ray = new Ray(point, direction);
                var end = ray.GetPoint(maxRange);
                if (Raycast(ray, out var hit, maxRange))
                {
                    end = hit.point;
                    ProcessHit(ray, hit);
                }
                
                if (trailFX)
                {
                    var start = transform.TransformPoint(muzzleOffset);
                    var vector = end - start;
                    var dist = vector.magnitude;
                    var step = 1.0f / trailDensity;
                    var dir = vector / dist;
                    
                    for (var d = 0.0f; d < dist; d += step)
                    {
                        trailFX.Emit(new ParticleSystem.EmitParams()
                        {
                            position = start + dir * d,
                        }, 1);
                    }
                }
            }

            if (flash) flash.Play();

            ShootEvent?.Invoke();

            currentMagazine--;
            lastFireTime = Time.time;
        }

        private void ProcessHit(Ray ray, RaycastHit hit)
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if ((Object)damageable)
            {
                damageable.Damage(new DamageInstance(damage, hit.point, ray.direction));
            }

            if (hitFX) Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(muzzleOffset, 0.04f);
        }

        private void ResetFlags()
        {
            shootFlag = false;
        }
    }
}