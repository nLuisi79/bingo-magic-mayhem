using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Transport-facing publishing seam for room/session lifecycle sync. The prototype
    /// uses an in-memory implementation now; a future UGS-backed adapter can replace it
    /// without changing the controller or gameplay/UI roots.
    /// </summary>
    public interface IMultiplayerRoomSessionSyncAdapter
    {
        MultiplayerRoomSyncPayload LatestRoomSync { get; }
        MultiplayerReadinessUpdatePayload LatestReadinessUpdate { get; }
        MultiplayerMatchStartPayload LatestMatchStart { get; }
        MultiplayerCallBroadcastPayload LatestCallBroadcast { get; }
        MultiplayerClaimSubmitPayload LatestClaimSubmit { get; }
        MultiplayerClaimResolutionPayload LatestClaimResolution { get; }
        MultiplayerMatchEndPayload LatestMatchEnd { get; }

        IReadOnlyList<MultiplayerRoomSyncPayload> RoomSyncLog { get; }
        IReadOnlyList<MultiplayerReadinessUpdatePayload> ReadinessLog { get; }
        IReadOnlyList<MultiplayerMatchStartPayload> MatchStartLog { get; }
        IReadOnlyList<MultiplayerCallBroadcastPayload> CallBroadcastLog { get; }
        IReadOnlyList<MultiplayerClaimSubmitPayload> ClaimSubmitLog { get; }
        IReadOnlyList<MultiplayerClaimResolutionPayload> ClaimResolutionLog { get; }
        IReadOnlyList<MultiplayerMatchEndPayload> MatchEndLog { get; }
        IMultiplayerRoomSessionMirrorView Mirror { get; }

        void PublishRoomSync(MultiplayerRoomSyncPayload payload);
        void PublishReadinessUpdate(MultiplayerReadinessUpdatePayload payload);
        void PublishMatchStart(MultiplayerMatchStartPayload payload);
        void PublishCallBroadcast(MultiplayerCallBroadcastPayload payload);
        void PublishClaimSubmit(MultiplayerClaimSubmitPayload payload);
        void PublishClaimResolution(MultiplayerClaimResolutionPayload payload);
        void PublishMatchEnd(MultiplayerMatchEndPayload payload);
        void ApplyReceivedRoomSync(MultiplayerRoomSyncPayload payload);
        void ApplyReceivedReadinessUpdate(MultiplayerReadinessUpdatePayload payload);
        void ApplyReceivedMatchStart(MultiplayerMatchStartPayload payload);
        void ApplyReceivedCallBroadcast(MultiplayerCallBroadcastPayload payload);
        void ApplyReceivedClaimSubmit(MultiplayerClaimSubmitPayload payload);
        void ApplyReceivedClaimResolution(MultiplayerClaimResolutionPayload payload);
        void ApplyReceivedMatchEnd(MultiplayerMatchEndPayload payload);
        void Reset();
    }

    public sealed class LocalInMemoryMultiplayerRoomSessionSyncAdapter : IMultiplayerRoomSessionSyncAdapter
    {
        private readonly List<MultiplayerRoomSyncPayload> roomSyncLog = new List<MultiplayerRoomSyncPayload>();
        private readonly List<MultiplayerReadinessUpdatePayload> readinessLog = new List<MultiplayerReadinessUpdatePayload>();
        private readonly List<MultiplayerMatchStartPayload> matchStartLog = new List<MultiplayerMatchStartPayload>();
        private readonly List<MultiplayerCallBroadcastPayload> callBroadcastLog = new List<MultiplayerCallBroadcastPayload>();
        private readonly List<MultiplayerClaimSubmitPayload> claimSubmitLog = new List<MultiplayerClaimSubmitPayload>();
        private readonly List<MultiplayerClaimResolutionPayload> claimResolutionLog = new List<MultiplayerClaimResolutionPayload>();
        private readonly List<MultiplayerMatchEndPayload> matchEndLog = new List<MultiplayerMatchEndPayload>();
        private readonly LocalMultiplayerRoomSessionMirror mirror = new LocalMultiplayerRoomSessionMirror();

        public MultiplayerRoomSyncPayload LatestRoomSync { get; private set; }
        public MultiplayerReadinessUpdatePayload LatestReadinessUpdate { get; private set; }
        public MultiplayerMatchStartPayload LatestMatchStart { get; private set; }
        public MultiplayerCallBroadcastPayload LatestCallBroadcast { get; private set; }
        public MultiplayerClaimSubmitPayload LatestClaimSubmit { get; private set; }
        public MultiplayerClaimResolutionPayload LatestClaimResolution { get; private set; }
        public MultiplayerMatchEndPayload LatestMatchEnd { get; private set; }

        public IReadOnlyList<MultiplayerRoomSyncPayload> RoomSyncLog => roomSyncLog;
        public IReadOnlyList<MultiplayerReadinessUpdatePayload> ReadinessLog => readinessLog;
        public IReadOnlyList<MultiplayerMatchStartPayload> MatchStartLog => matchStartLog;
        public IReadOnlyList<MultiplayerCallBroadcastPayload> CallBroadcastLog => callBroadcastLog;
        public IReadOnlyList<MultiplayerClaimSubmitPayload> ClaimSubmitLog => claimSubmitLog;
        public IReadOnlyList<MultiplayerClaimResolutionPayload> ClaimResolutionLog => claimResolutionLog;
        public IReadOnlyList<MultiplayerMatchEndPayload> MatchEndLog => matchEndLog;
        public IMultiplayerRoomSessionMirrorView Mirror => mirror;

        public void PublishRoomSync(MultiplayerRoomSyncPayload payload)
        {
            LatestRoomSync = payload ?? new MultiplayerRoomSyncPayload();
            roomSyncLog.Add(LatestRoomSync);
        }

        public void PublishReadinessUpdate(MultiplayerReadinessUpdatePayload payload)
        {
            LatestReadinessUpdate = payload ?? new MultiplayerReadinessUpdatePayload();
            readinessLog.Add(LatestReadinessUpdate);
        }

        public void PublishMatchStart(MultiplayerMatchStartPayload payload)
        {
            LatestMatchStart = payload ?? new MultiplayerMatchStartPayload();
            matchStartLog.Add(LatestMatchStart);
        }

        public void PublishCallBroadcast(MultiplayerCallBroadcastPayload payload)
        {
            LatestCallBroadcast = payload ?? new MultiplayerCallBroadcastPayload();
            callBroadcastLog.Add(LatestCallBroadcast);
        }

        public void PublishClaimSubmit(MultiplayerClaimSubmitPayload payload)
        {
            LatestClaimSubmit = payload ?? new MultiplayerClaimSubmitPayload();
            claimSubmitLog.Add(LatestClaimSubmit);
        }

        public void PublishClaimResolution(MultiplayerClaimResolutionPayload payload)
        {
            LatestClaimResolution = payload ?? new MultiplayerClaimResolutionPayload();
            claimResolutionLog.Add(LatestClaimResolution);
        }

        public void PublishMatchEnd(MultiplayerMatchEndPayload payload)
        {
            LatestMatchEnd = payload ?? new MultiplayerMatchEndPayload();
            matchEndLog.Add(LatestMatchEnd);
        }

        public void ApplyReceivedRoomSync(MultiplayerRoomSyncPayload payload)
        {
            mirror.ApplyRoomSync(payload);
        }

        public void ApplyReceivedReadinessUpdate(MultiplayerReadinessUpdatePayload payload)
        {
            mirror.ApplyReadinessUpdate(payload);
        }

        public void ApplyReceivedMatchStart(MultiplayerMatchStartPayload payload)
        {
            mirror.ApplyMatchStart(payload);
        }

        public void ApplyReceivedCallBroadcast(MultiplayerCallBroadcastPayload payload)
        {
            mirror.ApplyCallBroadcast(payload);
        }

        public void ApplyReceivedClaimSubmit(MultiplayerClaimSubmitPayload payload)
        {
            mirror.ApplyClaimSubmit(payload);
        }

        public void ApplyReceivedClaimResolution(MultiplayerClaimResolutionPayload payload)
        {
            mirror.ApplyClaimResolution(payload);
        }

        public void ApplyReceivedMatchEnd(MultiplayerMatchEndPayload payload)
        {
            mirror.ApplyMatchEnd(payload);
        }

        public void Reset()
        {
            LatestRoomSync = null;
            LatestReadinessUpdate = null;
            LatestMatchStart = null;
            LatestCallBroadcast = null;
            LatestClaimSubmit = null;
            LatestClaimResolution = null;
            LatestMatchEnd = null;

            roomSyncLog.Clear();
            readinessLog.Clear();
            matchStartLog.Clear();
            callBroadcastLog.Clear();
            claimSubmitLog.Clear();
            claimResolutionLog.Clear();
            matchEndLog.Clear();
            mirror.Reset();
        }
    }
}
