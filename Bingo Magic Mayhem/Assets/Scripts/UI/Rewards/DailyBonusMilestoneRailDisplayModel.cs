namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailyBonusMilestoneRailDisplayModel
    {
        public DailyBonusMilestoneRailDisplayModel(string progressText)
        {
            ProgressText = progressText;
        }

        public string ProgressText { get; }
    }
}
