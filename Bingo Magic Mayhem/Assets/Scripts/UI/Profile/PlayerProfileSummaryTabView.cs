using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class PlayerProfileSummaryTabView : MonoBehaviour
    {
        [SerializeField] private Text displayNameText;
        [SerializeField] private Text levelAndRankText;
        [SerializeField] private Text auraRankNoteText;
        [SerializeField] private Text manaText;
        [SerializeField] private Text crystalsText;
        [SerializeField] private Text albumText;
        [SerializeField] private Text bookText;
        [SerializeField] private Text roomsText;
        [SerializeField] private Text auraText;
        [SerializeField] private Button auraRanksButton;

        public event Action AuraRanksRequested;

        public void ResetRuntimeBindings()
        {
            if (auraRanksButton != null)
            {
                auraRanksButton.onClick.RemoveListener(HandleAuraRanksRequested);
            }

            displayNameText = null;
            levelAndRankText = null;
            auraRankNoteText = null;
            manaText = null;
            crystalsText = null;
            albumText = null;
            bookText = null;
            roomsText = null;
            auraText = null;
            auraRanksButton = null;
        }

        public void Initialize(
            Text displayNameLabel,
            Text levelAndRankLabel,
            Text auraRankNoteLabel,
            Text manaValue,
            Text crystalsValue,
            Text albumValue,
            Text bookValue,
            Text roomsValue,
            Text auraValue,
            Button auraButton)
        {
            if (auraRanksButton != null)
            {
                auraRanksButton.onClick.RemoveListener(HandleAuraRanksRequested);
            }

            displayNameText = displayNameLabel;
            levelAndRankText = levelAndRankLabel;
            auraRankNoteText = auraRankNoteLabel;
            manaText = manaValue;
            crystalsText = crystalsValue;
            albumText = albumValue;
            bookText = bookValue;
            roomsText = roomsValue;
            auraText = auraValue;
            auraRanksButton = auraButton;

            if (auraRanksButton != null)
            {
                auraRanksButton.onClick.RemoveListener(HandleAuraRanksRequested);
                auraRanksButton.onClick.AddListener(HandleAuraRanksRequested);
            }
        }

        public void Apply(PlayerProfileSummaryDisplayModel displayModel)
        {
            if (displayNameText != null) displayNameText.text = displayModel.DisplayName;
            if (levelAndRankText != null) levelAndRankText.text = displayModel.LevelAndRankText;
            if (auraRankNoteText != null) auraRankNoteText.text = displayModel.AuraRankNoteText;
            if (manaText != null) manaText.text = displayModel.ManaText;
            if (crystalsText != null) crystalsText.text = displayModel.CrystalsText;
            if (albumText != null) albumText.text = displayModel.AlbumText;
            if (bookText != null) bookText.text = displayModel.BookText;
            if (roomsText != null) roomsText.text = displayModel.RoomsText;
            if (auraText != null) auraText.text = displayModel.AuraText;
        }

        private void HandleAuraRanksRequested()
        {
            AuraRanksRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (auraRanksButton != null)
            {
                auraRanksButton.onClick.RemoveListener(HandleAuraRanksRequested);
            }
        }
    }
}
