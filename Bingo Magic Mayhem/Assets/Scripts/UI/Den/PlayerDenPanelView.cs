using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text cauldronButtonText;
        [SerializeField] private Text cauldronSummaryText;
        [SerializeField] private Button cauldronButton;

        public event Action CauldronRequested;

        public void ResetRuntimeBindings()
        {
            if (cauldronButton != null)
            {
                cauldronButton.onClick.RemoveListener(HandleCauldronRequested);
            }

            titleText = null;
            subtitleText = null;
            cauldronButtonText = null;
            cauldronSummaryText = null;
            cauldronButton = null;
        }

        public void Initialize(
            Text titleLabel,
            Text subtitleLabel,
            Button cauldronActionButton,
            Text cauldronActionLabel,
            Text cauldronSummaryLabel)
        {
            if (cauldronButton != null)
            {
                cauldronButton.onClick.RemoveListener(HandleCauldronRequested);
            }

            titleText = titleLabel;
            subtitleText = subtitleLabel;
            cauldronButton = cauldronActionButton;
            cauldronButtonText = cauldronActionLabel;
            cauldronSummaryText = cauldronSummaryLabel;

            if (cauldronButton != null)
            {
                cauldronButton.onClick.RemoveListener(HandleCauldronRequested);
                cauldronButton.onClick.AddListener(HandleCauldronRequested);
            }
        }

        public void Apply(PlayerDenDisplayModel displayModel)
        {
            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (subtitleText != null)
            {
                subtitleText.text = displayModel.Subtitle;
            }

            if (cauldronButtonText != null)
            {
                cauldronButtonText.text = displayModel.CauldronButtonText;
            }

            if (cauldronSummaryText != null)
            {
                cauldronSummaryText.text = displayModel.CauldronSummaryText;
            }
        }

        private void HandleCauldronRequested()
        {
            CauldronRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (cauldronButton != null)
            {
                cauldronButton.onClick.RemoveListener(HandleCauldronRequested);
            }
        }
    }
}
