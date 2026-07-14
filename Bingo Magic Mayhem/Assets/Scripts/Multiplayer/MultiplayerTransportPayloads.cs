using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Transport-neutral payload shapes for future UGS or other backend sync.
    /// These are not transport bindings yet; they define the data contract surface
    /// the controller/lobby/gameplay layers can converge on.
    /// </summary>
    public sealed class MultiplayerRoomSyncPayload
    {
        public string RoomId = "";
        public string RoomCode = "";
        public string HostPlayerId = "";
        public MultiplayerRoomLifecycleState RoomState = MultiplayerRoomLifecycleState.None;
        public int RealmIndex;
        public int RoomIndex;
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public readonly List<MultiplayerParticipantSyncPayload> Participants = new List<MultiplayerParticipantSyncPayload>();
    }

    public sealed class MultiplayerParticipantSyncPayload
    {
        public string PlayerId = "";
        public string DisplayName = "";
        public bool IsHost;
        public bool IsReady;
        public bool IsConnected = true;
    }

    public sealed class MultiplayerReadinessUpdatePayload
    {
        public string RoomId = "";
        public string PlayerId = "";
        public bool IsReady;
        public long UpdatedUtcTicks;
    }

    public sealed class MultiplayerMatchStartPayload
    {
        public string RoomId = "";
        public string MatchId = "";
        public string HostPlayerId = "";
        public int RealmIndex;
        public int RoomIndex;
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public int RoundSeed;
        public int MaxCallCount;
        public float AutoCallIntervalSeconds;
    }

    public sealed class MultiplayerCallBroadcastPayload
    {
        public string MatchId = "";
        public int CallIndex;
        public int CalledNumber;
        public long EmittedUtcTicks;
    }

    public sealed class MultiplayerClaimSubmitPayload
    {
        public string MatchId = "";
        public string PlayerId = "";
        public MatchClaimType ClaimType = MatchClaimType.Bingo;
        public int CardIndex;
        public int ClaimCallIndex;
        public string IdempotencyKey = "";
        public readonly List<string> MarkedCellKeys = new List<string>();
        public readonly List<int> ClaimedNumbers = new List<int>();
    }

    public sealed class MultiplayerClaimResolutionPayload
    {
        public string MatchId = "";
        public string PlayerId = "";
        public MatchClaimType ClaimType = MatchClaimType.Bingo;
        public MatchClaimResolutionKind Result = MatchClaimResolutionKind.Rejected;
        public int AcceptedCallIndex = -1;
        public int ValidatedNumberCount;
        public string Reason = "";
    }

    public sealed class MultiplayerMatchEndPayload
    {
        public string MatchId = "";
        public BingoMagicMayhem.Rounds.BingoRoundEndReasonKind EndReasonKind = BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None;
        public string EndReason = "";
        public int FinalCallIndex = -1;
        public readonly List<string> WheelspinEntitledPlayerIds = new List<string>();
    }
}
