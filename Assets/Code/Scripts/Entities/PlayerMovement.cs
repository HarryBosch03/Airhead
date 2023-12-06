using System;
using Airhead.Runtime.Utility;
using UnityEngine;

namespace Airhead.Runtime.Entities
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerMovement : MonoBehaviour
    {
        #region PROPERTIES

        public float moveSpeed = 12.0f;
        public float accelerationTime = 0.1f;
        public float jumpHeight = 2.2f;

        public float airMovement = 1.0f;
        public float upGravity = 2.0f;
        public float downGravity = 3.0f;
        public float stepHeight = 0.6f;

        public int maxAirJumps = 2;
        public int airJumpsLeft = 2;
        [Range(0.0f, 1.0f)]
        public float airJumpPenalty = 0.6f;

        [Range(0.0f, 1.0f)]
        public float heightSmoothing = 0.4f;

        [Space]
        [Range(0.0f, 1.0f)]
        public float slideMinSpeed = 0.2f;
        public float slideBoost;
        public float slideBoostDuration;
        public float slideBoostCooldown;
        [Range(0.0f, 1.0f)]
        public float slideFriction;
        public float slidingCameraHeight;

        #endregion

        [HideInInspector] public Vector3 moveInput;
        [HideInInspector] public bool jump;

        [HideInInspector] public Transform view;
        [HideInInspector] public Vector2 viewRotation;
        [HideInInspector] public Rigidbody body;

        [HideInInspector] public bool slideInput;
        public bool Sliding { get; private set; }
        private Vector3 slideDirection;
        private float lastSlideBoostTime;

        private float height;
        private RaycastHit groundHit;
        private CapsuleCollider movementCollider;

        public bool IsOnGround { get; private set; }

        public float Movement
        {
            get
            {
                var v = body.velocity;
                return Mathf.Sqrt(v.x * v.x + v.z * v.z) / moveSpeed;
            }
        }

        private Vector3 Gravity => Physics.gravity * (body.velocity.y > 0.0f ? upGravity : downGravity);

        private void Awake()
        {
            body = gameObject.GetOrAddComponent<Rigidbody>();
            movementCollider = GetComponent<CapsuleCollider>();
            view = transform.Find("View");
        }

        private void OnEnable()
        {
            body.isKinematic = false;
            body.useGravity = true;
            movementCollider.enabled = false;
        }

        private void OnDisable()
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = false;
            body.useGravity = false;
        }

        private void FixedUpdate()
        {
            LookForGround();
            Move();
            Jump();
            ApplyGravity();
            Collide();
        }

        private void Collide()
        {
            movementCollider.enabled = true;
            var bounds = movementCollider.bounds;
            var list = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity);

            foreach (var other in list)
            {
                if (other.transform.IsChildOf(transform)) continue;

                if (!Physics.ComputePenetration(
                        movementCollider,
                        movementCollider.transform.position,
                        movementCollider.transform.rotation,
                        other,
                        other.transform.position,
                        other.transform.rotation,
                        out var normal,
                        out var distance)
                   ) continue;

                normal.Normalize();
                body.position += normal * distance;
                body.velocity += normal * Mathf.Max(0.0f, Vector3.Dot(-body.velocity, normal));
            }

            movementCollider.enabled = false;
        }

        private void ApplyGravity()
        {
            if (!body.useGravity) return;
            body.AddForce(Gravity - Physics.gravity, ForceMode.Acceleration);
        }

        private void Jump()
        {
            if (IsOnGround) airJumpsLeft = maxAirJumps;

            if (!jump) return;

            if (!IsOnGround)
            {
                if (airJumpsLeft > 0) AirJump();
                return;
            }

            var force = GetJumpForce(jumpHeight);
            body.AddForce(Vector3.up * (force - Mathf.Min(0.0f, body.velocity.y)), ForceMode.VelocityChange);
        }

        private void AirJump()
        {
            var velocity = Vector3.up * GetJumpForce(jumpHeight * (1.0f - airJumpPenalty));
            velocity += Vector3.ClampMagnitude(new Vector3(moveInput.x, 0.0f, moveInput.z), 1.0f) * moveSpeed;

            body.velocity = velocity;
            airJumpsLeft--;
        }

        private float GetJumpForce(float jumpHeight) => Mathf.Sqrt(2.0f * -Physics.gravity.y * upGravity * jumpHeight);

        private void LookForGround()
        {
            var wasOnGround = IsOnGround;
            var ray = new Ray(body.position + Vector3.up, Vector3.down);
            var castDistance = wasOnGround ? 1.0f + stepHeight : 1.0f;

            IsOnGround = false;
            if (body.velocity.y < float.Epsilon)
            {
                var hits = Physics.RaycastAll(ray, castDistance);
                foreach (var hit in hits)
                {
                    if (hit.collider.transform.IsChildOf(transform)) continue;

                    IsOnGround = true;
                    groundHit = hit;
                    break;
                }
            }

            if (!IsOnGround) height = body.position.y;
            else height = Mathf.Lerp(height, groundHit.point.y, heightSmoothing);

            if (!IsOnGround) return;

            body.position = new Vector3(body.position.x, height, body.position.z);
            body.velocity = new Vector3(body.velocity.x, Mathf.Max(body.velocity.y, 0.0f), body.velocity.z);

            if (groundHit.rigidbody) body.position += groundHit.rigidbody.velocity * Time.deltaTime;
        }

        private void Move()
        {
            if (slideInput || Sliding)
            {
                Slide();
                return;
            }

            var target = Vector3.ClampMagnitude(moveInput, 1.0f) * moveSpeed;
            Vector3 force;

            if (IsOnGround)
            {
                var acceleration = 2.0f / accelerationTime;
                force = (target - body.velocity) * acceleration;
            }
            else
            {
                var acceleration = airMovement * moveInput.magnitude;
                force = (target - body.velocity) * acceleration;
            }

            force.y = 0.0f;
            force = Vector3.ClampMagnitude(force, moveSpeed * 2.0f / accelerationTime);
            body.AddForce(force, ForceMode.Acceleration);
        }

        private void Slide()
        {
            var v = body.velocity;
            var speed = Mathf.Sqrt(v.x * v.x + v.z * v.z);

            if (!IsOnGround || speed / moveSpeed < slideMinSpeed)
            {
                Sliding = false;
                return;
            }

            if (!Sliding)
            {
                Sliding = true;
                slideDirection = new Vector3(body.velocity.x, 0.0f, body.velocity.z).normalized;
                if (slideDirection.magnitude < 0.5f)
                {
                    Sliding = false;
                    return;
                }

                if (Time.time - lastSlideBoostTime > slideBoostDuration + slideBoostCooldown)
                {
                    lastSlideBoostTime = Time.time;
                    body.velocity = new Vector3
                    {
                        x = slideDirection.x * moveSpeed * (1.0f + slideBoost),
                        z = slideDirection.z * moveSpeed * (1.0f + slideBoost),
                        y = body.velocity.y,
                    };
                }
            }

            var friction = Time.time - lastSlideBoostTime < slideBoostDuration ? 0.0f : slideFriction;
            var force = -new Vector3(body.velocity.x, 0.0f, body.velocity.z) * friction;
            body.AddForce(force, ForceMode.VelocityChange);

            if (!slideInput)
            {
                Sliding = false;
            }
        }

        private void Update()
        {
            viewRotation.x %= 360.0f;
            viewRotation.y = Mathf.Clamp(viewRotation.y, -90.0f, 90.0f);

            transform.rotation = Quaternion.Euler(0.0f, viewRotation.x, 0.0f);

            view.position = transform.position + Vector3.up * (Sliding ? slidingCameraHeight : 1.8f);
            view.rotation = Quaternion.Euler(-viewRotation.y, viewRotation.x, 0.0f);
        }
    }
}