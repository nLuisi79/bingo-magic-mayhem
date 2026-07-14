namespace BingoMagicMayhem.UI.Bazaar
{
    public sealed class BazaarDestinationPanelDisplayModel
    {
        public BazaarDestinationPanelDisplayModel(string title, string subtitle, string detail, string hint)
        {
            Title = title;
            Subtitle = subtitle;
            Detail = detail;
            Hint = hint;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public string Detail { get; }
        public string Hint { get; }
    }
}
