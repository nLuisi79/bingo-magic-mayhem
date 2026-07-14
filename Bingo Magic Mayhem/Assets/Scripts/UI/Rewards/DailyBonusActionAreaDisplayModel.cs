namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailyBonusActionAreaDisplayModel
    {
        public DailyBonusActionAreaDisplayModel(string claimButtonText, string closeButtonText)
        {
            ClaimButtonText = claimButtonText;
            CloseButtonText = closeButtonText;
        }

        public string ClaimButtonText { get; }
        public string CloseButtonText { get; }
    }
}
