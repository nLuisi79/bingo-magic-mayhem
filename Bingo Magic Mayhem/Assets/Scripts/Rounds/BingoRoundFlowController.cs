using System.Collections.Generic;

namespace BingoMagicMayhem.Rounds
{
    /// <summary>
    /// Owns transient round runtime state so gameplay flow can be moved out of the
    /// prototype shell in smaller, behavior-preserving passes.
    /// </summary>
    public sealed class BingoRoundFlowController
    {
        private readonly BingoCaller caller = new BingoCaller();
        private readonly BingoRoundState roundState;
        private readonly Dictionary<int, float> calledAtTimes = new Dictionary<int, float>();

        public BingoRoundFlowController(BingoRoomRules rules)
        {
            roundState = new BingoRoundState(rules);
        }

        public bool IsActive => roundState.IsActive;
        public bool HasJackpotState => roundState.HasJackpotState;
        public float NextCallTimer => roundState.NextCallTimer;
        public int CalledCount => caller.CalledNumbers.Count;
        public IReadOnlyList<int> History => caller.History;

        public void BeginRound()
        {
            caller.Reset();
            calledAtTimes.Clear();
            roundState.StartRound();
        }

        public void StopRound()
        {
            roundState.StopRound();
        }

        public void ResetSession()
        {
            caller.Reset();
            calledAtTimes.Clear();
            roundState.StopRound();
        }

        public void MarkJackpotState()
        {
            roundState.MarkJackpotState();
        }

        public bool TickAutoCallTimer(float deltaTime)
        {
            return roundState.TickAutoCallTimer(deltaTime);
        }

        public bool TryCallNext(float calledAtTime, out int calledNumber)
        {
            if (!caller.TryCallNext(out calledNumber))
            {
                return false;
            }

            calledAtTimes[calledNumber] = calledAtTime;
            return true;
        }

        public bool IsNumberCalled(int number)
        {
            return caller.CalledNumbers.Contains(number);
        }

        public bool TryGetCalledAtTime(int number, out float calledAtTime)
        {
            return calledAtTimes.TryGetValue(number, out calledAtTime);
        }

        public bool IsMaxCallLimitReached(int maxBallCalls)
        {
            return caller.History.Count >= maxBallCalls;
        }
    }
}
