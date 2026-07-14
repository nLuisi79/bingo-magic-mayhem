namespace BingoMagicMayhem.UI.Friends
{
    public sealed class IncomingFriendRequestRowDisplayModel
    {
        public IncomingFriendRequestRowDisplayModel(
            string friendName,
            string detailText,
            string acceptButtonText,
            string declineButtonText)
        {
            FriendName = friendName ?? string.Empty;
            DetailText = detailText ?? string.Empty;
            AcceptButtonText = acceptButtonText ?? string.Empty;
            DeclineButtonText = declineButtonText ?? string.Empty;
        }

        public string FriendName { get; }
        public string DetailText { get; }
        public string AcceptButtonText { get; }
        public string DeclineButtonText { get; }
    }
}
