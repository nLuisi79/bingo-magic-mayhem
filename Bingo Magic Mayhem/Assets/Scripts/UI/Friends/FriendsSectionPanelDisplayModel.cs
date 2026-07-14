namespace BingoMagicMayhem.UI.Friends
{
    public sealed class FriendsSectionPanelDisplayModel
    {
        public FriendsSectionPanelDisplayModel(string titleText, string emptyStateText)
        {
            TitleText = titleText ?? string.Empty;
            EmptyStateText = emptyStateText ?? string.Empty;
        }

        public string TitleText { get; }
        public string EmptyStateText { get; }
    }
}
