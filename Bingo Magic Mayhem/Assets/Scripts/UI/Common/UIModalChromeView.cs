using System;
using BingoMagicMayhem.UI.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UIModalChromeView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Text closeButtonText;
        [SerializeField] private UIModalController modalController;

        public event Action CloseRequested;

        public void ResetRuntimeBindings()
        {
            if (modalController != null)
            {
                modalController.CloseRequested -= HandleCloseRequested;
            }
            else if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
            }

            titleText = null;
            subtitleText = null;
            closeButton = null;
            closeButtonText = null;
            modalController = null;
        }

        public void Initialize(
            Text titleLabel,
            Text subtitleLabel,
            Button closeActionButton,
            Text closeActionLabel,
            UIModalController controller)
        {
            if (modalController != null)
            {
                modalController.CloseRequested -= HandleCloseRequested;
            }
            else if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
            }

            titleText = titleLabel;
            subtitleText = subtitleLabel;
            closeButton = closeActionButton;
            closeButtonText = closeActionLabel;
            modalController = controller;

            if (modalController != null)
            {
                modalController.CloseRequested += HandleCloseRequested;
            }
            else if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
                closeButton.onClick.AddListener(HandleCloseRequested);
            }
        }

        public void Apply(string title, string subtitle, string closeLabel)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (subtitleText != null)
            {
                subtitleText.text = subtitle;
            }

            if (closeButtonText != null)
            {
                closeButtonText.text = closeLabel;
            }
        }

        private void OnDestroy()
        {
            if (modalController != null)
            {
                modalController.CloseRequested -= HandleCloseRequested;
            }
        }

        private void HandleCloseRequested()
        {
            CloseRequested?.Invoke();
        }
    }
}
