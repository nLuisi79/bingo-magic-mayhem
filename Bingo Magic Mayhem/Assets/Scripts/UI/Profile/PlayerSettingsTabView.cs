using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class PlayerSettingsTabView : MonoBehaviour
    {
        [SerializeField] private InputField displayNameInput;
        [SerializeField] private Text statusText;
        [SerializeField] private Button saveNameButton;
        [SerializeField] private Button soundToggleButton;
        [SerializeField] private Text soundToggleText;
        [SerializeField] private Button notificationsToggleButton;
        [SerializeField] private Text notificationsToggleText;

        public event Action SaveNameRequested;
        public event Action SoundToggleRequested;
        public event Action NotificationsToggleRequested;

        public InputField DisplayNameInput => displayNameInput;

        public void ResetRuntimeBindings()
        {
            if (saveNameButton != null)
            {
                saveNameButton.onClick.RemoveListener(HandleSaveNameRequested);
            }

            if (soundToggleButton != null)
            {
                soundToggleButton.onClick.RemoveListener(HandleSoundToggleRequested);
            }

            if (notificationsToggleButton != null)
            {
                notificationsToggleButton.onClick.RemoveListener(HandleNotificationsToggleRequested);
            }

            displayNameInput = null;
            statusText = null;
            saveNameButton = null;
            soundToggleButton = null;
            soundToggleText = null;
            notificationsToggleButton = null;
            notificationsToggleText = null;
        }

        public void Initialize(
            InputField nameInput,
            Text statusLabel,
            Button saveButton,
            Button soundButton,
            Text soundLabel,
            Button notificationsButton,
            Text notificationsLabel)
        {
            if (saveNameButton != null)
            {
                saveNameButton.onClick.RemoveListener(HandleSaveNameRequested);
            }

            if (soundToggleButton != null)
            {
                soundToggleButton.onClick.RemoveListener(HandleSoundToggleRequested);
            }

            if (notificationsToggleButton != null)
            {
                notificationsToggleButton.onClick.RemoveListener(HandleNotificationsToggleRequested);
            }

            displayNameInput = nameInput;
            statusText = statusLabel;
            saveNameButton = saveButton;
            soundToggleButton = soundButton;
            soundToggleText = soundLabel;
            notificationsToggleButton = notificationsButton;
            notificationsToggleText = notificationsLabel;

            saveNameButton?.onClick.AddListener(HandleSaveNameRequested);
            soundToggleButton?.onClick.AddListener(HandleSoundToggleRequested);
            notificationsToggleButton?.onClick.AddListener(HandleNotificationsToggleRequested);
        }

        public void Apply(PlayerSettingsTabDisplayModel displayModel)
        {
            if (displayNameInput != null)
            {
                displayNameInput.text = displayModel.DisplayName;
            }

            if (statusText != null)
            {
                statusText.text = displayModel.StatusMessage;
            }

            if (soundToggleText != null)
            {
                soundToggleText.text = displayModel.SoundEnabled ? "ON" : "OFF";
            }

            if (notificationsToggleText != null)
            {
                notificationsToggleText.text = displayModel.NotificationsEnabled ? "ON" : "OFF";
            }
        }

        private void HandleSaveNameRequested() => SaveNameRequested?.Invoke();
        private void HandleSoundToggleRequested() => SoundToggleRequested?.Invoke();
        private void HandleNotificationsToggleRequested() => NotificationsToggleRequested?.Invoke();

        private void OnDestroy()
        {
            if (saveNameButton != null)
            {
                saveNameButton.onClick.RemoveListener(HandleSaveNameRequested);
            }

            if (soundToggleButton != null)
            {
                soundToggleButton.onClick.RemoveListener(HandleSoundToggleRequested);
            }

            if (notificationsToggleButton != null)
            {
                notificationsToggleButton.onClick.RemoveListener(HandleNotificationsToggleRequested);
            }
        }
    }
}
