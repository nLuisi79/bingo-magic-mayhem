using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class JackpotSpinScreenView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text roomNameText;
        [SerializeField] private Text spinStatusText;
        [SerializeField] private Text resultText;
        [SerializeField] private Button centerButton;
        [SerializeField] private Button collectButton;

        public event Action CenterRequested;
        public event Action CollectRequested;

        public void ResetRuntimeBindings()
        {
            if (centerButton != null) centerButton.onClick.RemoveListener(HandleCenterRequested);
            if (collectButton != null) collectButton.onClick.RemoveListener(HandleCollectRequested);

            titleText = null;
            subtitleText = null;
            roomNameText = null;
            spinStatusText = null;
            resultText = null;
            centerButton = null;
            collectButton = null;
        }

        public void Initialize(Text title, Text subtitle, Text roomName, Text spinStatus, Text result, Button center, Button collect)
        {
            titleText = title;
            subtitleText = subtitle;
            roomNameText = roomName;
            spinStatusText = spinStatus;
            resultText = result;
            centerButton = center;
            collectButton = collect;

            if (centerButton != null)
            {
                centerButton.onClick.RemoveListener(HandleCenterRequested);
                centerButton.onClick.AddListener(HandleCenterRequested);
            }

            if (collectButton != null)
            {
                collectButton.onClick.RemoveListener(HandleCollectRequested);
                collectButton.onClick.AddListener(HandleCollectRequested);
            }
        }

        public void Apply(JackpotSpinScreenDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (subtitleText != null) subtitleText.text = displayModel.SubtitleText;
            if (roomNameText != null) roomNameText.text = displayModel.RoomNameText;
            if (spinStatusText != null) spinStatusText.text = displayModel.SpinStatusText;
            if (resultText != null) resultText.text = displayModel.ResultText;

            if (centerButton != null)
            {
                centerButton.interactable = displayModel.CanUseCenterButton;
                Text text = centerButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.CenterButtonText;
            }

            if (collectButton != null)
            {
                collectButton.interactable = displayModel.CanUseCollectButton;
                Text text = collectButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.CollectButtonText;
            }
        }

        private void HandleCenterRequested() => CenterRequested?.Invoke();
        private void HandleCollectRequested() => CollectRequested?.Invoke();
    }
}
