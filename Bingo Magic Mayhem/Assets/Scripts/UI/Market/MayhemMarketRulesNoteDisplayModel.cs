namespace BingoMagicMayhem.UI.Market
{
    public sealed class MayhemMarketRulesNoteDisplayModel
    {
        public MayhemMarketRulesNoteDisplayModel(string noteText)
        {
            NoteText = noteText ?? string.Empty;
        }

        public string NoteText { get; }
    }
}
