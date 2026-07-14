namespace BingoMagicMayhem.UI.Friends
{
    public sealed class FriendListRowDisplayModel
    {
        public FriendListRowDisplayModel(
            string friendName,
            string statusText,
            string reportStatusText,
            string sendButtonText,
            string receiveButtonText,
            string messageButtonText,
            string blockButtonText,
            string reportButtonText,
            bool canSend,
            bool canReceive,
            bool canMessage,
            bool canBlock,
            bool canReport)
        {
            FriendName = friendName ?? string.Empty;
            StatusText = statusText ?? string.Empty;
            ReportStatusText = reportStatusText ?? string.Empty;
            SendButtonText = sendButtonText ?? string.Empty;
            ReceiveButtonText = receiveButtonText ?? string.Empty;
            MessageButtonText = messageButtonText ?? string.Empty;
            BlockButtonText = blockButtonText ?? string.Empty;
            ReportButtonText = reportButtonText ?? string.Empty;
            CanSend = canSend;
            CanReceive = canReceive;
            CanMessage = canMessage;
            CanBlock = canBlock;
            CanReport = canReport;
        }

        public string FriendName { get; }
        public string StatusText { get; }
        public string ReportStatusText { get; }
        public string SendButtonText { get; }
        public string ReceiveButtonText { get; }
        public string MessageButtonText { get; }
        public string BlockButtonText { get; }
        public string ReportButtonText { get; }
        public bool CanSend { get; }
        public bool CanReceive { get; }
        public bool CanMessage { get; }
        public bool CanBlock { get; }
        public bool CanReport { get; }
    }
}
