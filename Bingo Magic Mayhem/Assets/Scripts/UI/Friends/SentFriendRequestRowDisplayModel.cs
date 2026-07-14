namespace BingoMagicMayhem.UI.Friends
{
    public sealed class SentFriendRequestRowDisplayModel
    {
        public SentFriendRequestRowDisplayModel(string friendName, string cancelButtonText)
        {
            FriendName = friendName ?? string.Empty;
            CancelButtonText = cancelButtonText ?? string.Empty;
        }

        public string FriendName { get; }
        public string CancelButtonText { get; }
    }
}
