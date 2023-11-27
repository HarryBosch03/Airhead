using System;
using Airhead.Runtime.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Airhead.Runtime.Player
{
    [DefaultExecutionOrder(100)]
    public class ProceduralWeaponAnimator : MonoBehaviour
    {
        [Space]
        public Vector3 basisPosition;
        public Vector3 basisRotation;
        
        [Space]
        public float velocityLag = 0.002f;
        public float rotationLag = 0.002f;

        [Space]
        public Vector2 stepAmplitude;
        public float stepFrequency;

        [Space]
        public Vector3 shootForce;
        public Vector3 shootTorque;
        public float linearShootSpring;
        public float linearShootDamping;
        public float angularShootSpring;
        public float angularShootDamping;

        [Space]
        public float smoothing;

        private Vector3 shootPosition;
        private Vector3 shootLinearVelocity;
        
        private Vector3 shootRotation;
        private Vector3 shootAngularVelocity;
        
        private Camera mainCamera;
        private Vector3 position;
        private Quaternion rotation;
        private Quaternion lastCameraRotation;
        
        private float stepDistance;
        private float stepMovement;

        private Vector3 smoothedPosition;
        private Quaternion smoothedRotation;

        private BipedController biped;
        private PlayerGun gun;

        private void Awake()
        {
            mainCamera = Camera.main;
            biped = GetComponentInParent<BipedController>();
        }

        private void OnEnable()
        {
            gun = GetComponent<PlayerGun>();
            gun.ShootEvent += OnShoot;
        }

        private void OnDisable()
        {
            gun.ShootEvent -= OnShoot;
        }
        
        private void OnShoot()
        {
            shootLinearVelocity += new Vector3(shootForce.x * Random.Range(-1.0f, 1.0f), shootForce.y, shootForce.z);
            shootAngularVelocity += new Vector3(shootTorque.x, shootTorque.y * Random.Range(-1.0f, 1.0f), shootTorque.z * Random.Range(-1.0f, 1.0f));
        }

        private void FixedUpdate()
        {
            var movement = biped.IsOnGround ? biped.Movement : 0.0f;
            stepMovement = movement;
            stepDistance += movement * stepFrequency * Time.deltaTime;
        }

        private void LateUpdate()
        {
            ResetPose();

            Lag();
            Step();
            Recoil();

            SetPose();
        }

        private void Recoil()
        {
            var force = -shootPosition * linearShootSpring - shootLinearVelocity * linearShootDamping;
            var torque = -shootRotation * angularShootSpring - shootAngularVelocity * angularShootDamping;
            
            shootPosition += shootLinearVelocity * Time.deltaTime;
            shootLinearVelocity += force * Time.deltaTime;

            shootRotation += shootAngularVelocity * Time.deltaTime;
            shootAngularVelocity += torque * Time.deltaTime;

            position += mainCamera.transform.TransformVector(shootPosition);
            rotation = Quaternion.Euler(shootRotation) * rotation;
        }

        private void Step()
        {
            var x = Mathf.Cos(stepDistance * Mathf.PI) * stepMovement * stepAmplitude.x;
            var y = Mathf.Abs(Mathf.Sin(stepDistance * Mathf.PI)) * stepMovement * stepAmplitude.y;

            position += mainCamera.transform.rotation * new Vector3(x, y, 0.0f);
        }

        private void Lag()
        {
            position -= biped.body.velocity * velocityLag;

            var cameraRotation = mainCamera.transform.rotation;
            var deltaRotation = cameraRotation * Quaternion.Inverse(lastCameraRotation);
            rotation = Quaternion.SlerpUnclamped(Quaternion.identity, deltaRotation, -rotationLag) * rotation;
            
            lastCameraRotation = cameraRotation;
        }

        private void ResetPose()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }

        private void SetPose()
        {
            var t = Time.deltaTime / Mathf.Max(Time.unscaledDeltaTime, smoothing);
            smoothedPosition = Vector3.Lerp(smoothedPosition, position, t);
            smoothedRotation = Quaternion.Slerp(smoothedRotation, rotation, t);

            transform.position = mainCamera.transform.TransformPoint(basisPosition) + smoothedPosition;
            transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(basisRotation) * smoothedRotation;
        }
    }
}