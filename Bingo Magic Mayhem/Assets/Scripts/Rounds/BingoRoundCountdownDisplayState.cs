namespace BingoMagicMayhem.Rounds
{
    public sealed class BingoRoundCountdownDisplayState
    {
        public BingoRoundCountdownDisplayState(
            string timerLabel,
            string calledNumberLabel,
            string ballsLeftLabel,
            string burstLabel,
            string statusLabel)
        {
            TimerLabel = timerLabel ?? "";
            CalledNumberLabel = calledNumberLabel ?? "";
            BallsLeftLabel = ballsLeftLabel ?? "";
            BurstLabel = burstLabel ?? "";
            StatusLabel = statusLabel ?? "";
        }

        public string TimerLabel { get; }
        public string CalledNumberLabel { get; }
        public string BallsLeftLabel { get; }
        public string BurstLabel { get; }
        public string StatusLabel { get; }
    }

    public static class BingoRoundCountdownPresenter
    {
        public static BingoRoundCountdownDisplayState Build(int secondsRemaining, string latestCallLabel, string countdownMessage)
        {
            string endLabel = $"END {secondsRemaining}";
            return new BingoRoundCountdownDisplayState(
                endLabel,
                string.IsNullOrEmpty(latestCallLabel) ? endLabel : $"{latestCallLabel}\n{endLabel}",
                secondsRemaining.ToString(),
                $"ROUND ENDS IN {secondsRemaining}",
                $"{countdownMessage} Round ends in {secondsRemaining}.");
        }
    }
}
