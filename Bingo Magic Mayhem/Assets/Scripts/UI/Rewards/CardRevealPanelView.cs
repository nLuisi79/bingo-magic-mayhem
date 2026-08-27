using System;
using BingoMagicMayhem.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class CardRevealPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text overflowText;
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
            subtitleText = null;
            overflowText = null;
            continueButton = null;
            fadeCanvasGroup = null;
        }

        public void Initialize(Text title, Text subtitle, Text overflow, Button button)
        {
            titleText = title;
            subtitleText = subtitle;
            overflowText = overflow;
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

        public void Apply(CardRevealPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (subtitleText != null) subtitleText.text = displayModel.SubtitleText;

            if (overflowText != null)
            {
                overflowText.text = displayModel.OverflowText;
                overflowText.gameObject.SetActive(!string.IsNullOrWhiteSpace(displayModel.OverflowText));
            }

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
