namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class RoomEntryCardOptionDisplayModel
    {
        public RoomEntryCardOptionDisplayModel(
            int cardCount,
            string title,
            string flavorText,
            string ingredientText,
            string chanceText,
            string actionText,
            bool canPlay)
        {
            CardCount = cardCount;
            Title = title;
            FlavorText = flavorText;
            IngredientText = ingredientText;
            ChanceText = chanceText;
            ActionText = actionText;
            CanPlay = canPlay;
        }

        public int CardCount { get; }
        public string Title { get; }
        public string FlavorText { get; }
        public string IngredientText { get; }
        public string ChanceText { get; }
        public string ActionText { get; }
        public bool CanPlay { get; }
    }
}
