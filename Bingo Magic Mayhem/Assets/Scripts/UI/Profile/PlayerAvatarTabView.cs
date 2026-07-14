using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class PlayerAvatarTabView : MonoBehaviour
    {
        [SerializeField] private Text avatarNameText;
        [SerializeField] private Text cosmeticsSummaryText;
        [SerializeField] private Text avatarValueText;
        [SerializeField] private Text frameValueText;
        [SerializeField] private Text dauberValueText;
        [SerializeField] private Text footnoteText;
        [SerializeField] private Button avatarNextButton;
        [SerializeField] private Button frameNextButton;
        [SerializeField] private Button dauberNextButton;

        public event Action AvatarNextRequested;
        public event Action FrameNextRequested;
        public event Action DauberNextRequested;

        public void ResetRuntimeBindings()
        {
            if (avatarNextButton != null)
            {
                avatarNextButton.onClick.RemoveListener(HandleAvatarNextRequested);
            }

            if (frameNextButton != null)
            {
                frameNextButton.onClick.RemoveListener(HandleFrameNextRequested);
            }

            if (dauberNextButton != null)
            {
                dauberNextButton.onClick.RemoveListener(HandleDauberNextRequested);
            }

            avatarNameText = null;
            cosmeticsSummaryText = null;
            avatarValueText = null;
            frameValueText = null;
            dauberValueText = null;
            footnoteText = null;
            avatarNextButton = null;
            frameNextButton = null;
            dauberNextButton = null;
        }

        public void Initialize(
            Text avatarNameLabel,
            Text cosmeticsSummaryLabel,
            Text avatarValueLabel,
            Text frameValueLabel,
            Text dauberValueLabel,
            Text footnoteLabel,
            Button avatarButton,
            Button frameButton,
            Button dauberButton)
        {
            if (avatarNextButton != null)
            {
                avatarNextButton.onClick.RemoveListener(HandleAvatarNextRequested);
            }

            if (frameNextButton != null)
            {
                frameNextButton.onClick.RemoveListener(HandleFrameNextRequested);
            }

            if (dauberNextButton != null)
            {
                dauberNextButton.onClick.RemoveListener(HandleDauberNextRequested);
            }

            avatarNameText = avatarNameLabel;
            cosmeticsSummaryText = cosmeticsSummaryLabel;
            avatarValueText = avatarValueLabel;
            frameValueText = frameValueLabel;
            dauberValueText = dauberValueLabel;
            footnoteText = footnoteLabel;
            avatarNextButton = avatarButton;
            frameNextButton = frameButton;
            dauberNextButton = dauberButton;

            avatarNextButton?.onClick.AddListener(HandleAvatarNextRequested);
            frameNextButton?.onClick.AddListener(HandleFrameNextRequested);
            dauberNextButton?.onClick.AddListener(HandleDauberNextRequested);
        }

        public void Apply(PlayerAvatarTabDisplayModel displayModel)
        {
            if (avatarNameText != null) avatarNameText.text = displayModel.AvatarName;
            if (cosmeticsSummaryText != null) cosmeticsSummaryText.text = displayModel.CosmeticsSummary;
            if (avatarValueText != null) avatarValueText.text = displayModel.AvatarValue;
            if (frameValueText != null) frameValueText.text = displayModel.FrameValue;
            if (dauberValueText != null) dauberValueText.text = displayModel.DauberValue;
            if (footnoteText != null) footnoteText.text = displayModel.Footnote;
        }

        private void HandleAvatarNextRequested() => AvatarNextRequested?.Invoke();
        private void HandleFrameNextRequested() => FrameNextRequested?.Invoke();
        private void HandleDauberNextRequested() => DauberNextRequested?.Invoke();

        private void OnDestroy()
        {
            if (avatarNextButton != null)
            {
                avatarNextButton.onClick.RemoveListener(HandleAvatarNextRequested);
            }

            if (frameNextButton != null)
            {
                frameNextButton.onClick.RemoveListener(HandleFrameNextRequested);
            }

            if (dauberNextButton != null)
            {
                dauberNextButton.onClick.RemoveListener(HandleDauberNextRequested);
            }
        }
    }
}
