using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Coven
{
    [DisallowMultipleComponent]
    public sealed class CovenJoinRequestRowView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button denyButton;

        public event Action AcceptRequested;
        public event Action DenyRequested;

        public void ResetRuntimeBindings()
        {
            UnsubscribeButtons();
            nameText = null;
            summaryText = null;
            acceptButton = null;
            denyButton = null;
        }

        public void Initialize(Text name, Text summary, Button accept, Button deny)
        {
            nameText = name;
            summaryText = summary;
            acceptButton = accept;
            denyButton = deny;
            SubscribeButtons();
        }

        public void Apply(CovenJoinRequestRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (nameText != null) nameText.text = displayModel.NameText;
            if (summaryText != null) summaryText.text = displayModel.SummaryText;
            ApplyButton(acceptButton, displayModel.AcceptButtonText, displayModel.CanAccept);
            ApplyButton(denyButton, displayModel.DenyButtonText, displayModel.CanDeny);
        }

        private void SubscribeButtons()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(HandleAcceptClicked);
                acceptButton.onClick.AddListener(HandleAcceptClicked);
            }

            if (denyButton != null)
            {
                denyButton.onClick.RemoveListener(HandleDenyClicked);
                denyButton.onClick.AddListener(HandleDenyClicked);
            }
        }

        private void UnsubscribeButtons()
        {
            if (acceptButton != null) acceptButton.onClick.RemoveListener(HandleAcceptClicked);
            if (denyButton != null) denyButton.onClick.RemoveListener(HandleDenyClicked);
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null) return;
            button.interactable = interactable;
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = label;
        }

        private void HandleAcceptClicked() => AcceptRequested?.Invoke();
        private void HandleDenyClicked() => DenyRequested?.Invoke();
    }
}
