namespace BingoMagicMayhem.UI.Market
{
    public sealed class MayhemMarketWalletDisplayModel
    {
        public MayhemMarketWalletDisplayModel(string walletText)
        {
            WalletText = walletText ?? string.Empty;
        }

        public string WalletText { get; }
    }
}
