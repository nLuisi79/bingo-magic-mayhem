namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailySpinChromeDisplayModel
    {
        public DailySpinChromeDisplayModel(string title, string subtitle, string footerButtonText)
        {
            Title = title;
            Subtitle = subtitle;
            FooterButtonText = footerButtonText;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public string FooterButtonText { get; }
    }
}
