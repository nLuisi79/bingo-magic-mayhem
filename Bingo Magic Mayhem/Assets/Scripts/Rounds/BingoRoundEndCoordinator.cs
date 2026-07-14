namespace BingoMagicMayhem.Rounds
{
    public sealed class BingoRoundEndCoordinator
    {
        public bool CountdownActive { get; private set; }

        public bool TryBeginCountdown(BingoRoundEndDecision decision, bool rewardPreviewShown)
        {
            if (decision == null || !decision.ShouldStartCountdown || CountdownActive || rewardPreviewShown)
            {
                return false;
            }

            CountdownActive = true;
            return true;
        }

        public void CancelCountdown()
        {
            CountdownActive = false;
        }

        public void CompleteCountdown()
        {
            CountdownActive = false;
        }
    }
}
