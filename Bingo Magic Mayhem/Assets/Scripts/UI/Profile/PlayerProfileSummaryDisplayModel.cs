namespace BingoMagicMayhem.UI.Profile
{
    public sealed class PlayerProfileSummaryDisplayModel
    {
        public PlayerProfileSummaryDisplayModel(
            string displayName,
            string levelAndRankText,
            string auraRankNoteText,
            string manaText,
            string crystalsText,
            string albumText,
            string bookText,
            string roomsText,
            string auraText)
        {
            DisplayName = displayName;
            LevelAndRankText = levelAndRankText;
            AuraRankNoteText = auraRankNoteText;
            ManaText = manaText;
            CrystalsText = crystalsText;
            AlbumText = albumText;
            BookText = bookText;
            RoomsText = roomsText;
            AuraText = auraText;
        }

        public string DisplayName { get; }
        public string LevelAndRankText { get; }
        public string AuraRankNoteText { get; }
        public string ManaText { get; }
        public string CrystalsText { get; }
        public string AlbumText { get; }
        public string BookText { get; }
        public string RoomsText { get; }
        public string AuraText { get; }
    }
}
