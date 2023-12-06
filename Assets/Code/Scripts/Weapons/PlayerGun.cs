using System;
using Airhead.Runtime.Utility;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Airhead.Runtime.Weapons
{
    public class PlayerGun : PlayerWeapon
    {
        public Projectile projectile;
        public bool singleFire = false;
        public float fireRate = 180.0f;
        public int projectilesPerShot = 1;
        public float spread;

        [Space]
        public int magazineSize;
        public float reloadTime;
        [ReadOnly] public int currentMagazine;
        
        [Space]
        public Vector3 muzzleOffset;

        private float lastFireTime;

        private ParticleSystem flash;

        public override string AmmoString => $"{currentMagazine}/{magazineSize}";
        public override bool IsReloading => currentMagazine < magazineSize;
        public override float ReloadPercent => currentMagazine < magazineSize ? (Time.time - lastFireTime) / reloadTime : 1.0f;

        public event Action ShootEvent; 

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

        private void FixedUpdate()
        {
            if (UseFlag)
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

                projectile.Spawn(point, direction, Player.gameObject);
            }

            if (flash) flash.Play();

            ShootEvent?.Invoke();

            currentMagazine--;
            lastFireTime = Time.time;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(muzzleOffset, 0.04f);
        }

        private void ResetFlags()
        {
            if (singleFire) UseFlag = false;
        }
    }
}