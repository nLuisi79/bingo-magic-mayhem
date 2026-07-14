namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailyBonusHeaderDisplayModel
    {
        public DailyBonusHeaderDisplayModel(string title, string streakText, string nextChestText, string streakWarningText)
        {
            Title = title;
            StreakText = streakText;
            NextChestText = nextChestText;
            StreakWarningText = streakWarningText;
        }

        public string Title { get; }
        public string StreakText { get; }
        public string NextChestText { get; }
        public string StreakWarningText { get; }
    }
}
