using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Centralized construction seam for room/session sync adapters. This keeps
    /// transport selection out of higher-level runtime composition.
    /// </summary>
    public interface IPrototypeMultiplayerRoomSessionSyncAdapterFactory
    {
        IMultiplayerRoomSessionSyncAdapter CreateAdapter(PrototypeMultiplayerBackendMode backendMode);
    }

    /// <summary>
    /// Default sync-adapter selector. UGS currently resolves to a non-live adapter
    /// boundary that safely delegates to the local in-memory transport seam.
    /// </summary>
    public sealed class PrototypeMultiplayerRoomSessionSyncAdapterFactory : IPrototypeMultiplayerRoomSessionSyncAdapterFactory
    {
        public IMultiplayerRoomSessionSyncAdapter CreateAdapter(PrototypeMultiplayerBackendMode backendMode)
        {
            switch (backendMode)
            {
                case PrototypeMultiplayerBackendMode.Ugs:
                    return new PrototypeMultiplayerUgsRoomSessionSyncAdapter(new LocalInMemoryMultiplayerRoomSessionSyncAdapter());
                case PrototypeMultiplayerBackendMode.Local:
                default:
                    return new LocalInMemoryMultiplayerRoomSessionSyncAdapter();
            }
        }
    }

    /// <summary>
    /// Non-live UGS-shaped room/session sync adapter. This is the transport-facing
    /// boundary for future Lobby/Relay-backed session synchronization while the
    /// prototype still delegates safely to local in-memory sync behavior.
    /// </summary>
    public sealed class PrototypeMultiplayerUgsRoomSessionSyncAdapter : IMultiplayerRoomSessionSyncAdapter
    {
        private readonly IMultiplayerRoomSessionSyncAdapter localFallbackAdapter;

        public PrototypeMultiplayerUgsRoomSessionSyncAdapter(IMultiplayerRoomSessionSyncAdapter localFallbackAdapter)
        {
            this.localFallbackAdapter = localFallbackAdapter ?? throw new ArgumentNullException(nameof(localFallbackAdapter));
        }

        public IMultiplayerRoomSessionSyncAdapter LocalFallbackAdapter => localFallbackAdapter;
        public MultiplayerRoomSyncPayload LatestRoomSync => localFallbackAdapter.LatestRoomSync;
        public MultiplayerReadinessUpdatePayload LatestReadinessUpdate => localFallbackAdapter.LatestReadinessUpdate;
        public MultiplayerMatchStartPayload LatestMatchStart => localFallbackAdapter.LatestMatchStart;
        public MultiplayerCallBroadcastPayload LatestCallBroadcast => localFallbackAdapter.LatestCallBroadcast;
        public MultiplayerClaimSubmitPayload LatestClaimSubmit => localFallbackAdapter.LatestClaimSubmit;
        public MultiplayerClaimResolutionPayload LatestClaimResolution => localFallbackAdapter.LatestClaimResolution;
        public MultiplayerMatchEndPayload LatestMatchEnd => localFallbackAdapter.LatestMatchEnd;
        public IReadOnlyList<MultiplayerRoomSyncPayload> RoomSyncLog => localFallbackAdapter.RoomSyncLog;
        public IReadOnlyList<MultiplayerReadinessUpdatePayload> ReadinessLog => localFallbackAdapter.ReadinessLog;
        public IReadOnlyList<MultiplayerMatchStartPayload> MatchStartLog => localFallbackAdapter.MatchStartLog;
        public IReadOnlyList<MultiplayerCallBroadcastPayload> CallBroadcastLog => localFallbackAdapter.CallBroadcastLog;
        public IReadOnlyList<MultiplayerClaimSubmitPayload> ClaimSubmitLog => localFallbackAdapter.ClaimSubmitLog;
        public IReadOnlyList<MultiplayerClaimResolutionPayload> ClaimResolutionLog => localFallbackAdapter.ClaimResolutionLog;
        public IReadOnlyList<MultiplayerMatchEndPayload> MatchEndLog => localFallbackAdapter.MatchEndLog;
        public IMultiplayerRoomSessionMirrorView Mirror => localFallbackAdapter.Mirror;

        public void PublishRoomSync(MultiplayerRoomSyncPayload payload) => localFallbackAdapter.PublishRoomSync(payload);
        public void PublishReadinessUpdate(MultiplayerReadinessUpdatePayload payload) => localFallbackAdapter.PublishReadinessUpdate(payload);
        public void PublishMatchStart(MultiplayerMatchStartPayload payload) => localFallbackAdapter.PublishMatchStart(payload);
        public void PublishCallBroadcast(MultiplayerCallBroadcastPayload payload) => localFallbackAdapter.PublishCallBroadcast(payload);
        public void PublishClaimSubmit(MultiplayerClaimSubmitPayload payload) => localFallbackAdapter.PublishClaimSubmit(payload);
        public void PublishClaimResolution(MultiplayerClaimResolutionPayload payload) => localFallbackAdapter.PublishClaimResolution(payload);
        public void PublishMatchEnd(MultiplayerMatchEndPayload payload) => localFallbackAdapter.PublishMatchEnd(payload);
        public void ApplyReceivedRoomSync(MultiplayerRoomSyncPayload payload) => localFallbackAdapter.ApplyReceivedRoomSync(payload);
        public void ApplyReceivedReadinessUpdate(MultiplayerReadinessUpdatePayload payload) => localFallbackAdapter.ApplyReceivedReadinessUpdate(payload);
        public void ApplyReceivedMatchStart(MultiplayerMatchStartPayload payload) => localFallbackAdapter.ApplyReceivedMatchStart(payload);
        public void ApplyReceivedCallBroadcast(MultiplayerCallBroadcastPayload payload) => localFallbackAdapter.ApplyReceivedCallBroadcast(payload);
        public void ApplyReceivedClaimSubmit(MultiplayerClaimSubmitPayload payload) => localFallbackAdapter.ApplyReceivedClaimSubmit(payload);
        public void ApplyReceivedClaimResolution(MultiplayerClaimResolutionPayload payload) => localFallbackAdapter.ApplyReceivedClaimResolution(payload);
        public void ApplyReceivedMatchEnd(MultiplayerMatchEndPayload payload) => localFallbackAdapter.ApplyReceivedMatchEnd(payload);
        public void Reset() => localFallbackAdapter.Reset();
    }
}
