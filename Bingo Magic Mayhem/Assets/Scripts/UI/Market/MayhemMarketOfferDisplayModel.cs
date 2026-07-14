namespace BingoMagicMayhem.UI.Market
{
    public sealed class MayhemMarketOfferDisplayModel
    {
        public MayhemMarketOfferDisplayModel(string category, string title, string detail, string buttonText, bool isInteractable)
        {
            Category = category;
            Title = title;
            Detail = detail;
            ButtonText = buttonText;
            IsInteractable = isInteractable;
        }

        public string Category { get; }
        public string Title { get; }
        public string Detail { get; }
        public string ButtonText { get; }
        public bool IsInteractable { get; }
    }
}
