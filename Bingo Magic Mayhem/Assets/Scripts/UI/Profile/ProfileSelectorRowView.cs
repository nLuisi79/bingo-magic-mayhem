using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class ProfileSelectorRowView : MonoBehaviour
    {
        [SerializeField] private Text labelText;
        [SerializeField] private Text valueText;
        [SerializeField] private Button actionButton;

        public event Action ActionRequested;

        public void ResetRuntimeBindings()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandleActionClicked);
            }

            labelText = null;
            valueText = null;
            actionButton = null;
        }

        public void Initialize(Text label, Text value, Button action)
        {
            labelText = label;
            valueText = value;
            actionButton = action;
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandleActionClicked);
                actionButton.onClick.AddListener(HandleActionClicked);
            }
        }

        public void Apply(ProfileSelectorRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (labelText != null) labelText.text = displayModel.LabelText;
            if (valueText != null) valueText.text = displayModel.ValueText;
            if (actionButton != null)
            {
                Text buttonText = actionButton.GetComponentInChildren<Text>();
                if (buttonText != null) buttonText.text = displayModel.ActionButtonText;
            }
        }

        private void HandleActionClicked() => ActionRequested?.Invoke();
    }
}
