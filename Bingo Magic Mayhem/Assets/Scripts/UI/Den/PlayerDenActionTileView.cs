using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenActionTileView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button button;

        public event Action Selected;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleButtonClicked);
            }

            titleText = null;
            subtitleText = null;
            button = null;
        }

        public void Initialize(Text title, Text subtitle, Button actionButton)
        {
            titleText = title;
            subtitleText = subtitle;
            button = actionButton;

            if (button != null)
            {
                button.onClick.RemoveListener(HandleButtonClicked);
                button.onClick.AddListener(HandleButtonClicked);
            }
        }

        public void Apply(string title, string subtitle)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (subtitleText != null)
            {
                subtitleText.text = subtitle;
            }
        }

        private void HandleButtonClicked()
        {
            Selected?.Invoke();
        }
    }
}
