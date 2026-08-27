using System;
using BingoMagicMayhem.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RewardPreviewPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text roundResultText;
        [SerializeField] private Text collectedRewardsTitleText;
        [SerializeField] private Text potionProgressTitleText;
        [SerializeField] private Button continueButton;
        [SerializeField] private UIFadeCanvasGroup fadeCanvasGroup;

        public event Action ContinueRequested;

        public void ResetRuntimeBindings()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueRequested);
            }

            titleText = null;
            roundResultText = null;
            collectedRewardsTitleText = null;
            potionProgressTitleText = null;
            continueButton = null;
            fadeCanvasGroup = null;
        }

        public void Initialize(
            Text title,
            Text roundResult,
            Text collectedRewardsTitle,
            Text potionProgressTitle,
            Button button)
        {
            titleText = title;
            roundResultText = roundResult;
            collectedRewardsTitleText = collectedRewardsTitle;
            potionProgressTitleText = potionProgressTitle;
            continueButton = button;

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueRequested);
                continueButton.onClick.AddListener(HandleContinueRequested);
            }
        }

        public void PlayIntroPresentation()
        {
            if (fadeCanvasGroup == null)
            {
                fadeCanvasGroup = gameObject.GetComponent<UIFadeCanvasGroup>();
            }

            fadeCanvasGroup?.Show();
        }

        public void Apply(RewardPreviewPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (roundResultText != null) roundResultText.text = displayModel.RoundResultText;
            if (collectedRewardsTitleText != null) collectedRewardsTitleText.text = displayModel.CollectedRewardsTitleText;
            if (potionProgressTitleText != null) potionProgressTitleText.text = displayModel.PotionProgressTitleText;

            if (continueButton != null)
            {
                Text buttonText = continueButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.ContinueButtonText;
                }
            }
        }

        private void HandleContinueRequested() => ContinueRequested?.Invoke();

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(HandleContinueRequested);
            }
        }
    }
}
