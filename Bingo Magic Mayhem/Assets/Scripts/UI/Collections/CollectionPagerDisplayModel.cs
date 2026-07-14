namespace BingoMagicMayhem.UI.Collections
{
    public sealed class CollectionPagerDisplayModel
    {
        public string PreviousButtonText { get; set; } = string.Empty;
        public bool PreviousButtonInteractable { get; set; }
        public string NextButtonText { get; set; } = string.Empty;
        public bool NextButtonInteractable { get; set; }
    }
}
