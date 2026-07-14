using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryMissingCardRowView : MonoBehaviour
    {
        [SerializeField] private Text cardNameText;
        [SerializeField] private Text detailText;
        [SerializeField] private Button actionButton;

        public event Action ActionRequested;

        public void ResetRuntimeBindings()
        {
            UnsubscribeButton();
            cardNameText = null;
            detailText = null;
            actionButton = null;
        }

        public void Initialize(Text cardName, Text detail, Button action)
        {
            cardNameText = cardName;
            detailText = detail;
            actionButton = action;
            SubscribeButton();
        }

        public void Apply(LibraryMissingCardRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (cardNameText != null)
            {
                cardNameText.text = displayModel.CardName;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.DetailText;
            }

            if (actionButton != null)
            {
                actionButton.interactable = displayModel.ActionInteractable;
                Text buttonText = actionButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.ActionButtonText;
                }
            }
        }

        private void SubscribeButton()
        {
            if (actionButton == null)
            {
                return;
            }

            actionButton.onClick.RemoveListener(HandleActionClicked);
            actionButton.onClick.AddListener(HandleActionClicked);
        }

        private void UnsubscribeButton()
        {
            if (actionButton == null)
            {
                return;
            }

            actionButton.onClick.RemoveListener(HandleActionClicked);
        }

        private void HandleActionClicked() => ActionRequested?.Invoke();
    }
}
