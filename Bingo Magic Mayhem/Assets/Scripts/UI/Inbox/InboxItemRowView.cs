using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Inbox
{
    [DisallowMultipleComponent]
    public sealed class InboxItemRowView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Text primaryBadgeText;
        [SerializeField] private Text secondaryBadgeText;
        [SerializeField] private Button primaryButton;
        [SerializeField] private Button secondaryButton;

        public event Action PrimaryActionRequested;
        public event Action SecondaryActionRequested;

        public void ResetRuntimeBindings()
        {
            if (primaryButton != null)
            {
                primaryButton.onClick.RemoveListener(HandlePrimaryButtonClicked);
            }

            if (secondaryButton != null)
            {
                secondaryButton.onClick.RemoveListener(HandleSecondaryButtonClicked);
            }

            titleText = null;
            detailText = null;
            primaryBadgeText = null;
            secondaryBadgeText = null;
            primaryButton = null;
            secondaryButton = null;
        }

        public void Initialize(
            Text title,
            Text detail,
            Text primaryBadge,
            Text secondaryBadge,
            Button primaryActionButton,
            Button secondaryActionButton)
        {
            titleText = title;
            detailText = detail;
            primaryBadgeText = primaryBadge;
            secondaryBadgeText = secondaryBadge;
            primaryButton = primaryActionButton;
            secondaryButton = secondaryActionButton;

            if (primaryButton != null)
            {
                primaryButton.onClick.RemoveListener(HandlePrimaryButtonClicked);
                primaryButton.onClick.AddListener(HandlePrimaryButtonClicked);
            }

            if (secondaryButton != null)
            {
                secondaryButton.onClick.RemoveListener(HandleSecondaryButtonClicked);
                secondaryButton.onClick.AddListener(HandleSecondaryButtonClicked);
            }
        }

        public void Apply(InboxItemRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.Detail;
            }

            if (primaryBadgeText != null)
            {
                primaryBadgeText.text = displayModel.PrimaryBadgeText;
            }

            if (secondaryBadgeText != null)
            {
                secondaryBadgeText.text = displayModel.SecondaryBadgeText;
            }

            ApplyButton(primaryButton, displayModel.PrimaryButtonText, displayModel.IsPrimaryButtonVisible);
            ApplyButton(secondaryButton, displayModel.SecondaryButtonText, displayModel.IsSecondaryButtonVisible);
        }

        private void ApplyButton(Button button, string label, bool isVisible)
        {
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(isVisible);
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = label;
            }
        }

        private void HandlePrimaryButtonClicked()
        {
            PrimaryActionRequested?.Invoke();
        }

        private void HandleSecondaryButtonClicked()
        {
            SecondaryActionRequested?.Invoke();
        }
    }
}
