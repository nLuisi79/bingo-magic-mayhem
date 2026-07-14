using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenCauldronView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text buttonText;
        [SerializeField] private Text summaryText;

        public event Action Requested;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
            }

            button = null;
            buttonText = null;
            summaryText = null;
        }

        public void Initialize(Button targetButton, Text targetButtonText, Text targetSummaryText)
        {
            button = targetButton;
            buttonText = targetButtonText;
            summaryText = targetSummaryText;

            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
                button.onClick.AddListener(HandleRequested);
            }
        }

        public void Apply(PlayerDenCauldronDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (buttonText != null)
            {
                buttonText.text = displayModel.ButtonText;
            }

            if (summaryText != null)
            {
                summaryText.text = displayModel.SummaryText;
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
            }
        }

        private void HandleRequested()
        {
            Requested?.Invoke();
        }
    }
}
