using System;
using BingoMagicMayhem.UI.Common;
using UnityEngine;

namespace BingoMagicMayhem.UI.Navigation
{
    /// <summary>
    /// Presentation-only modal shell controller. It surfaces close intent without owning flow rules.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIModalController : MonoBehaviour
    {
        [SerializeField] private UIFadeCanvasGroup fadeCanvasGroup;
        [SerializeField] private bool closeOnEscape = true;
        [SerializeField] private bool hideOnCloseRequest = true;

        public event Action CloseRequested;

        public bool IsVisible => fadeCanvasGroup != null && fadeCanvasGroup.IsVisible;

        private void Reset()
        {
            fadeCanvasGroup = GetComponent<UIFadeCanvasGroup>();
        }

        private void Awake()
        {
            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = GetComponent<UIFadeCanvasGroup>();
            }
        }

        private void Update()
        {
            if (closeOnEscape && IsVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                RequestClose();
            }
        }

        public void Show()
        {
            fadeCanvasGroup?.Show();
        }

        public void Hide()
        {
            fadeCanvasGroup?.Hide();
        }

        public void ShowInstant()
        {
            fadeCanvasGroup?.ShowInstant();
        }

        public void HideInstant()
        {
            fadeCanvasGroup?.HideInstant();
        }

        public void RequestClose()
        {
            if (hideOnCloseRequest)
            {
                Hide();
            }

            CloseRequested?.Invoke();
        }
    }
}
