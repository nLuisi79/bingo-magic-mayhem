using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class BookOfShadowsDetailPanelView : MonoBehaviour
    {
        [SerializeField] private Text potionNameText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text rewardText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button viewIngredientsButton;

        public event Action ClaimRequested;
        public event Action ViewIngredientsRequested;

        public void ResetRuntimeBindings()
        {
            if (claimButton != null) claimButton.onClick.RemoveListener(HandleClaimRequested);
            if (viewIngredientsButton != null) viewIngredientsButton.onClick.RemoveListener(HandleViewIngredientsRequested);

            potionNameText = null;
            progressText = null;
            rewardText = null;
            claimButton = null;
            viewIngredientsButton = null;
        }

        public void Initialize(
            Text potionName,
            Text progress,
            Text reward,
            Button claim,
            Button viewIngredients)
        {
            potionNameText = potionName;
            progressText = progress;
            rewardText = reward;
            claimButton = claim;
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

        public void Apply(BookOfShadowsDetailPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (potionNameText != null) potionNameText.text = displayModel.PotionNameText;
            if (progressText != null) progressText.text = displayModel.ProgressText;
            if (rewardText != null) rewardText.text = displayModel.RewardText;

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
