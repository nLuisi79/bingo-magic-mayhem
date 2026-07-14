using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryCardGiftingView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Button sampleButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button inboxButton;

        public event Action SampleRequested;
        public event Action BackRequested;
        public event Action InboxRequested;

        public void ResetRuntimeBindings()
        {
            if (sampleButton != null) sampleButton.onClick.RemoveListener(HandleSampleRequested);
            if (backButton != null) backButton.onClick.RemoveListener(HandleBackRequested);
            if (inboxButton != null) inboxButton.onClick.RemoveListener(HandleInboxRequested);

            titleText = null;
            subtitleText = null;
            summaryText = null;
            sampleButton = null;
            backButton = null;
            inboxButton = null;
        }

        public void Initialize(Text title, Text subtitle, Text summary, Button sample, Button back, Button inbox)
        {
            titleText = title;
            subtitleText = subtitle;
            summaryText = summary;
            sampleButton = sample;
            backButton = back;
            inboxButton = inbox;

            if (sampleButton != null)
            {
                sampleButton.onClick.RemoveListener(HandleSampleRequested);
                sampleButton.onClick.AddListener(HandleSampleRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }

            if (inboxButton != null)
            {
                inboxButton.onClick.RemoveListener(HandleInboxRequested);
                inboxButton.onClick.AddListener(HandleInboxRequested);
            }
        }

        public void Apply(LibraryCardGiftingDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (subtitleText != null) subtitleText.text = displayModel.SubtitleText;
            if (summaryText != null) summaryText.text = displayModel.SummaryText;

            if (sampleButton != null)
            {
                sampleButton.interactable = displayModel.CanSample;
                Text text = sampleButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.SampleButtonText;
            }

            if (backButton != null)
            {
                Text text = backButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.BackButtonText;
            }

            if (inboxButton != null)
            {
                Text text = inboxButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.InboxButtonText;
            }
        }

        private void HandleSampleRequested() => SampleRequested?.Invoke();
        private void HandleBackRequested() => BackRequested?.Invoke();
        private void HandleInboxRequested() => InboxRequested?.Invoke();
    }
}
