using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenTopBandView : MonoBehaviour
    {
        [SerializeField] private Text brandText;
        [SerializeField] private Text manaValueText;
        [SerializeField] private Text crystalsValueText;
        [SerializeField] private Button rankButton;
        [SerializeField] private Button mapButton;
        [SerializeField] private Button roomButton;
        [SerializeField] private Button settingsButton;

        public event Action RankRequested;
        public event Action MapRequested;
        public event Action RoomRequested;
        public event Action SettingsRequested;

        public void ResetRuntimeBindings()
        {
            if (rankButton != null) rankButton.onClick.RemoveListener(HandleRankRequested);
            if (mapButton != null) mapButton.onClick.RemoveListener(HandleMapRequested);
            if (roomButton != null) roomButton.onClick.RemoveListener(HandleRoomRequested);
            if (settingsButton != null) settingsButton.onClick.RemoveListener(HandleSettingsRequested);

            brandText = null;
            manaValueText = null;
            crystalsValueText = null;
            rankButton = null;
            mapButton = null;
            roomButton = null;
            settingsButton = null;
        }

        public void Initialize(Text brand, Text manaValue, Text crystalsValue, Button rank, Button map, Button room, Button settings)
        {
            brandText = brand;
            manaValueText = manaValue;
            crystalsValueText = crystalsValue;
            rankButton = rank;
            mapButton = map;
            roomButton = room;
            settingsButton = settings;

            if (rankButton != null)
            {
                rankButton.onClick.RemoveListener(HandleRankRequested);
                rankButton.onClick.AddListener(HandleRankRequested);
            }

            if (mapButton != null)
            {
                mapButton.onClick.RemoveListener(HandleMapRequested);
                mapButton.onClick.AddListener(HandleMapRequested);
            }

            if (roomButton != null)
            {
                roomButton.onClick.RemoveListener(HandleRoomRequested);
                roomButton.onClick.AddListener(HandleRoomRequested);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(HandleSettingsRequested);
                settingsButton.onClick.AddListener(HandleSettingsRequested);
            }
        }

        public void Apply(PlayerDenTopBandDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (brandText != null) brandText.text = displayModel.BrandText;
            if (manaValueText != null) manaValueText.text = displayModel.ManaText;
            if (crystalsValueText != null) crystalsValueText.text = displayModel.CrystalsText;

            ApplyButtonText(rankButton, displayModel.RankButtonText);
            ApplyButtonText(mapButton, displayModel.MapButtonText);
            ApplyButtonText(roomButton, displayModel.RoomButtonText);
            ApplyButtonText(settingsButton, displayModel.SettingsButtonText);
        }

        private static void ApplyButtonText(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleRankRequested() => RankRequested?.Invoke();
        private void HandleMapRequested() => MapRequested?.Invoke();
        private void HandleRoomRequested() => RoomRequested?.Invoke();
        private void HandleSettingsRequested() => SettingsRequested?.Invoke();
    }
}
