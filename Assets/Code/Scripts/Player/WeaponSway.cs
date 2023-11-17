using System;
using Unity.Collections;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    public class WeaponSway : MonoBehaviour
    {
        public float max = 8.0f;
        public float response = 0.5f;
        public float invSmoothing = 15.0f;
        [Range(0.0f, 1.0f)]
        [ReadOnly]
        public float evaluation;
        public Vector3 rotationOffset;
        public Vector3 originOffset;

        private Vector2 lastRotation;
        private Vector2 smoothedDelta;

        private void OnEnable()
        {
            if (!transform.parent) enabled = false;
        }

        private void LateUpdate()
        {
            var cameraRotation = new Vector2(transform.parent.eulerAngles.y, -transform.parent.eulerAngles.x);
            var delta = componentWise(x => Mathf.DeltaAngle(x(cameraRotation), x(lastRotation)));
            lastRotation = cameraRotation;

            smoothedDelta = componentWise(x => Mathf.LerpAngle(x(smoothedDelta), x(delta), invSmoothing * Time.deltaTime));
            
            var offset = componentWise(x => Mathf.Atan(x(smoothedDelta) * response) * 2.0f * max / Mathf.PI);
            evaluation = offset.magnitude / max;
            
            transform.localRotation = Quaternion.Euler(-offset.y, offset.x, 0.0f) * Quaternion.Euler(rotationOffset);
            transform.localPosition = originOffset + transform.localRotation * -originOffset;

            Vector2 componentWise(Func<Func<Vector2, float>, float> operation)
            {
                return new Vector2(operation(v => v.x), operation(v => v.y));
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(originOffset, 0.06f);
        }
    }
}