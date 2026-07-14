using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Presentation wrapper for the room-entry mana bet row.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryBetRowView : MonoBehaviour
    {
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Text summaryText;

        public event Action DecreaseRequested;
        public event Action IncreaseRequested;

        public Text SummaryText => summaryText;

        public void ResetRuntimeBindings()
        {
            if (decreaseButton != null)
            {
                decreaseButton.onClick.RemoveListener(HandleDecreaseClicked);
            }

            if (increaseButton != null)
            {
                increaseButton.onClick.RemoveListener(HandleIncreaseClicked);
            }

            decreaseButton = null;
            increaseButton = null;
            summaryText = null;
        }

        public void Initialize(Button decrease, Button increase, Text summary)
        {
            if (decreaseButton != null)
            {
                decreaseButton.onClick.RemoveListener(HandleDecreaseClicked);
            }

            if (increaseButton != null)
            {
                increaseButton.onClick.RemoveListener(HandleIncreaseClicked);
            }

            decreaseButton = decrease;
            increaseButton = increase;
            summaryText = summary;

            if (decreaseButton != null)
            {
                decreaseButton.onClick.RemoveListener(HandleDecreaseClicked);
                decreaseButton.onClick.AddListener(HandleDecreaseClicked);
            }

            if (increaseButton != null)
            {
                increaseButton.onClick.RemoveListener(HandleIncreaseClicked);
                increaseButton.onClick.AddListener(HandleIncreaseClicked);
            }
        }

        public void SetSummary(string summary)
        {
            if (summaryText != null)
            {
                summaryText.text = summary;
            }
        }

        private void HandleDecreaseClicked()
        {
            DecreaseRequested?.Invoke();
        }

        private void HandleIncreaseClicked()
        {
            IncreaseRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (decreaseButton != null)
            {
                decreaseButton.onClick.RemoveListener(HandleDecreaseClicked);
            }

            if (increaseButton != null)
            {
                increaseButton.onClick.RemoveListener(HandleIncreaseClicked);
            }
        }
    }
}
