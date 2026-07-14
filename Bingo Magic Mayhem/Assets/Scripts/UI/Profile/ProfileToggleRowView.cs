using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class ProfileToggleRowView : MonoBehaviour
    {
        [SerializeField] private Text labelText;
        [SerializeField] private Button toggleButton;

        public event Action ToggleRequested;

        public void ResetRuntimeBindings()
        {
            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveListener(HandleToggleClicked);
            }

            labelText = null;
            toggleButton = null;
        }

        public void Initialize(Text label, Button toggle)
        {
            labelText = label;
            toggleButton = toggle;
            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveListener(HandleToggleClicked);
                toggleButton.onClick.AddListener(HandleToggleClicked);
            }
        }

        public void Apply(ProfileToggleRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (labelText != null) labelText.text = displayModel.LabelText;
            if (toggleButton != null)
            {
                Text buttonText = toggleButton.GetComponentInChildren<Text>();
                if (buttonText != null) buttonText.text = displayModel.ToggleText;
            }
        }

        private void HandleToggleClicked() => ToggleRequested?.Invoke();
    }
}
