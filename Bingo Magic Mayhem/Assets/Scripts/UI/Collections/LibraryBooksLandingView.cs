using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryBooksLandingView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button giftsButton;
        [SerializeField] private Button backButton;

        public event Action GiftsRequested;
        public event Action BackRequested;

        public void ResetRuntimeBindings()
        {
            if (giftsButton != null) giftsButton.onClick.RemoveListener(HandleGiftsRequested);
            if (backButton != null) backButton.onClick.RemoveListener(HandleBackRequested);

            titleText = null;
            subtitleText = null;
            giftsButton = null;
            backButton = null;
        }

        public void Initialize(Text title, Text subtitle, Button gifts, Button back)
        {
            titleText = title;
            subtitleText = subtitle;
            giftsButton = gifts;
            backButton = back;

            if (giftsButton != null)
            {
                giftsButton.onClick.RemoveListener(HandleGiftsRequested);
                giftsButton.onClick.AddListener(HandleGiftsRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }
        }

        public void Apply(LibraryBooksLandingDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (subtitleText != null) subtitleText.text = displayModel.SubtitleText;

            if (giftsButton != null)
            {
                Text text = giftsButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.GiftsButtonText;
            }

            if (backButton != null)
            {
                Text text = backButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.BackButtonText;
            }
        }

        private void HandleGiftsRequested() => GiftsRequested?.Invoke();
        private void HandleBackRequested() => BackRequested?.Invoke();
    }
}
