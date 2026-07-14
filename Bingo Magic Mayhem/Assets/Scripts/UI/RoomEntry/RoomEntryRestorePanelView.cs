using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Presentation wrapper for the room-entry restore and ingredient strip.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryRestorePanelView : MonoBehaviour
    {
        [SerializeField] private Text potionNameText;
        [SerializeField] private Text hintText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text ingredientHeaderText;
        [SerializeField] private Text ingredientBodyText;
        [SerializeField] private Text restoreRewardText;
        [SerializeField] private Button restoreButton;
        [SerializeField] private Text restoreButtonText;

        public event Action RestoreRequested;

        public void ResetRuntimeBindings()
        {
            if (restoreButton != null)
            {
                restoreButton.onClick.RemoveListener(HandleRestoreClicked);
            }

            potionNameText = null;
            hintText = null;
            statusText = null;
            ingredientHeaderText = null;
            ingredientBodyText = null;
            restoreRewardText = null;
            restoreButton = null;
            restoreButtonText = null;
        }

        public void Initialize(
            Text potionNameLabel,
            Text hintLabel,
            Text statusLabel,
            Text ingredientHeaderLabel,
            Text ingredientBodyLabel,
            Text restoreRewardLabel,
            Button restoreActionButton,
            Text restoreActionButtonText)
        {
            if (restoreButton != null)
            {
                restoreButton.onClick.RemoveListener(HandleRestoreClicked);
            }

            potionNameText = potionNameLabel;
            hintText = hintLabel;
            statusText = statusLabel;
            ingredientHeaderText = ingredientHeaderLabel;
            ingredientBodyText = ingredientBodyLabel;
            restoreRewardText = restoreRewardLabel;
            restoreButton = restoreActionButton;
            restoreButtonText = restoreActionButtonText;

            if (restoreButton != null)
            {
                restoreButton.onClick.RemoveListener(HandleRestoreClicked);
                restoreButton.onClick.AddListener(HandleRestoreClicked);
            }
        }

        public void Apply(RoomEntryRestorePanelDisplayModel displayModel)
        {
            if (potionNameText != null)
            {
                potionNameText.text = displayModel.PotionName;
            }

            if (hintText != null)
            {
                hintText.text = displayModel.HintText;
            }

            if (statusText != null)
            {
                statusText.text = displayModel.StatusText;
                statusText.color = displayModel.CanRestore
                    ? new Color(0.06f, 0.45f, 0.08f)
                    : new Color(0.15f, 0.09f, 0.22f);
            }

            if (ingredientHeaderText != null)
            {
                ingredientHeaderText.text = displayModel.IngredientHeaderText;
            }

            if (ingredientBodyText != null)
            {
                ingredientBodyText.text = displayModel.IngredientBodyText;
            }

            if (restoreRewardText != null)
            {
                restoreRewardText.text = displayModel.RestoreRewardText;
            }

            if (restoreButton != null)
            {
                restoreButton.interactable = displayModel.CanRestore;
                Image buttonImage = restoreButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = displayModel.CanRestore
                        ? new Color(0.12f, 0.55f, 0.08f)
                        : new Color(0.35f, 0.12f, 0.62f);
                }
            }

            if (restoreButtonText != null)
            {
                restoreButtonText.text = displayModel.RestoreButtonText;
            }
        }

        private void HandleRestoreClicked()
        {
            RestoreRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (restoreButton != null)
            {
                restoreButton.onClick.RemoveListener(HandleRestoreClicked);
            }
        }
    }
}
