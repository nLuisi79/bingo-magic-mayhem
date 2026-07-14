namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class SocialFreebiesFooterNoteDisplayModel
    {
        public SocialFreebiesFooterNoteDisplayModel(string noteText)
        {
            NoteText = noteText ?? string.Empty;
        }

        public string NoteText { get; }
    }
}
