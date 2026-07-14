namespace BingoMagicMayhem.UI.Den
{
    public sealed class PlayerDenDisplayModel
    {
        public PlayerDenDisplayModel(string title, string subtitle, string cauldronButtonText, string cauldronSummaryText)
        {
            Title = title;
            Subtitle = subtitle;
            CauldronButtonText = cauldronButtonText;
            CauldronSummaryText = cauldronSummaryText;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public string CauldronButtonText { get; }
        public string CauldronSummaryText { get; }
    }
}
