using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Coven
{
    [DisallowMultipleComponent]
    public sealed class CovenMemberRowView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Button openButton;

        public event Action OpenRequested;

        public void ResetRuntimeBindings()
        {
            if (openButton != null)
            {
                openButton.onClick.RemoveListener(HandleOpenClicked);
            }

            nameText = null;
            summaryText = null;
            openButton = null;
        }

        public void Initialize(Text name, Text summary, Button button)
        {
            nameText = name;
            summaryText = summary;
            openButton = button;
            if (openButton != null)
            {
                openButton.onClick.RemoveListener(HandleOpenClicked);
                openButton.onClick.AddListener(HandleOpenClicked);
            }
        }

        public void Apply(CovenMemberRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (nameText != null) nameText.text = displayModel.NameText;
            if (summaryText != null) summaryText.text = displayModel.SummaryText;
        }

        private void HandleOpenClicked() => OpenRequested?.Invoke();
    }
}
