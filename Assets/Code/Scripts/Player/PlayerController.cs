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

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction shootAction;
        private InputAction slideAction;
        public PlayerAvatar Avatar { get; private set; }

        public static readonly List<PlayerController> All = new();

        private void Awake()
        {
            mainCam = Camera.main;

            moveAction = inputAsset.FindAction("Move");
            jumpAction = inputAsset.FindAction("Jump");
            shootAction = inputAsset.FindAction("Shoot");
            slideAction = inputAsset.FindAction("Slide");
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
                if (shootAction.WasPerformedThisFrame()) Avatar.WeaponManager.UseFlag = true;
                if (shootAction.WasReleasedThisFrame()) Avatar.WeaponManager.UseFlag = false;
            }
        }

        private void FixedUpdate()
        {
            if (Avatar)
            {
                var moveInput = moveAction.ReadValue<Vector2>();
                Avatar.Movement.moveInput = Avatar.transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);

                Avatar.Movement.jump = jumpFlag;
                Avatar.Movement.slideInput = slideAction.IsPressed();

                var velocity = Avatar.Movement.body.velocity;
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
                Avatar.Movement.viewRotation += delta;

                smoothedDutch = Mathf.Lerp(smoothedDutch, dutch, Time.deltaTime / Mathf.Max(Time.deltaTime, dutchSmoothing));

                mainCam.transform.position = Avatar.Movement.view.position;
                mainCam.transform.rotation = Avatar.Movement.view.rotation * Quaternion.Euler(0.0f, 0.0f, smoothedDutch);
            }

            if (jumpAction.WasPressedThisFrame()) jumpFlag = true;
        }
    }
}