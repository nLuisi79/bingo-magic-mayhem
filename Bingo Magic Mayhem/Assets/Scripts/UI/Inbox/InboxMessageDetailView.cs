using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Inbox
{
    [DisallowMultipleComponent]
    public sealed class InboxMessageDetailView : MonoBehaviour
    {
        [SerializeField] private Text bodyText;
        [SerializeField] private Text footnoteText;
        [SerializeField] private Button backButton;
        [SerializeField] private Button replyButton;

        public event Action BackRequested;
        public event Action ReplyRequested;

        public void ResetRuntimeBindings()
        {
            if (backButton != null) backButton.onClick.RemoveListener(HandleBackClicked);
            if (replyButton != null) replyButton.onClick.RemoveListener(HandleReplyClicked);
            bodyText = null;
            footnoteText = null;
            backButton = null;
            replyButton = null;
        }

        public void Initialize(Text body, Text footnote, Button back, Button reply)
        {
            bodyText = body;
            footnoteText = footnote;
            backButton = back;
            replyButton = reply;
            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackClicked);
                backButton.onClick.AddListener(HandleBackClicked);
            }

            if (replyButton != null)
            {
                replyButton.onClick.RemoveListener(HandleReplyClicked);
                replyButton.onClick.AddListener(HandleReplyClicked);
            }
        }

        public void Apply(InboxMessageDetailDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (bodyText != null) bodyText.text = displayModel.BodyText;
            if (footnoteText != null) footnoteText.text = displayModel.FootnoteText;
            ApplyButton(backButton, displayModel.BackButtonText, true);
            if (replyButton != null)
            {
                replyButton.gameObject.SetActive(displayModel.ShowReplyButton);
                ApplyButton(replyButton, displayModel.ReplyButtonText, displayModel.ShowReplyButton);
            }
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null) return;
            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleBackClicked() => BackRequested?.Invoke();
        private void HandleReplyClicked() => ReplyRequested?.Invoke();
    }
}
