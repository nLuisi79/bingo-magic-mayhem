namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class RoomEntryRestorePanelDisplayModel
    {
        public RoomEntryRestorePanelDisplayModel(
            string potionName,
            string hintText,
            string statusText,
            string ingredientHeaderText,
            string ingredientBodyText,
            string restoreRewardText,
            string restoreButtonText,
            bool canRestore)
        {
            PotionName = potionName;
            HintText = hintText;
            StatusText = statusText;
            IngredientHeaderText = ingredientHeaderText;
            IngredientBodyText = ingredientBodyText;
            RestoreRewardText = restoreRewardText;
            RestoreButtonText = restoreButtonText;
            CanRestore = canRestore;
        }

        public string PotionName { get; }
        public string HintText { get; }
        public string StatusText { get; }
        public string IngredientHeaderText { get; }
        public string IngredientBodyText { get; }
        public string RestoreRewardText { get; }
        public string RestoreButtonText { get; }
        public bool CanRestore { get; }
    }
}
