using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Presentation wrapper for the room-entry jackpot summary tile.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryJackpotView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Button spinPendingButton;
        [SerializeField] private Text spinPendingButtonText;

        public event Action SpinPendingRequested;

        public void ResetRuntimeBindings()
        {
            if (spinPendingButton != null)
            {
                spinPendingButton.onClick.RemoveListener(HandleSpinPendingClicked);
            }

            titleText = null;
            detailText = null;
            spinPendingButton = null;
            spinPendingButtonText = null;
        }

        public void Initialize(
            Text titleLabel,
            Text detailLabel,
            Button spinButton,
            Text spinButtonLabel)
        {
            if (spinPendingButton != null)
            {
                spinPendingButton.onClick.RemoveListener(HandleSpinPendingClicked);
            }

            titleText = titleLabel;
            detailText = detailLabel;
            spinPendingButton = spinButton;
            spinPendingButtonText = spinButtonLabel;

            if (spinPendingButton != null)
            {
                spinPendingButton.onClick.RemoveListener(HandleSpinPendingClicked);
                spinPendingButton.onClick.AddListener(HandleSpinPendingClicked);
            }
        }

        public void Apply(RoomEntryJackpotDisplayModel displayModel)
        {
            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.Detail;
            }

            if (spinPendingButton != null)
            {
                spinPendingButton.gameObject.SetActive(displayModel.ShowSpinPendingButton);
            }

            if (spinPendingButtonText != null)
            {
                spinPendingButtonText.text = displayModel.SpinPendingButtonText;
            }
        }

        private void HandleSpinPendingClicked()
        {
            SpinPendingRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (spinPendingButton != null)
            {
                spinPendingButton.onClick.RemoveListener(HandleSpinPendingClicked);
            }
        }
    }
}
