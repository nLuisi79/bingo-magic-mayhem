namespace BingoMagicMayhem.UI.Trail
{
    public sealed class TrailRulesCardDisplayModel
    {
        public TrailRulesCardDisplayModel(string title, string bodyText, string noteText)
        {
            Title = title;
            BodyText = bodyText;
            NoteText = noteText;
        }

        public string Title { get; }
        public string BodyText { get; }
        public string NoteText { get; }
    }
}
