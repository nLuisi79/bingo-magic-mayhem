namespace BingoMagicMayhem.UI.Collections
{
    public sealed class IngredientHelpModalDisplayModel
    {
        public string TitleText { get; set; } = string.Empty;
        public string UsageText { get; set; } = string.Empty;
        public string SummaryCardNameText { get; set; } = string.Empty;
        public string SummarySpecimenText { get; set; } = string.Empty;
        public string SummaryCountText { get; set; } = string.Empty;
        public string CovenButtonText { get; set; } = string.Empty;
        public bool CovenButtonInteractable { get; set; } = true;
        public string FriendsButtonText { get; set; } = string.Empty;
        public bool FriendsButtonInteractable { get; set; }
        public string RecipientsTitleText { get; set; } = string.Empty;
        public string HelpNoteText { get; set; } = string.Empty;
        public string SendButtonText { get; set; } = string.Empty;
        public bool SendButtonInteractable { get; set; }
        public string BackButtonText { get; set; } = string.Empty;
    }
}
