using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Friends
{
    [DisallowMultipleComponent]
    public sealed class SentFriendRequestRowView : MonoBehaviour
    {
        [SerializeField] private Text friendNameText;
        [SerializeField] private Button cancelButton;

        public event Action CancelRequested;

        public void ResetRuntimeBindings()
        {
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(HandleCancelClicked);
            }

            friendNameText = null;
            cancelButton = null;
        }

        public void Initialize(Text friendName, Button cancel)
        {
            friendNameText = friendName;
            cancelButton = cancel;

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(HandleCancelClicked);
                cancelButton.onClick.AddListener(HandleCancelClicked);
            }
        }

        public void Apply(SentFriendRequestRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (friendNameText != null)
            {
                friendNameText.text = displayModel.FriendName;
            }

            if (cancelButton != null)
            {
                Text buttonText = cancelButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.CancelButtonText;
                }
            }
        }

        private void HandleCancelClicked() => CancelRequested?.Invoke();
    }
}
