namespace BingoMagicMayhem.UI.Bazaar
{
    public sealed class OracleReadingTableDisplayModel
    {
        public OracleReadingTableDisplayModel(string titleText, string statusText)
        {
            TitleText = titleText ?? string.Empty;
            StatusText = statusText ?? string.Empty;
        }

        public string TitleText { get; }
        public string StatusText { get; }
    }
}
