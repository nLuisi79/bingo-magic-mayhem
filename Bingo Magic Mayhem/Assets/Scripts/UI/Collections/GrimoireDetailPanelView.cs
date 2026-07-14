using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class GrimoireDetailPanelView : MonoBehaviour
    {
        [SerializeField] private Text potionNameText;
        [SerializeField] private Text setLabelText;
        [SerializeField] private Text artPlaceholderText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text rewardIntroText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Text ingredientPreviewText;
        [SerializeField] private Button viewIngredientsButton;

        public event Action ClaimRequested;
        public event Action ViewIngredientsRequested;

        public void ResetRuntimeBindings()
        {
            if (claimButton != null) claimButton.onClick.RemoveListener(HandleClaimRequested);
            if (viewIngredientsButton != null) viewIngredientsButton.onClick.RemoveListener(HandleViewIngredientsRequested);

            potionNameText = null;
            setLabelText = null;
            artPlaceholderText = null;
            progressText = null;
            rewardIntroText = null;
            claimButton = null;
            ingredientPreviewText = null;
            viewIngredientsButton = null;
        }

        public void Initialize(
            Text potionName,
            Text setLabel,
            Text artPlaceholder,
            Text progress,
            Text rewardIntro,
            Button claim,
            Text ingredientPreview,
            Button viewIngredients)
        {
            potionNameText = potionName;
            setLabelText = setLabel;
            artPlaceholderText = artPlaceholder;
            progressText = progress;
            rewardIntroText = rewardIntro;
            claimButton = claim;
            ingredientPreviewText = ingredientPreview;
            viewIngredientsButton = viewIngredients;

            if (claimButton != null)
            {
                claimButton.onClick.RemoveListener(HandleClaimRequested);
                claimButton.onClick.AddListener(HandleClaimRequested);
            }

            if (viewIngredientsButton != null)
            {
                viewIngredientsButton.onClick.RemoveListener(HandleViewIngredientsRequested);
                viewIngredientsButton.onClick.AddListener(HandleViewIngredientsRequested);
            }
        }

        public void Apply(GrimoireDetailPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (potionNameText != null) potionNameText.text = displayModel.PotionNameText;
            if (setLabelText != null) setLabelText.text = displayModel.SetLabelText;
            if (artPlaceholderText != null) artPlaceholderText.text = displayModel.ArtPlaceholderText;
            if (progressText != null) progressText.text = displayModel.ProgressText;
            if (rewardIntroText != null) rewardIntroText.text = displayModel.RewardIntroText;
            if (ingredientPreviewText != null) ingredientPreviewText.text = displayModel.IngredientPreviewText;

            ApplyButton(claimButton, displayModel.ClaimButtonText, displayModel.ClaimButtonInteractable);

            if (viewIngredientsButton != null)
            {
                Text text = viewIngredientsButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.ViewIngredientsButtonText;
            }
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleClaimRequested() => ClaimRequested?.Invoke();
        private void HandleViewIngredientsRequested() => ViewIngredientsRequested?.Invoke();
    }
}
