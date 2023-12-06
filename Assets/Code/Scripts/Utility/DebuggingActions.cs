using System;
using System.Linq;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Airhead.Runtime.Utility
{
    [SelectionBase, DisallowMultipleComponent]
    public sealed class DebuggingActions : MonoBehaviour
    {
        public InputAction startHost;
        public InputAction startClient;

        private void OnEnable()
        {
            Subscribe(startHost, StartHost);
            Subscribe(startClient, StartClient);
        }

        private void OnDisable()
        {
            Unsubscribe(startHost, StartHost);
            Unsubscribe(startClient, StartClient);
        }

        private void StartHost(InputAction.CallbackContext ctx)
        {
            var serverManager = NetworkManager.Instances.First().ServerManager;

            if (serverManager.Started) serverManager.StopConnection(true);
            else serverManager.StartConnection();
            
            StartClient(ctx);
        }

        private void StartClient (InputAction.CallbackContext ctx)
        {
            var clientManager = NetworkManager.Instances.First().ClientManager;

            if (clientManager.Started) clientManager.StopConnection();
            else clientManager.StartConnection();
        }

        private void Subscribe(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.performed += callback;
            action.Enable();
        }
        
        private void Unsubscribe(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            action.performed -= callback;
            action.Disable();
        }
    }
}
