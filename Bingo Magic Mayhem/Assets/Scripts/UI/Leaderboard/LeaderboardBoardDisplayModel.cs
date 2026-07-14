namespace BingoMagicMayhem.UI.Leaderboard
{
    public sealed class LeaderboardBoardDisplayModel
    {
        public LeaderboardBoardDisplayModel(
            string titleText,
            string rankHeaderText,
            string playerHeaderText,
            string covenHeaderText,
            string weeklyScoreHeaderText)
        {
            TitleText = titleText ?? string.Empty;
            RankHeaderText = rankHeaderText ?? string.Empty;
            PlayerHeaderText = playerHeaderText ?? string.Empty;
            CovenHeaderText = covenHeaderText ?? string.Empty;
            WeeklyScoreHeaderText = weeklyScoreHeaderText ?? string.Empty;
        }

        public string TitleText { get; }
        public string RankHeaderText { get; }
        public string PlayerHeaderText { get; }
        public string CovenHeaderText { get; }
        public string WeeklyScoreHeaderText { get; }
    }
}
