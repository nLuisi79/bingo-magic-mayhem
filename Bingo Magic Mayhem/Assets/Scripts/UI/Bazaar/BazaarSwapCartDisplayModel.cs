namespace BingoMagicMayhem.UI.Bazaar
{
    public sealed class BazaarSwapCartDisplayModel
    {
        public BazaarSwapCartDisplayModel(string titleText, string subtitleText, string actionButtonText)
        {
            TitleText = titleText ?? string.Empty;
            SubtitleText = subtitleText ?? string.Empty;
            ActionButtonText = actionButtonText ?? string.Empty;
        }

        public string TitleText { get; }
        public string SubtitleText { get; }
        public string ActionButtonText { get; }
    }
}
