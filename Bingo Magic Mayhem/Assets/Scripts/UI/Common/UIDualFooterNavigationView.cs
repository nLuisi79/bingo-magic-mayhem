using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UIDualFooterNavigationView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button closeButton;

        public event Action BackRequested;
        public event Action CloseRequested;

        public void ResetRuntimeBindings()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
            }

            backButton = null;
            closeButton = null;
        }

        public void Initialize(Button targetBackButton, Button targetCloseButton)
        {
            backButton = targetBackButton;
            closeButton = targetCloseButton;

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
                closeButton.onClick.AddListener(HandleCloseRequested);
            }
        }

        public void Apply(UIDualFooterNavigationDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            ApplyButton(backButton, displayModel.BackButtonText, displayModel.BackButtonInteractable);
            ApplyButton(closeButton, displayModel.CloseButtonText, displayModel.CloseButtonInteractable);
        }

        private static void ApplyButton(Button button, string textValue, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = textValue;
            }
        }

        private void OnDestroy()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
            }
        }

        private void HandleBackRequested()
        {
            BackRequested?.Invoke();
        }

        private void HandleCloseRequested()
        {
            CloseRequested?.Invoke();
        }
    }
}
