namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class JackpotSpinScreenDisplayModel
    {
        public string TitleText { get; set; } = string.Empty;
        public string SubtitleText { get; set; } = string.Empty;
        public string RoomNameText { get; set; } = string.Empty;
        public string SpinStatusText { get; set; } = string.Empty;
        public string ResultText { get; set; } = string.Empty;
        public string CenterButtonText { get; set; } = string.Empty;
        public string CollectButtonText { get; set; } = string.Empty;
        public bool CanUseCenterButton { get; set; }
        public bool CanUseCollectButton { get; set; }
    }
}
