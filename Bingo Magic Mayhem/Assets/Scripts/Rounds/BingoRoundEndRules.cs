namespace BingoMagicMayhem.Rounds
{
    public enum BingoRoundEndReasonKind
    {
        None = 0,
        FinalBall = 1,
        BlackoutComplete = 2,
        RoomPoolExhausted = 3,
        StandardCoverage = 4
    }

    public sealed class BingoRoundEndDecision
    {
        public BingoRoundEndDecision(BingoRoundEndReasonKind kind, string reason, string countdownMessage)
        {
            Kind = kind;
            Reason = reason ?? "";
            CountdownMessage = countdownMessage ?? "";
        }

        public BingoRoundEndReasonKind Kind { get; }
        public string Reason { get; }
        public string CountdownMessage { get; }
        public bool ShouldStartCountdown => Kind != BingoRoundEndReasonKind.None;
    }

    public static class BingoRoundEndRules
    {
        public static BingoRoundEndDecision ResolvePostProgressDecision(
            bool blackoutRoom,
            bool allCardsReachedJackpot,
            bool roomPoolExhausted,
            string roomPoolReason,
            bool standardCoverageReached,
            string standardCoverageReason)
        {
            if (blackoutRoom && allCardsReachedJackpot)
            {
                return CreateBlackoutCompleteDecision();
            }

            if (!blackoutRoom && roomPoolExhausted)
            {
                return CreateRoomPoolExhaustedDecision(roomPoolReason);
            }

            if (!blackoutRoom && standardCoverageReached)
            {
                return CreateStandardCoverageDecision(standardCoverageReason);
            }

            return new BingoRoundEndDecision(BingoRoundEndReasonKind.None, "", "");
        }

        public static BingoRoundEndDecision CreateFinalBallDecision(string reason)
        {
            return new BingoRoundEndDecision(BingoRoundEndReasonKind.FinalBall, reason, "Final ball called. Daub anything you missed.");
        }

        public static BingoRoundEndDecision CreateBlackoutCompleteDecision()
        {
            return new BingoRoundEndDecision(BingoRoundEndReasonKind.BlackoutComplete, "Every card reached blackout.", "All active cards reached blackout.");
        }

        public static BingoRoundEndDecision CreateRoomPoolExhaustedDecision(string reason)
        {
            return new BingoRoundEndDecision(BingoRoundEndReasonKind.RoomPoolExhausted, reason, "Room bingo pool exhausted.");
        }

        public static BingoRoundEndDecision CreateStandardCoverageDecision(string reason)
        {
            return new BingoRoundEndDecision(BingoRoundEndReasonKind.StandardCoverage, reason, "Final ball called. Daub anything you missed.");
        }

        public static bool IsStandardCoverageLimitReached(bool blackoutRoom, int playerCardCount, int totalMarkedPlayableSquares, int selectedCardCount)
        {
            if (blackoutRoom || playerCardCount <= 0)
            {
                return false;
            }

            float averageMarked = (float)totalMarkedPlayableSquares / playerCardCount;
            return averageMarked >= GetStandardCoverageLimit(selectedCardCount);
        }

        public static int GetStandardCoverageLimit(int selectedCardCount)
        {
            if (selectedCardCount <= 1) return 17;
            if (selectedCardCount <= 2) return 16;
            if (selectedCardCount <= 4) return 15;
            return 14;
        }
    }
}
