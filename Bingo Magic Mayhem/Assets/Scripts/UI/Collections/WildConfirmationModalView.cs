using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class WildConfirmationModalView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text wildCardText;
        [SerializeField] private Text arrowText;
        [SerializeField] private Text targetSpecimenText;
        [SerializeField] private Text targetCardNameText;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;

        public event Action NoRequested;
        public event Action YesRequested;

        public void ResetRuntimeBindings()
        {
            if (noButton != null) noButton.onClick.RemoveListener(HandleNoRequested);
            if (yesButton != null) yesButton.onClick.RemoveListener(HandleYesRequested);

            titleText = null;
            wildCardText = null;
            arrowText = null;
            targetSpecimenText = null;
            targetCardNameText = null;
            noButton = null;
            yesButton = null;
        }

        public void Initialize(
            Text title,
            Text wildCard,
            Text arrow,
            Text targetSpecimen,
            Text targetCardName,
            Button no,
            Button yes)
        {
            titleText = title;
            wildCardText = wildCard;
            arrowText = arrow;
            targetSpecimenText = targetSpecimen;
            targetCardNameText = targetCardName;
            noButton = no;
            yesButton = yes;

            if (noButton != null)
            {
                noButton.onClick.RemoveListener(HandleNoRequested);
                noButton.onClick.AddListener(HandleNoRequested);
            }

            if (yesButton != null)
            {
                yesButton.onClick.RemoveListener(HandleYesRequested);
                yesButton.onClick.AddListener(HandleYesRequested);
            }
        }

        public void Apply(WildConfirmationModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (wildCardText != null) wildCardText.text = displayModel.WildCardText;
            if (arrowText != null) arrowText.text = displayModel.ArrowText;
            if (targetSpecimenText != null) targetSpecimenText.text = displayModel.TargetSpecimenText;
            if (targetCardNameText != null) targetCardNameText.text = displayModel.TargetCardNameText;

            ApplyButton(noButton, displayModel.NoButtonText);
            ApplyButton(yesButton, displayModel.YesButtonText);
        }

        private static void ApplyButton(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleNoRequested() => NoRequested?.Invoke();
        private void HandleYesRequested() => YesRequested?.Invoke();
    }
}
