namespace BingoMagicMayhem.UI.Bazaar
{
    public sealed class BazaarFooterNoteDisplayModel
    {
        public BazaarFooterNoteDisplayModel(string noteText)
        {
            NoteText = noteText ?? string.Empty;
        }

        public string NoteText { get; }
    }
}
