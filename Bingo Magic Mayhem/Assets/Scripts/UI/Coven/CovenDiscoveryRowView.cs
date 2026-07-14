using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Coven
{
    [DisallowMultipleComponent]
    public sealed class CovenDiscoveryRowView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Button actionButton;

        public event Action ActionRequested;

        public void ResetRuntimeBindings()
        {
            UnsubscribeButton();
            nameText = null;
            summaryText = null;
            actionButton = null;
        }

        public void Initialize(Text name, Text summary, Button action)
        {
            nameText = name;
            summaryText = summary;
            actionButton = action;
            SubscribeButton();
        }

        public void Apply(CovenDiscoveryRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (nameText != null)
            {
                nameText.text = displayModel.NameText;
            }

            if (summaryText != null)
            {
                summaryText.text = displayModel.SummaryText;
            }

            if (actionButton != null)
            {
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
