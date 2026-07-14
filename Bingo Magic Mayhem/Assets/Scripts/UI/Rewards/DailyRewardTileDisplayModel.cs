namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailyRewardTileDisplayModel
    {
        public DailyRewardTileDisplayModel(string dayText, string iconText, string rewardText, string statusText)
        {
            DayText = dayText;
            IconText = iconText;
            RewardText = rewardText;
            StatusText = statusText;
        }

        public string DayText { get; }
        public string IconText { get; }
        public string RewardText { get; }
        public string StatusText { get; }
    }
}
