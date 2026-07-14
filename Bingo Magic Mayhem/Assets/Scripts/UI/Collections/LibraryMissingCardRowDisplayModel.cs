namespace BingoMagicMayhem.UI.Collections
{
    public sealed class LibraryMissingCardRowDisplayModel
    {
        public string CardName { get; set; } = string.Empty;
        public string DetailText { get; set; } = string.Empty;
        public string ActionButtonText { get; set; } = string.Empty;
        public bool ActionInteractable { get; set; }
    }
}
