namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class DailySpinActionAreaDisplayModel
    {
        public DailySpinActionAreaDisplayModel(string spinButtonText, string closeButtonText)
        {
            SpinButtonText = spinButtonText;
            CloseButtonText = closeButtonText;
        }

        public string SpinButtonText { get; }
        public string CloseButtonText { get; }
    }
}
