namespace BingoMagicMayhem.UI.Leaderboard
{
    public sealed class LeaderboardScopeNoteDisplayModel
    {
        public LeaderboardScopeNoteDisplayModel(string noteText)
        {
            NoteText = noteText ?? string.Empty;
        }

        public string NoteText { get; }
    }
}
