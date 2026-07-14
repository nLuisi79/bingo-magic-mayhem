using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryCardGiftRowView : MonoBehaviour
    {
        [SerializeField] private Text cardNameText;
        [SerializeField] private Text detailText;
        [SerializeField] private Text starText;
        [SerializeField] private Button actionButton;

        public event Action ActionRequested;

        public void ResetRuntimeBindings()
        {
            UnsubscribeButton();
            cardNameText = null;
            detailText = null;
            starText = null;
            actionButton = null;
        }

        public void Initialize(Text cardName, Text detail, Text stars, Button action)
        {
            cardNameText = cardName;
            detailText = detail;
            starText = stars;
            actionButton = action;
            SubscribeButton();
        }

        public void Apply(LibraryCardGiftRowDisplayModel displayModel)
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

            if (starText != null)
            {
                starText.text = displayModel.StarText;
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
