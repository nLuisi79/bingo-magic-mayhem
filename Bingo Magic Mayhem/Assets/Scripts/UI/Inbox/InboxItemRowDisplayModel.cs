namespace BingoMagicMayhem.UI.Inbox
{
    public sealed class InboxItemRowDisplayModel
    {
        public InboxItemRowDisplayModel(
            string title,
            string detail,
            string primaryBadgeText,
            string secondaryBadgeText,
            string primaryButtonText,
            string secondaryButtonText,
            bool isPrimaryButtonVisible,
            bool isSecondaryButtonVisible)
        {
            Title = title;
            Detail = detail;
            PrimaryBadgeText = primaryBadgeText;
            SecondaryBadgeText = secondaryBadgeText;
            PrimaryButtonText = primaryButtonText;
            SecondaryButtonText = secondaryButtonText;
            IsPrimaryButtonVisible = isPrimaryButtonVisible;
            IsSecondaryButtonVisible = isSecondaryButtonVisible;
        }

        public string Title { get; }
        public string Detail { get; }
        public string PrimaryBadgeText { get; }
        public string SecondaryBadgeText { get; }
        public string PrimaryButtonText { get; }
        public string SecondaryButtonText { get; }
        public bool IsPrimaryButtonVisible { get; }
        public bool IsSecondaryButtonVisible { get; }
    }
}
