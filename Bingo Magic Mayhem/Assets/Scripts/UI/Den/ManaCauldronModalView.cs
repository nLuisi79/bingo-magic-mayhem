using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class ManaCauldronModalView : MonoBehaviour
    {
        [SerializeField] private Text headerText;
        [SerializeField] private Text amountText;
        [SerializeField] private Text resourceLabelText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text refillNoteText;
        [SerializeField] private Button collectButton;
        [SerializeField] private Button backButton;

        public event Action CollectRequested;
        public event Action BackRequested;

        public void ResetRuntimeBindings()
        {
            if (collectButton != null)
            {
                collectButton.onClick.RemoveListener(HandleCollectRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
            }

            headerText = null;
            amountText = null;
            resourceLabelText = null;
            statusText = null;
            refillNoteText = null;
            collectButton = null;
            backButton = null;
        }

        public void Initialize(
            Text header,
            Text amount,
            Text resourceLabel,
            Text status,
            Text refillNote,
            Button collect,
            Button back)
        {
            headerText = header;
            amountText = amount;
            resourceLabelText = resourceLabel;
            statusText = status;
            refillNoteText = refillNote;
            collectButton = collect;
            backButton = back;

            if (collectButton != null)
            {
                collectButton.onClick.RemoveListener(HandleCollectRequested);
                collectButton.onClick.AddListener(HandleCollectRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }
        }

        public void Apply(ManaCauldronModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (headerText != null) headerText.text = displayModel.HeaderText;
            if (amountText != null) amountText.text = displayModel.AmountText;
            if (resourceLabelText != null) resourceLabelText.text = displayModel.ResourceLabelText;
            if (statusText != null) statusText.text = displayModel.StatusText;
            if (refillNoteText != null) refillNoteText.text = displayModel.RefillNoteText;

            ApplyButton(collectButton, displayModel.CollectButtonText, displayModel.CollectButtonInteractable);
            ApplyButton(backButton, displayModel.BackButtonText, backButton != null && backButton.interactable);
        }

        private static void ApplyButton(Button button, string textValue, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = textValue;
            }
        }

        private void OnDestroy()
        {
            if (collectButton != null)
            {
                collectButton.onClick.RemoveListener(HandleCollectRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
            }
        }

        private void HandleCollectRequested()
        {
            CollectRequested?.Invoke();
        }

        private void HandleBackRequested()
        {
            BackRequested?.Invoke();
        }
    }
}
