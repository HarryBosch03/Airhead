using System;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    [DefaultExecutionOrder(100)]
    public class ProceduralWeaponAnimator : MonoBehaviour
    {
        public Vector3 center;
        public float lag;

        private Camera mainCamera;
        private Vector3 position;
        private Quaternion rotation;

        private Rigidbody body;

        private void Awake()
        {
            mainCamera = Camera.main;
            body = GetComponentInParent<Rigidbody>();
        }

        private void LateUpdate()
        {
            ResetPose();

            Lag();
            
            SetPose();
        }

        private void Lag()
        {
            position -= body.velocity * lag;
        }

        private void ResetPose()
        {
            position = mainCamera.transform.position;
            rotation = mainCamera.transform.rotation;
        }

        private void SetPose()
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localPosition = transform.localRotation * transform.localPosition + transform.localPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.TransformPoint(center), 0.03f);
        }
    }
}