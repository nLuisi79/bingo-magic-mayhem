namespace BingoMagicMayhem.UI.Coven
{
    public sealed class CovenJoinRequestRowDisplayModel
    {
        public string NameText { get; set; } = string.Empty;
        public string SummaryText { get; set; } = string.Empty;
        public string AcceptButtonText { get; set; } = string.Empty;
        public bool CanAccept { get; set; }
        public string DenyButtonText { get; set; } = string.Empty;
        public bool CanDeny { get; set; } = true;
    }
}
