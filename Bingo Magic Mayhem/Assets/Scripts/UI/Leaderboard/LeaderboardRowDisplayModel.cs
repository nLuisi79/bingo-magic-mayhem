namespace BingoMagicMayhem.UI.Leaderboard
{
    public sealed class LeaderboardRowDisplayModel
    {
        public LeaderboardRowDisplayModel(string rankText, string playerNameText, string covenNameText, string scoreText)
        {
            RankText = rankText ?? string.Empty;
            PlayerNameText = playerNameText ?? string.Empty;
            CovenNameText = covenNameText ?? string.Empty;
            ScoreText = scoreText ?? string.Empty;
        }

        public string RankText { get; }
        public string PlayerNameText { get; }
        public string CovenNameText { get; }
        public string ScoreText { get; }
    }
}
