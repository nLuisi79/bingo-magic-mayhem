namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class SocialFreebiesActionsDisplayModel
    {
        public SocialFreebiesActionsDisplayModel(
            string instagramButtonText,
            string facebookButtonText,
            string xButtonText,
            string simulateButtonText)
        {
            InstagramButtonText = instagramButtonText ?? string.Empty;
            FacebookButtonText = facebookButtonText ?? string.Empty;
            XButtonText = xButtonText ?? string.Empty;
            SimulateButtonText = simulateButtonText ?? string.Empty;
        }

        public string InstagramButtonText { get; }
        public string FacebookButtonText { get; }
        public string XButtonText { get; }
        public string SimulateButtonText { get; }
    }
}
