using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Presentation wrapper for a runtime-built room-entry card option tile.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryCardOptionView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Text titleText;
        [SerializeField] private Text flavorText;
        [SerializeField] private Text ingredientText;
        [SerializeField] private Text chanceText;
        [SerializeField] private Button actionButton;
        [SerializeField] private Text actionButtonText;

        private int cardCount;

        public event Action<int> PlayRequested;

        public void ResetRuntimeBindings()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandlePlayClicked);
            }

            background = null;
            titleText = null;
            flavorText = null;
            ingredientText = null;
            chanceText = null;
            actionButton = null;
            actionButtonText = null;
            cardCount = 0;
        }

        public void Initialize(
            Image backgroundImage,
            Text titleLabel,
            Text flavorLabel,
            Text ingredientLabel,
            Text chanceLabel,
            Button button,
            Text buttonLabel)
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandlePlayClicked);
            }

            background = backgroundImage;
            titleText = titleLabel;
            flavorText = flavorLabel;
            ingredientText = ingredientLabel;
            chanceText = chanceLabel;
            actionButton = button;
            actionButtonText = buttonLabel;

            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandlePlayClicked);
                actionButton.onClick.AddListener(HandlePlayClicked);
            }
        }

        public void Apply(RoomEntryCardOptionDisplayModel displayModel, Color backgroundColor)
        {
            cardCount = displayModel.CardCount;

            if (background != null)
            {
                background.color = backgroundColor;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (flavorText != null)
            {
                flavorText.text = displayModel.FlavorText;
            }

            if (ingredientText != null)
            {
                ingredientText.text = displayModel.IngredientText;
            }

            if (chanceText != null)
            {
                chanceText.text = displayModel.ChanceText;
            }

            if (actionButton != null)
            {
                actionButton.interactable = displayModel.CanPlay;
            }

            if (actionButtonText != null)
            {
                actionButtonText.text = displayModel.ActionText;
            }
        }

        private void HandlePlayClicked()
        {
            PlayRequested?.Invoke(cardCount);
        }

        private void OnDestroy()
        {
            if (actionButton != null)
            {
                actionButton.onClick.RemoveListener(HandlePlayClicked);
            }
        }
    }
}
