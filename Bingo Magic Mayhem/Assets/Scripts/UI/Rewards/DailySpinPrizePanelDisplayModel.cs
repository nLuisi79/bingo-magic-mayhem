namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailySpinPrizePanelDisplayModel
    {
        public DailySpinPrizePanelDisplayModel(string title, string subtitle, string sectionLabel, string mainText, string noteText)
        {
            Title = title;
            Subtitle = subtitle;
            SectionLabel = sectionLabel;
            MainText = mainText;
            NoteText = noteText;
        }

        public string Title { get; }
        public string Subtitle { get; }
        public string SectionLabel { get; }
        public string MainText { get; }
        public string NoteText { get; }
    }
}
