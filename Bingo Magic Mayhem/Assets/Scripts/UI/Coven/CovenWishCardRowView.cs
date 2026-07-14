using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Coven
{
    [DisallowMultipleComponent]
    public sealed class CovenWishCardRowView : MonoBehaviour
    {
        [SerializeField] private Text statusText;
        [SerializeField] private Button actionButton;

        public event Action ActionRequested;

        public void ResetRuntimeBindings()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandleActionClicked);
            }

            statusText = null;
            actionButton = null;
        }

        public void Initialize(Text status, Button action)
        {
            statusText = status;
            actionButton = action;
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandleActionClicked);
                actionButton.onClick.AddListener(HandleActionClicked);
            }
        }

        public void Apply(CovenWishCardRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (statusText != null)
            {
                statusText.text = displayModel.StatusText;
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

        private void HandleActionClicked() => ActionRequested?.Invoke();
    }
}
