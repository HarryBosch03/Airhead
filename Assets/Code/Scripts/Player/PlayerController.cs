using System;
using System.Collections.Generic;
using Airhead.Runtime.Entities;
using FishNet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhead.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public PlayerAvatar avatarPrefab;
        public float mouseSensitivity = 0.3f;

        [Space]
        public float cameraMoveDutchMax;
        public float cameraMoveDutchResponse;
        public float dutchSmoothing;

        private Camera mainCam;
        private float height;
        private bool jumpFlag;
        private float dutch;
        private float smoothedDutch;

        public InputAction MoveAction { get; private set; }
        public InputAction JumpAction { get; private set; }
        public InputAction ShootAction { get; private set; }
        public PlayerAvatar Avatar { get; private set; }

        public static readonly List<PlayerController> All = new();

        private void Awake()
        {
            mainCam = Camera.main;

            MoveAction = inputAsset.FindAction("Move");
            JumpAction = inputAsset.FindAction("Jump");
            ShootAction = inputAsset.FindAction("Shoot");
        }

        private void OnEnable()
        {
            All.Add(this);

            inputAsset.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            inputAsset.Disable();
            Cursor.lockState = CursorLockMode.None;

            All.Remove(this);
        }

        private void Start() { Spawn(); }

        private void Spawn()
        {
            var sp = SpawnPoint.GetSpawnPoint();
            
            Avatar = Instantiate(avatarPrefab, sp.position, sp.rotation);
            InstanceFinder.ServerManager.Spawn(Avatar.gameObject);
        }

        private void Update()
        {
            if (Avatar)
            {
                if (ShootAction.WasPerformedThisFrame()) Avatar.WeaponManager.UseFlag = true;
                if (ShootAction.WasReleasedThisFrame()) Avatar.WeaponManager.UseFlag = false;
            }
        }

        private void FixedUpdate()
        {
            if (Avatar)
            {
                var moveInput = MoveAction.ReadValue<Vector2>();
                Avatar.Biped.moveInput = Avatar.transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);

                Avatar.Biped.jump = jumpFlag;

                var velocity = Avatar.Biped.body.velocity;
                var speed = -Vector3.Dot(velocity, transform.right);
                dutch = Mathf.Atan(speed * cameraMoveDutchResponse) * 2.0f / Mathf.PI * cameraMoveDutchMax;
            }

            jumpFlag = false;
        }

        private void LateUpdate()
        {
            if (Avatar)
            {
                var delta = Vector2.zero;
                delta += Mouse.current.delta.ReadValue() * mouseSensitivity * Mathf.Min(1.0f, Time.timeScale);
                Avatar.Biped.viewRotation += delta;

                smoothedDutch = Mathf.Lerp(smoothedDutch, dutch, Time.deltaTime / Mathf.Max(Time.deltaTime, dutchSmoothing));

                mainCam.transform.position = Avatar.Biped.view.position;
                mainCam.transform.rotation = Avatar.Biped.view.rotation * Quaternion.Euler(0.0f, 0.0f, smoothedDutch);
            }

            if (JumpAction.WasPressedThisFrame()) jumpFlag = true;
        }
    }
}