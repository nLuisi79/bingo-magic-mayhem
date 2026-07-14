using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class IngredientHelpModalView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text usageText;
        [SerializeField] private Text summaryCardNameText;
        [SerializeField] private Text summarySpecimenText;
        [SerializeField] private Text summaryCountText;
        [SerializeField] private Button covenButton;
        [SerializeField] private Button friendsButton;
        [SerializeField] private Text recipientsTitleText;
        [SerializeField] private Text helpNoteText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button backButton;

        public event Action CovenRequested;
        public event Action FriendsRequested;
        public event Action SendRequested;
        public event Action BackRequested;

        public void ResetRuntimeBindings()
        {
            if (covenButton != null) covenButton.onClick.RemoveListener(HandleCovenRequested);
            if (friendsButton != null) friendsButton.onClick.RemoveListener(HandleFriendsRequested);
            if (sendButton != null) sendButton.onClick.RemoveListener(HandleSendRequested);
            if (backButton != null) backButton.onClick.RemoveListener(HandleBackRequested);

            titleText = null;
            usageText = null;
            summaryCardNameText = null;
            summarySpecimenText = null;
            summaryCountText = null;
            covenButton = null;
            friendsButton = null;
            recipientsTitleText = null;
            helpNoteText = null;
            sendButton = null;
            backButton = null;
        }

        public void Initialize(
            Text title,
            Text usage,
            Text summaryCardName,
            Text summarySpecimen,
            Text summaryCount,
            Button coven,
            Button friends,
            Text recipientsTitle,
            Text helpNote,
            Button send,
            Button back)
        {
            titleText = title;
            usageText = usage;
            summaryCardNameText = summaryCardName;
            summarySpecimenText = summarySpecimen;
            summaryCountText = summaryCount;
            covenButton = coven;
            friendsButton = friends;
            recipientsTitleText = recipientsTitle;
            helpNoteText = helpNote;
            sendButton = send;
            backButton = back;

            if (covenButton != null)
            {
                covenButton.onClick.RemoveListener(HandleCovenRequested);
                covenButton.onClick.AddListener(HandleCovenRequested);
            }

            if (friendsButton != null)
            {
                friendsButton.onClick.RemoveListener(HandleFriendsRequested);
                friendsButton.onClick.AddListener(HandleFriendsRequested);
            }

            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(HandleSendRequested);
                sendButton.onClick.AddListener(HandleSendRequested);
            }

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }
        }

        public void Apply(IngredientHelpModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (usageText != null) usageText.text = displayModel.UsageText;
            if (summaryCardNameText != null) summaryCardNameText.text = displayModel.SummaryCardNameText;
            if (summarySpecimenText != null) summarySpecimenText.text = displayModel.SummarySpecimenText;
            if (summaryCountText != null) summaryCountText.text = displayModel.SummaryCountText;
            if (recipientsTitleText != null) recipientsTitleText.text = displayModel.RecipientsTitleText;
            if (helpNoteText != null) helpNoteText.text = displayModel.HelpNoteText;

            ApplyButton(covenButton, displayModel.CovenButtonText, displayModel.CovenButtonInteractable);
            ApplyButton(friendsButton, displayModel.FriendsButtonText, displayModel.FriendsButtonInteractable);
            ApplyButton(sendButton, displayModel.SendButtonText, displayModel.SendButtonInteractable);
            ApplyButton(backButton, displayModel.BackButtonText, true);
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

        private void HandleCovenRequested() => CovenRequested?.Invoke();
        private void HandleFriendsRequested() => FriendsRequested?.Invoke();
        private void HandleSendRequested() => SendRequested?.Invoke();
        private void HandleBackRequested() => BackRequested?.Invoke();
    }
}
