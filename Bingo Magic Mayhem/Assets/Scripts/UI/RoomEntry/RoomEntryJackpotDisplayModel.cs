namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class RoomEntryJackpotDisplayModel
    {
        public RoomEntryJackpotDisplayModel(
            string title,
            string detail,
            bool showSpinPendingButton,
            string spinPendingButtonText)
        {
            Title = title;
            Detail = detail;
            ShowSpinPendingButton = showSpinPendingButton;
            SpinPendingButtonText = spinPendingButtonText;
        }

        public string Title { get; }
        public string Detail { get; }
        public bool ShowSpinPendingButton { get; }
        public string SpinPendingButtonText { get; }
    }

    public static class RoomEntryJackpotPresenter
    {
        public static RoomEntryJackpotDisplayModel Build(
            string currentPotText,
            string jackpotSpinText,
            string postRoundActionLabel,
            int pendingJackpotSpins,
            bool canResolvePostRoundHandoff)
        {
            string detail = $"Pot {currentPotText}\n{jackpotSpinText}";
            if (!string.IsNullOrEmpty(postRoundActionLabel))
            {
                detail = $"{detail}\n{postRoundActionLabel}";
            }

            return new RoomEntryJackpotDisplayModel(
                "Jackpot\nWheelspin",
                detail,
                pendingJackpotSpins > 0 || canResolvePostRoundHandoff,
                canResolvePostRoundHandoff
                    ? "Resolve Handoff"
                    : "Spin Pending");
        }
    }

    public sealed class RoomEntryRoundLaunchGateResult
    {
        public RoomEntryRoundLaunchGateResult(bool canLaunch, string blockedStatusText)
        {
            CanLaunch = canLaunch;
            BlockedStatusText = blockedStatusText ?? "";
        }

        public bool CanLaunch { get; }
        public string BlockedStatusText { get; }
    }

    public static class RoomEntryRoundLaunchGate
    {
        public static RoomEntryRoundLaunchGateResult Evaluate(bool isReplayBlockedByPostRoundHandoff, bool isWaitingForReconnect, string replayActionLabel)
        {
            if (isReplayBlockedByPostRoundHandoff || isWaitingForReconnect)
            {
                return new RoomEntryRoundLaunchGateResult(false, replayActionLabel);
            }

            return new RoomEntryRoundLaunchGateResult(true, "");
        }
    }
}
