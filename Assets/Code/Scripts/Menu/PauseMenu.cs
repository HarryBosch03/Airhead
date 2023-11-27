using System.Linq;
using Airhead.Runtime.Utility;
using FishNet.Managing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Airhead.Runtime.Menu
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PauseMenu : MonoBehaviour
    {
        public InputAction pauseAction;
        
        private Canvas canvas;

        private void Awake()
        {
            canvas = transform.Find<Canvas>("Canvas");
            
            var buttonPrefab = canvas.transform.Find<Button>("Pad/MenuButton");
            var buttons = new Button[5];
            buttons[0] = buttonPrefab;
            for (var i = 1; i < buttons.Length; i++)
            {
                buttons[i] = Instantiate(buttonPrefab, buttonPrefab.transform.parent);
            }

            SetupButton(buttons[0], "RESUME", () => Open(false));
            SetupButton(buttons[1], "RELOAD SCENE", ReloadScene);
            SetupButton(buttons[2], "START/STOP SERVER", StartServer);
            SetupButton(buttons[3], "START/STOP CLIENT", StartClient);
            SetupButton(buttons[4], "QUIT", Quit);

            pauseAction.performed += OnPauseActionPerformed;
            pauseAction.Enable();

            Open(false);
        }

        private void OnDestroy() { pauseAction.performed -= OnPauseActionPerformed; }

        private void OnPauseActionPerformed(InputAction.CallbackContext obj) => Open(!canvas.gameObject.activeSelf);

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ReloadScene() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

        private void StartServer()
        {
            var serverManager = NetworkManager.Instances.First().ServerManager;

            if (serverManager.Started) serverManager.StopConnection(true);
            else serverManager.StartConnection();
        }

        private void StartClient()
        {
            var clientManager = NetworkManager.Instances.First().ClientManager;

            if (clientManager.Started) clientManager.StopConnection();
            else clientManager.StartConnection();
        }

        private void Open(bool state)
        {
            canvas.gameObject.SetActive(state);
            Time.timeScale = state ? 0.0f : 1.0f;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }

        public void SetupButton(Button button, string label, UnityAction callback)
        {
            button.transform.SetAsLastSibling();
            button.name = label;
            button.GetComponentInChildren<TMP_Text>().text = label;
            button.onClick.AddListener(callback);
        }

        private void OnGUI()
        {
            var networkManager = NetworkManager.Instances.First();
            var server = networkManager.ServerManager;
            var client = networkManager.ClientManager;

            var text =
                $"Client: {(client.Started ? $"Connected to {client.Connection}" : "Not Connected")}\n" +
                $"Server: {(server.Started ? $"Started" : "Not Started")}";
            
            GUI.Label(new Rect(15.0f, 15.0f, 500.0f, 100.0f), text);
        }
    }
}