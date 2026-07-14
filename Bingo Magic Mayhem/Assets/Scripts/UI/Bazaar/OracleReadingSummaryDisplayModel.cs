namespace BingoMagicMayhem.UI.Bazaar
{
    public sealed class OracleReadingSummaryDisplayModel
    {
        public OracleReadingSummaryDisplayModel(string summaryText, string actionButtonText)
        {
            SummaryText = summaryText ?? string.Empty;
            ActionButtonText = actionButtonText ?? string.Empty;
        }

        public string SummaryText { get; }
        public string ActionButtonText { get; }
    }
}
