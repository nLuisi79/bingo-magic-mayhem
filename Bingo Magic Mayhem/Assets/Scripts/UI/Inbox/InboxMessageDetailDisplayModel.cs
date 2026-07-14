namespace BingoMagicMayhem.UI.Inbox
{
    public sealed class InboxMessageDetailDisplayModel
    {
        public string BodyText { get; set; } = string.Empty;
        public string FootnoteText { get; set; } = string.Empty;
        public string BackButtonText { get; set; } = string.Empty;
        public string ReplyButtonText { get; set; } = string.Empty;
        public bool ShowReplyButton { get; set; }
    }
}
