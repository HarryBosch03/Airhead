using Airhead.Runtime.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhead.Runtime.Player
{
    [RequireComponent(typeof(BipedController))]
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset inputAsset;
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
        public BipedController Biped { get; private set; }
        
        private void Awake()
        {
            mainCam = Camera.main;
            Biped = GetComponent<BipedController>();

            MoveAction = inputAsset.FindAction("Move");
            JumpAction = inputAsset.FindAction("Jump");
            ShootAction = inputAsset.FindAction("Shoot");
        }

        private void OnEnable()
        {
            inputAsset.Enable();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            inputAsset.Disable();
            Cursor.lockState = CursorLockMode.None;
        }

        private void FixedUpdate()
        {
            var moveInput = MoveAction.ReadValue<Vector2>();
            Biped.moveInput = transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);
            
            Biped.jump = jumpFlag;
            jumpFlag = false;
            
            var velocity = Biped.body.velocity;
            var speed = -Vector3.Dot(velocity, transform.right);
            dutch = Mathf.Atan(speed * cameraMoveDutchResponse) * 2.0f / Mathf.PI * cameraMoveDutchMax;
        }

        private void LateUpdate()
        {
            var delta = Vector2.zero;
            delta += Mouse.current.delta.ReadValue() * mouseSensitivity * Mathf.Min(1.0f, Time.timeScale);
            Biped.viewRotation += delta;

            smoothedDutch = Mathf.Lerp(smoothedDutch, dutch, Time.deltaTime / Mathf.Max(Time.deltaTime, dutchSmoothing));
            
            mainCam.transform.position = Biped.view.position;
            mainCam.transform.rotation = Biped.view.rotation * Quaternion.Euler(0.0f, 0.0f, smoothedDutch);

            if (JumpAction.WasPressedThisFrame()) jumpFlag = true;
        }
    }
}