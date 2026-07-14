using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Friends
{
    [DisallowMultipleComponent]
    public sealed class IncomingFriendRequestRowView : MonoBehaviour
    {
        [SerializeField] private Text friendNameText;
        [SerializeField] private Text detailText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        public event Action AcceptRequested;
        public event Action DeclineRequested;

        public void ResetRuntimeBindings()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(HandleAcceptClicked);
            }

            if (declineButton != null)
            {
                declineButton.onClick.RemoveListener(HandleDeclineClicked);
            }

            friendNameText = null;
            detailText = null;
            acceptButton = null;
            declineButton = null;
        }

        public void Initialize(Text friendName, Text detail, Button accept, Button decline)
        {
            friendNameText = friendName;
            detailText = detail;
            acceptButton = accept;
            declineButton = decline;

            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(HandleAcceptClicked);
                acceptButton.onClick.AddListener(HandleAcceptClicked);
            }

            if (declineButton != null)
            {
                declineButton.onClick.RemoveListener(HandleDeclineClicked);
                declineButton.onClick.AddListener(HandleDeclineClicked);
            }
        }

        public void Apply(IncomingFriendRequestRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (friendNameText != null)
            {
                friendNameText.text = displayModel.FriendName;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.DetailText;
            }

            ApplyButtonText(acceptButton, displayModel.AcceptButtonText);
            ApplyButtonText(declineButton, displayModel.DeclineButtonText);
        }

        private static void ApplyButtonText(Button button, string text)
        {
            if (button == null)
            {
                return;
            }

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }

        private void HandleAcceptClicked() => AcceptRequested?.Invoke();
        private void HandleDeclineClicked() => DeclineRequested?.Invoke();
    }
}
