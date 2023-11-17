using Airhead.Runtime.Utility;
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
        
        private void Awake()
        {
            var buttonPrefab = transform.Find<Button>("Pad/MenuButton");
            var buttons = new Button[3];
            buttons[0] = buttonPrefab;
            for (var i = 1; i < buttons.Length; i++)
            {
                buttons[i] = Instantiate(buttonPrefab, buttonPrefab.transform.parent);
            }

            SetupButton(buttons[0], "RESUME", () => Open(false));
            SetupButton(buttons[1], "RELOAD SCENE", ReloadScene);
            SetupButton(buttons[2], "QUIT", Quit);

            pauseAction.performed += OnPauseActionPerformed;
            pauseAction.Enable();
            
            Open(false);
        }

        private void OnDestroy()
        {
            pauseAction.performed -= OnPauseActionPerformed;
        }

        private void OnPauseActionPerformed(InputAction.CallbackContext obj) => Open(!gameObject.activeSelf);

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Open(bool state)
        {
            gameObject.SetActive(state);
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
    }
}