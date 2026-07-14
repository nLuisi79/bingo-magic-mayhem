using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Friends
{
    [DisallowMultipleComponent]
    public sealed class FriendListRowView : MonoBehaviour
    {
        [SerializeField] private Text friendNameText;
        [SerializeField] private Text statusText;
        [SerializeField] private Text reportStatusText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button receiveButton;
        [SerializeField] private Button messageButton;
        [SerializeField] private Button blockButton;
        [SerializeField] private Button reportButton;

        public event Action SendRequested;
        public event Action ReceiveRequested;
        public event Action MessageRequested;
        public event Action BlockRequested;
        public event Action ReportRequested;

        public void ResetRuntimeBindings()
        {
            UnsubscribeButtons();
            friendNameText = null;
            statusText = null;
            reportStatusText = null;
            sendButton = null;
            receiveButton = null;
            messageButton = null;
            blockButton = null;
            reportButton = null;
        }

        public void Initialize(
            Text friendName,
            Text status,
            Text reportStatus,
            Button send,
            Button receive,
            Button message,
            Button block,
            Button report)
        {
            friendNameText = friendName;
            statusText = status;
            reportStatusText = reportStatus;
            sendButton = send;
            receiveButton = receive;
            messageButton = message;
            blockButton = block;
            reportButton = report;
            SubscribeButtons();
        }

        public void Apply(FriendListRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (friendNameText != null)
            {
                friendNameText.text = displayModel.FriendName;
            }

            if (statusText != null)
            {
                statusText.text = displayModel.StatusText;
            }

            if (reportStatusText != null)
            {
                reportStatusText.text = displayModel.ReportStatusText;
            }

            ApplyButton(sendButton, displayModel.SendButtonText, displayModel.CanSend);
            ApplyButton(receiveButton, displayModel.ReceiveButtonText, displayModel.CanReceive);
            ApplyButton(messageButton, displayModel.MessageButtonText, displayModel.CanMessage);
            ApplyButton(blockButton, displayModel.BlockButtonText, displayModel.CanBlock);
            ApplyButton(reportButton, displayModel.ReportButtonText, displayModel.CanReport);
        }

        private void SubscribeButtons()
        {
            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(HandleSendClicked);
                sendButton.onClick.AddListener(HandleSendClicked);
            }

            if (receiveButton != null)
            {
                receiveButton.onClick.RemoveListener(HandleReceiveClicked);
                receiveButton.onClick.AddListener(HandleReceiveClicked);
            }

            if (messageButton != null)
            {
                messageButton.onClick.RemoveListener(HandleMessageClicked);
                messageButton.onClick.AddListener(HandleMessageClicked);
            }

            if (blockButton != null)
            {
                blockButton.onClick.RemoveListener(HandleBlockClicked);
                blockButton.onClick.AddListener(HandleBlockClicked);
            }

            if (reportButton != null)
            {
                reportButton.onClick.RemoveListener(HandleReportClicked);
                reportButton.onClick.AddListener(HandleReportClicked);
            }
        }

        private void UnsubscribeButtons()
        {
            if (sendButton != null)
            {
                sendButton.onClick.RemoveListener(HandleSendClicked);
            }

            if (receiveButton != null)
            {
                receiveButton.onClick.RemoveListener(HandleReceiveClicked);
            }

            if (messageButton != null)
            {
                messageButton.onClick.RemoveListener(HandleMessageClicked);
            }

            if (blockButton != null)
            {
                blockButton.onClick.RemoveListener(HandleBlockClicked);
            }

            if (reportButton != null)
            {
                reportButton.onClick.RemoveListener(HandleReportClicked);
            }
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = label;
            }
        }

        private void HandleSendClicked() => SendRequested?.Invoke();
        private void HandleReceiveClicked() => ReceiveRequested?.Invoke();
        private void HandleMessageClicked() => MessageRequested?.Invoke();
        private void HandleBlockClicked() => BlockRequested?.Invoke();
        private void HandleReportClicked() => ReportRequested?.Invoke();
    }
}
