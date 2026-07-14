namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailyBonusMilestoneMarkerDisplayModel
    {
        public DailyBonusMilestoneMarkerDisplayModel(string badgeText, string dayText)
        {
            BadgeText = badgeText;
            DayText = dayText;
        }

        public string BadgeText { get; }
        public string DayText { get; }
    }
}
