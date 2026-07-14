namespace BingoMagicMayhem.UI.Profile
{
    public sealed class PlayerProfileModalDisplayModel
    {
        public PlayerProfileModalDisplayModel(string title, string subtitle, string closeButtonText)
        {
            Title = title;
            Subtitle = subtitle;
            CloseButtonText = closeButtonText;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public string CloseButtonText { get; }
    }
}
