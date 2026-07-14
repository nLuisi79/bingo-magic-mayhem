using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class IngredientDetailModalView : MonoBehaviour
    {
        [SerializeField] private Text cardNameText;
        [SerializeField] private Text metadataText;
        [SerializeField] private Text artPlaceholderText;
        [SerializeField] private Text artFooterText;
        [SerializeField] private Text countTitleText;
        [SerializeField] private Text countValueText;
        [SerializeField] private Text countStatusText;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button wildButton;
        [SerializeField] private Text sourceTitleText;
        [SerializeField] private Text sourceBodyText;
        [SerializeField] private Button closeButton;

        public event Action HelpRequested;
        public event Action WildRequested;
        public event Action CloseRequested;

        public void ResetRuntimeBindings()
        {
            if (helpButton != null) helpButton.onClick.RemoveListener(HandleHelpRequested);
            if (wildButton != null) wildButton.onClick.RemoveListener(HandleWildRequested);
            if (closeButton != null) closeButton.onClick.RemoveListener(HandleCloseRequested);

            cardNameText = null;
            metadataText = null;
            artPlaceholderText = null;
            artFooterText = null;
            countTitleText = null;
            countValueText = null;
            countStatusText = null;
            helpButton = null;
            wildButton = null;
            sourceTitleText = null;
            sourceBodyText = null;
            closeButton = null;
        }

        public void Initialize(
            Text cardName,
            Text metadata,
            Text artPlaceholder,
            Text artFooter,
            Text countTitle,
            Text countValue,
            Text countStatus,
            Button help,
            Button wild,
            Text sourceTitle,
            Text sourceBody,
            Button close)
        {
            cardNameText = cardName;
            metadataText = metadata;
            artPlaceholderText = artPlaceholder;
            artFooterText = artFooter;
            countTitleText = countTitle;
            countValueText = countValue;
            countStatusText = countStatus;
            helpButton = help;
            wildButton = wild;
            sourceTitleText = sourceTitle;
            sourceBodyText = sourceBody;
            closeButton = close;

            if (helpButton != null)
            {
                helpButton.onClick.RemoveListener(HandleHelpRequested);
                helpButton.onClick.AddListener(HandleHelpRequested);
            }

            if (wildButton != null)
            {
                wildButton.onClick.RemoveListener(HandleWildRequested);
                wildButton.onClick.AddListener(HandleWildRequested);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleCloseRequested);
                closeButton.onClick.AddListener(HandleCloseRequested);
            }
        }

        public void Apply(IngredientDetailModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (cardNameText != null) cardNameText.text = displayModel.CardNameText;
            if (metadataText != null) metadataText.text = displayModel.MetadataText;
            if (artPlaceholderText != null) artPlaceholderText.text = displayModel.ArtPlaceholderText;
            if (artFooterText != null) artFooterText.text = displayModel.ArtFooterText;
            if (countTitleText != null) countTitleText.text = displayModel.CountTitleText;
            if (countValueText != null) countValueText.text = displayModel.CountValueText;
            if (countStatusText != null) countStatusText.text = displayModel.CountStatusText;
            if (sourceTitleText != null) sourceTitleText.text = displayModel.SourceTitleText;
            if (sourceBodyText != null) sourceBodyText.text = displayModel.SourceBodyText;

            ApplyButton(helpButton, displayModel.HelpButtonText, true);
            ApplyButton(wildButton, displayModel.WildButtonText, displayModel.WildButtonInteractable);
            ApplyButton(closeButton, displayModel.CloseButtonText, true);
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleHelpRequested() => HelpRequested?.Invoke();
        private void HandleWildRequested() => WildRequested?.Invoke();
        private void HandleCloseRequested() => CloseRequested?.Invoke();
    }
}
