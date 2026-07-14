using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenFeatureTileView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Text hintText;
        [SerializeField] private Button button;

        public event Action Selected;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClicked);
            }

            titleText = null;
            subtitleText = null;
            detailText = null;
            hintText = null;
            button = null;
        }

        public void Initialize(Text title, Text subtitle, Text detail, Text hint, Button actionButton)
        {
            titleText = title;
            subtitleText = subtitle;
            detailText = detail;
            hintText = hint;
            button = actionButton;

            if (button != null)
            {
                button.onClick.RemoveListener(HandleClicked);
                button.onClick.AddListener(HandleClicked);
            }
        }

        public void Apply(string title, string subtitle, string detail, string hint)
        {
            if (titleText != null) titleText.text = title;
            if (subtitleText != null) subtitleText.text = subtitle;
            if (detailText != null) detailText.text = detail;
            if (hintText != null) hintText.text = hint;
        }

        private void HandleClicked()
        {
            Selected?.Invoke();
        }
    }
}
