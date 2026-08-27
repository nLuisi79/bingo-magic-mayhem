using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Presentation-facing seam for prototype multiplayer. This consolidates the
    /// UI/gameplay bridge actions and display-model reads so large UI roots do not
    /// couple themselves to runtime construction details.
    /// </summary>
    public sealed class PrototypeMultiplayerPresentationState
    {
        public PrototypeMultiplayerPresentationState(
            MultiplayerLobbyDisplayModel lobby,
            MultiplayerLobbyStatusDisplayModel lobbyStatus,
            MultiplayerRoomSessionDisplayModel session,
            MultiplayerGameplayRoundDisplayModel gameplayRound,
            MultiplayerRoomSessionSnapshot snapshot,
            LocalAuthoritativeMatchSummary matchSummary)
            : this(
                lobby,
                lobbyStatus,
                session,
                gameplayRound,
                null,
                snapshot,
                matchSummary)
        {
        }

        public PrototypeMultiplayerPresentationState(
            MultiplayerLobbyDisplayModel lobby,
            MultiplayerLobbyStatusDisplayModel lobbyStatus,
            MultiplayerRoomSessionDisplayModel session,
            MultiplayerGameplayRoundDisplayModel gameplayRound,
            PrototypeMultiplayerUiSurfaceModel uiSurface,
            MultiplayerRoomSessionSnapshot snapshot,
            LocalAuthoritativeMatchSummary matchSummary)
        {
            Lobby = lobby;
            LobbyStatus = lobbyStatus;
            Session = session;
            GameplayRound = gameplayRound;
            UiSurface = uiSurface;
            Snapshot = snapshot;
            MatchSummary = matchSummary;
        }

        public MultiplayerLobbyDisplayModel Lobby { get; }
        public MultiplayerLobbyStatusDisplayModel LobbyStatus { get; }
        public MultiplayerRoomSessionDisplayModel Session { get; }
        public MultiplayerGameplayRoundDisplayModel GameplayRound { get; }
        public PrototypeMultiplayerUiSurfaceModel UiSurface { get; }
        public MultiplayerRoomSessionSnapshot Snapshot { get; }
        public LocalAuthoritativeMatchSummary MatchSummary { get; }
    }

    /// <summary>
    /// Compact UI-facing multiplayer surface that summarizes the current
    /// lifecycle, reconnect pressure, and replay/post-round gating rules.
    /// Views should prefer this model for top-level screen state rather than
    /// re-deriving multiplayer rules from multiple lower-level display models.
    /// </summary>
    public sealed class PrototypeMultiplayerUiSurfaceModel
    {
        public PrototypeMultiplayerUiSurfaceModel(
            string titleLabel,
            string lifecycleSummaryLabel,
            string connectionSummaryLabel,
            string replayActionLabel,
            string postRoundActionLabel,
            int connectedParticipantCount,
            int disconnectedParticipantCount,
            int readyParticipantCount,
            int pendingReadyParticipantCount,
            bool hasRoom,
            bool hasActiveRound,
            bool isWaitingForReconnect,
            bool isReplayBlockedByPostRoundHandoff,
            bool hasPendingJackpotHandoff,
            bool canResolvePostRoundHandoff,
            bool canReturnToLobby,
            bool canStartRoundLocally)
        {
            TitleLabel = titleLabel ?? "";
            LifecycleSummaryLabel = lifecycleSummaryLabel ?? "";
            ConnectionSummaryLabel = connectionSummaryLabel ?? "";
            ReplayActionLabel = replayActionLabel ?? "";
            PostRoundActionLabel = postRoundActionLabel ?? "";
            ConnectedParticipantCount = connectedParticipantCount;
            DisconnectedParticipantCount = disconnectedParticipantCount;
            ReadyParticipantCount = readyParticipantCount;
            PendingReadyParticipantCount = pendingReadyParticipantCount;
            HasRoom = hasRoom;
            HasActiveRound = hasActiveRound;
            IsWaitingForReconnect = isWaitingForReconnect;
            IsReplayBlockedByPostRoundHandoff = isReplayBlockedByPostRoundHandoff;
            HasPendingJackpotHandoff = hasPendingJackpotHandoff;
            CanResolvePostRoundHandoff = canResolvePostRoundHandoff;
            CanReturnToLobby = canReturnToLobby;
            CanStartRoundLocally = canStartRoundLocally;
        }

        public string TitleLabel { get; }
        public string LifecycleSummaryLabel { get; }
        public string ConnectionSummaryLabel { get; }
        public string ReplayActionLabel { get; }
        public string PostRoundActionLabel { get; }
        public int ConnectedParticipantCount { get; }
        public int DisconnectedParticipantCount { get; }
        public int ReadyParticipantCount { get; }
        public int PendingReadyParticipantCount { get; }
        public bool HasRoom { get; }
        public bool HasActiveRound { get; }
        public bool IsWaitingForReconnect { get; }
        public bool IsReplayBlockedByPostRoundHandoff { get; }
        public bool HasPendingJackpotHandoff { get; }
        public bool CanResolvePostRoundHandoff { get; }
        public bool CanReturnToLobby { get; }
        public bool CanStartRoundLocally { get; }
    }

    public static class PrototypeMultiplayerUiSurfacePresenter
    {
        public static PrototypeMultiplayerUiSurfaceModel Build(PrototypeMultiplayerPresentationState state)
        {
            MultiplayerRoomSessionDisplayModel session = state?.Session;
            if (session == null || !session.HasRoom)
            {
                return new PrototypeMultiplayerUiSurfaceModel(
                    "Multiplayer offline",
                    "No room is active.",
                    "0 connected  |  0 disconnected",
                    "Create a room to begin multiplayer.",
                    "No post-round flow is active.",
                    0,
                    0,
                    0,
                    0,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false);
            }

            int participantCount = session.Participants?.Count ?? 0;
            int disconnectedCount = participantCount - session.ConnectedParticipantCount;
            if (disconnectedCount < 0)
            {
                disconnectedCount = 0;
            }

            bool hasPendingJackpotHandoff = session.PostRoundSequenceState == MultiplayerPostRoundSequenceState.JackpotPending;
            bool canResolvePostRoundHandoff = hasPendingJackpotHandoff;
            bool isReplayBlockedByPostRoundHandoff = hasPendingJackpotHandoff;
            bool isWaitingForReconnect = disconnectedCount > 0;
            bool canReturnToLobby = session.HasRoom
                && !session.HasActiveMatch
                && !isReplayBlockedByPostRoundHandoff;

            return new PrototypeMultiplayerUiSurfaceModel(
                BuildTitleLabel(session),
                BuildLifecycleSummaryLabel(session, isWaitingForReconnect, hasPendingJackpotHandoff),
                BuildConnectionSummaryLabel(session, disconnectedCount),
                BuildReplayActionLabel(session, isWaitingForReconnect, isReplayBlockedByPostRoundHandoff, canReturnToLobby),
                BuildPostRoundActionLabel(session, hasPendingJackpotHandoff),
                session.ConnectedParticipantCount,
                disconnectedCount,
                session.ReadyParticipantCount,
                session.PendingReadyParticipantCount,
                session.HasRoom,
                session.HasActiveMatch,
                isWaitingForReconnect,
                isReplayBlockedByPostRoundHandoff,
                hasPendingJackpotHandoff,
                canResolvePostRoundHandoff,
                canReturnToLobby,
                session.CanStartMatchLocally && !isReplayBlockedByPostRoundHandoff);
        }

        private static string BuildTitleLabel(MultiplayerRoomSessionDisplayModel session)
        {
            return string.IsNullOrEmpty(session.HostDisplayName)
                ? $"Room {session.RoomCode}"
                : $"{session.HostDisplayName}'s room  |  {session.RoomCode}";
        }

        private static string BuildLifecycleSummaryLabel(
            MultiplayerRoomSessionDisplayModel session,
            bool isWaitingForReconnect,
            bool hasPendingJackpotHandoff)
        {
            if (hasPendingJackpotHandoff)
            {
                return $"{session.MatchStateLabel}  |  Jackpot handoff pending";
            }

            if (isWaitingForReconnect)
            {
                return $"{session.MatchStateLabel}  |  Waiting on reconnection";
            }

            switch (session.GameplayFlowState)
            {
                case MultiplayerGameplayFlowState.WaitingForPlayers:
                    return "Lobby open  |  Waiting for more players";
                case MultiplayerGameplayFlowState.WaitingForReadyPlayers:
                    return "Lobby open  |  Waiting for ready players";
                case MultiplayerGameplayFlowState.ReadyToStart:
                    return "Lobby open  |  Ready to start";
                case MultiplayerGameplayFlowState.MatchInProgress:
                    return "Round active  |  Authority live";
                case MultiplayerGameplayFlowState.MatchEnded:
                    return "Round ended  |  Awaiting next step";
                default:
                    return session.RoomStateLabel;
            }
        }

        private static string BuildConnectionSummaryLabel(MultiplayerRoomSessionDisplayModel session, int disconnectedCount)
        {
            return $"{session.ConnectedParticipantCount} connected  |  {disconnectedCount} disconnected  |  {session.PendingReadyParticipantCount} pending ready";
        }

        private static string BuildReplayActionLabel(
            MultiplayerRoomSessionDisplayModel session,
            bool isWaitingForReconnect,
            bool isReplayBlockedByPostRoundHandoff,
            bool canReturnToLobby)
        {
            if (isReplayBlockedByPostRoundHandoff)
            {
                return "Resolve jackpot handoff before returning the room to lobby.";
            }

            if (session.HasActiveMatch)
            {
                return "Round is still active.";
            }

            if (canReturnToLobby && session.GameplayFlowState == MultiplayerGameplayFlowState.MatchEnded)
            {
                return "Replay ready. Return the room to lobby for the next round.";
            }

            if (session.CanStartMatchLocally)
            {
                return "All connected players are ready. Host may start.";
            }

            if (isWaitingForReconnect)
            {
                return "Wait for disconnected players to reconnect or update lobby readiness.";
            }

            return session.HostStartHint;
        }

        private static string BuildPostRoundActionLabel(
            MultiplayerRoomSessionDisplayModel session,
            bool hasPendingJackpotHandoff)
        {
            if (hasPendingJackpotHandoff)
            {
                return session.JackpotHandoffLabel;
            }

            if (session.PostRoundSequenceState == MultiplayerPostRoundSequenceState.JackpotCompleted)
            {
                return "Post-round handoff complete. Lobby replay may resume.";
            }

            if (session.PostRoundSequenceState == MultiplayerPostRoundSequenceState.JackpotNotEligible)
            {
                return "No jackpot handoff is queued.";
            }

            return session.LastEventSummary;
        }
    }

    public sealed class PrototypeMultiplayerRoundEndPublishResult
    {
        public PrototypeMultiplayerRoundEndPublishResult(bool wasPublished, PrototypeMultiplayerPresentationState presentationState)
        {
            WasPublished = wasPublished;
            PresentationState = presentationState;
        }

        public bool WasPublished { get; }
        public PrototypeMultiplayerPresentationState PresentationState { get; }
        public MatchEndEvent LatestMatchEnd => PrototypeMultiplayerPresentationStateInspector.GetLatestMatchEnd(PresentationState);
        public MultiplayerAuthorityOutcomeModel Outcome => MultiplayerAuthorityOutcomePresenter.BuildRoundEndOutcome(LatestMatchEnd);
    }

    public sealed class PrototypeMultiplayerClaimSubmitResult
    {
        public PrototypeMultiplayerClaimSubmitResult(
            MatchClaimResolution resolution,
            PrototypeMultiplayerPresentationState presentationState)
        {
            Resolution = resolution;
            PresentationState = presentationState;
        }

        public MatchClaimResolution Resolution { get; }
        public PrototypeMultiplayerPresentationState PresentationState { get; }
        public MultiplayerAuthorityOutcomeModel Outcome => MultiplayerAuthorityOutcomePresenter.BuildClaimOutcome(Resolution);
    }

    public sealed class PrototypeMultiplayerCallResult
    {
        public PrototypeMultiplayerCallResult(
            MatchCallEvent callEvent,
            PrototypeMultiplayerPresentationState presentationState)
        {
            CallEvent = callEvent;
            PresentationState = presentationState;
        }

        public MatchCallEvent CallEvent { get; }
        public PrototypeMultiplayerPresentationState PresentationState { get; }
    }

    public sealed class PrototypeMultiplayerPostRoundHandoffResult
    {
        public PrototypeMultiplayerPostRoundHandoffResult(
            MatchEndEvent endEvent,
            PrototypeMultiplayerPresentationState presentationState)
        {
            EndEvent = endEvent;
            PresentationState = presentationState;
        }

        public MatchEndEvent EndEvent { get; }
        public PrototypeMultiplayerPresentationState PresentationState { get; }
        public MultiplayerAuthorityOutcomeModel Outcome => MultiplayerAuthorityOutcomePresenter.BuildRoundEndOutcome(EndEvent);
    }

    public sealed class PrototypeMultiplayerLobbySyncRequest
    {
        public PrototypeMultiplayerLobbySyncRequest(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            HostDisplayName = hostDisplayName ?? "";
            RealmIndex = realmIndex;
            RoomIndex = roomIndex;
            SelectedCardCount = selectedCardCount;
            ManaBetPerCard = manaBetPerCard;
        }

        public string HostDisplayName { get; }
        public int RealmIndex { get; }
        public int RoomIndex { get; }
        public int SelectedCardCount { get; }
        public int ManaBetPerCard { get; }
    }

    public sealed class PrototypeMultiplayerRoundStartRequest
    {
        public PrototypeMultiplayerRoundStartRequest(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            HostDisplayName = hostDisplayName ?? "";
            RealmIndex = realmIndex;
            RoomIndex = roomIndex;
            SelectedCardCount = selectedCardCount;
            ManaBetPerCard = manaBetPerCard;
            RoundSeed = roundSeed;
            MaxCallCount = maxCallCount;
            AutoCallIntervalSeconds = autoCallIntervalSeconds;
        }

        public string HostDisplayName { get; }
        public int RealmIndex { get; }
        public int RoomIndex { get; }
        public int SelectedCardCount { get; }
        public int ManaBetPerCard { get; }
        public int RoundSeed { get; }
        public int MaxCallCount { get; }
        public float AutoCallIntervalSeconds { get; }
    }

    public sealed class PrototypeMultiplayerObservedCallRequest
    {
        public PrototypeMultiplayerObservedCallRequest(bool roundIsActive, int calledNumber)
        {
            RoundIsActive = roundIsActive;
            CalledNumber = calledNumber;
        }

        public bool RoundIsActive { get; }
        public int CalledNumber { get; }
    }

    public sealed class PrototypeMultiplayerAuthoritativeCallRequest
    {
        public PrototypeMultiplayerAuthoritativeCallRequest(bool roundIsActive, long emittedUtcTicks = 0)
        {
            RoundIsActive = roundIsActive;
            EmittedUtcTicks = emittedUtcTicks;
        }

        public bool RoundIsActive { get; }
        public long EmittedUtcTicks { get; }
    }

    public sealed class PrototypeMultiplayerBingoClaimRequest
    {
        public PrototypeMultiplayerBingoClaimRequest(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string playerId = "",
            string idempotencyKey = "")
        {
            RoundIsActive = roundIsActive;
            CardIndex = cardIndex;
            ClaimCallIndex = claimCallIndex;
            NewBingoCount = newBingoCount;
            MarkedCellKeys = markedCellKeys;
            ClaimedNumbers = claimedNumbers;
            PlayerId = playerId ?? "";
            IdempotencyKey = idempotencyKey ?? "";
        }

        public bool RoundIsActive { get; }
        public int CardIndex { get; }
        public int ClaimCallIndex { get; }
        public int NewBingoCount { get; }
        public IReadOnlyList<string> MarkedCellKeys { get; }
        public IReadOnlyList<int> ClaimedNumbers { get; }
        public string PlayerId { get; }
        public string IdempotencyKey { get; }
    }

    public sealed class PrototypeMultiplayerJackpotClaimRequest
    {
        public PrototypeMultiplayerJackpotClaimRequest(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string playerId = "",
            string idempotencyKey = "")
        {
            RoundIsActive = roundIsActive;
            CardIndex = cardIndex;
            ClaimCallIndex = claimCallIndex;
            ClaimedNumbers = claimedNumbers;
            PlayerId = playerId ?? "";
            IdempotencyKey = idempotencyKey ?? "";
        }

        public bool RoundIsActive { get; }
        public int CardIndex { get; }
        public int ClaimCallIndex { get; }
        public IReadOnlyList<int> ClaimedNumbers { get; }
        public string PlayerId { get; }
        public string IdempotencyKey { get; }
    }

    public sealed class PrototypeMultiplayerRoundEndRequest
    {
        public PrototypeMultiplayerRoundEndRequest(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin,
            IReadOnlyList<string> wheelspinEntitledPlayerIds = null)
        {
            AlreadyPublished = alreadyPublished;
            Decision = decision;
            HostEarnedWheelspin = hostEarnedWheelspin;
            WheelspinEntitledPlayerIds = wheelspinEntitledPlayerIds;
        }

        public bool AlreadyPublished { get; }
        public BingoMagicMayhem.Rounds.BingoRoundEndDecision Decision { get; }
        public bool HostEarnedWheelspin { get; }
        public IReadOnlyList<string> WheelspinEntitledPlayerIds { get; }
    }

    /// <summary>
    /// Local-only relay helper that advances published room/session sync payloads into
    /// the receive/apply mirror so guest-facing presentation can be exercised without a
    /// live transport layer.
    /// </summary>
    public sealed class PrototypeMultiplayerMirrorRelay
    {
        private readonly IMultiplayerRoomSessionSyncAdapter syncAdapter;
        private int relayedRoomSyncCount;
        private int relayedReadinessCount;
        private int relayedMatchStartCount;
        private int relayedCallBroadcastCount;
        private int relayedClaimSubmitCount;
        private int relayedClaimResolutionCount;
        private int relayedMatchEndCount;

        public PrototypeMultiplayerMirrorRelay(IMultiplayerRoomSessionSyncAdapter syncAdapter)
        {
            this.syncAdapter = syncAdapter ?? throw new ArgumentNullException(nameof(syncAdapter));
        }

        public void RelayUnseenPublishedEvents()
        {
            RelayRange(syncAdapter.RoomSyncLog, ref relayedRoomSyncCount, syncAdapter.ApplyReceivedRoomSync);
            RelayRange(syncAdapter.ReadinessLog, ref relayedReadinessCount, syncAdapter.ApplyReceivedReadinessUpdate);
            RelayRange(syncAdapter.MatchStartLog, ref relayedMatchStartCount, syncAdapter.ApplyReceivedMatchStart);
            RelayRange(syncAdapter.CallBroadcastLog, ref relayedCallBroadcastCount, syncAdapter.ApplyReceivedCallBroadcast);
            RelayRange(syncAdapter.ClaimSubmitLog, ref relayedClaimSubmitCount, syncAdapter.ApplyReceivedClaimSubmit);
            RelayRange(syncAdapter.ClaimResolutionLog, ref relayedClaimResolutionCount, syncAdapter.ApplyReceivedClaimResolution);
            RelayRange(syncAdapter.MatchEndLog, ref relayedMatchEndCount, syncAdapter.ApplyReceivedMatchEnd);
        }

        public void ResetProgress()
        {
            relayedRoomSyncCount = 0;
            relayedReadinessCount = 0;
            relayedMatchStartCount = 0;
            relayedCallBroadcastCount = 0;
            relayedClaimSubmitCount = 0;
            relayedClaimResolutionCount = 0;
            relayedMatchEndCount = 0;
        }

        private static void RelayRange<TPayload>(
            IReadOnlyList<TPayload> source,
            ref int relayedCount,
            Action<TPayload> receiver)
        {
            if (source == null || receiver == null)
            {
                return;
            }

            int safeRelayedCount = Math.Max(0, relayedCount);
            for (int index = safeRelayedCount; index < source.Count; index++)
            {
                receiver(source[index]);
            }

            relayedCount = source.Count;
        }
    }

    public static class PrototypeMultiplayerPresentationStateInspector
    {
        public static MatchEndEvent GetLatestMatchEnd(PrototypeMultiplayerPresentationState state)
        {
            if (state?.Snapshot?.MatchEndLog == null || state.Snapshot.MatchEndLog.Count <= 0)
            {
                return null;
            }

            return state.Snapshot.MatchEndLog[state.Snapshot.MatchEndLog.Count - 1];
        }
    }

    public sealed class PrototypeMultiplayerPresentationFacade
    {
        private readonly PrototypeMultiplayerRoomSessionController controller;
        private readonly PrototypeMultiplayerGameplayBridge gameplayBridge;
        private readonly PrototypeMultiplayerMirrorRelay mirrorRelay;

        public PrototypeMultiplayerPresentationFacade(
            PrototypeMultiplayerRoomSessionController controller,
            PrototypeMultiplayerGameplayBridge gameplayBridge)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.gameplayBridge = gameplayBridge ?? throw new ArgumentNullException(nameof(gameplayBridge));
            mirrorRelay = new PrototypeMultiplayerMirrorRelay(this.controller.SyncAdapter);
        }

        public PrototypeMultiplayerRoomSessionController Controller => controller;
        public PrototypeMultiplayerGameplayBridge GameplayBridge => gameplayBridge;

        public void SyncLobby(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            gameplayBridge.SyncLobby(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
        }

        public PrototypeMultiplayerPresentationState SyncLobbyAndBuildState(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard)
        {
            SyncLobby(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerPresentationState SyncLobbyAndBuildState(PrototypeMultiplayerLobbySyncRequest request)
        {
            if (request == null)
            {
                return BuildPresentationState();
            }

            return SyncLobbyAndBuildState(
                request.HostDisplayName,
                request.RealmIndex,
                request.RoomIndex,
                request.SelectedCardCount,
                request.ManaBetPerCard);
        }

        public void BeginAuthoritativeRound(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            gameplayBridge.BeginAuthoritativeRound(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds);
        }

        public PrototypeMultiplayerPresentationState BeginAuthoritativeRoundAndBuildState(
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds)
        {
            BeginAuthoritativeRound(
                hostDisplayName,
                realmIndex,
                roomIndex,
                selectedCardCount,
                manaBetPerCard,
                roundSeed,
                maxCallCount,
                autoCallIntervalSeconds);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerPresentationState BeginAuthoritativeRoundAndBuildState(
            PrototypeMultiplayerRoundStartRequest request)
        {
            if (request == null)
            {
                return BuildPresentationState();
            }

            return BeginAuthoritativeRoundAndBuildState(
                request.HostDisplayName,
                request.RealmIndex,
                request.RoomIndex,
                request.SelectedCardCount,
                request.ManaBetPerCard,
                request.RoundSeed,
                request.MaxCallCount,
                request.AutoCallIntervalSeconds);
        }

        public MatchCallEvent TryRecordObservedCall(bool roundIsActive, int calledNumber)
        {
            return gameplayBridge.TryRecordObservedCall(roundIsActive, calledNumber);
        }

        public MatchCallEvent TryEmitAuthoritativeCall(bool roundIsActive, long emittedUtcTicks = 0)
        {
            return gameplayBridge.TryEmitAuthoritativeCall(roundIsActive, emittedUtcTicks);
        }

        public PrototypeMultiplayerPresentationState TryEmitAuthoritativeCallAndBuildState(bool roundIsActive, long emittedUtcTicks = 0)
        {
            TryEmitAuthoritativeCall(roundIsActive, emittedUtcTicks);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerCallResult TryEmitAuthoritativeCallAndBuildResult(bool roundIsActive, long emittedUtcTicks = 0)
        {
            MatchCallEvent callEvent = TryEmitAuthoritativeCall(roundIsActive, emittedUtcTicks);
            return new PrototypeMultiplayerCallResult(callEvent, BuildPresentationState());
        }

        public PrototypeMultiplayerCallResult TryEmitAuthoritativeCallAndBuildResult(
            PrototypeMultiplayerAuthoritativeCallRequest request)
        {
            if (request == null)
            {
                return new PrototypeMultiplayerCallResult(null, BuildPresentationState());
            }

            return TryEmitAuthoritativeCallAndBuildResult(request.RoundIsActive, request.EmittedUtcTicks);
        }

        public PrototypeMultiplayerPresentationState TryRecordObservedCallAndBuildState(bool roundIsActive, int calledNumber)
        {
            TryRecordObservedCall(roundIsActive, calledNumber);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerPresentationState TryRecordObservedCallAndBuildState(
            PrototypeMultiplayerObservedCallRequest request)
        {
            if (request == null)
            {
                return BuildPresentationState();
            }

            return TryRecordObservedCallAndBuildState(request.RoundIsActive, request.CalledNumber);
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            return gameplayBridge.TrySubmitBingoClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
        }

        public MatchClaimResolution TrySubmitBingoClaim(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            return gameplayBridge.TrySubmitBingoClaim(
                roundIsActive,
                playerId,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers,
                idempotencyKey);
        }

        public PrototypeMultiplayerPresentationState TrySubmitBingoClaimAndBuildState(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            TrySubmitBingoClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitBingoClaimAndBuildResult(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers)
        {
            MatchClaimResolution resolution = TrySubmitBingoClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers);
            return new PrototypeMultiplayerClaimSubmitResult(resolution, BuildPresentationState());
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitBingoClaimAndBuildResult(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            int newBingoCount,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            MatchClaimResolution resolution = TrySubmitBingoClaim(
                roundIsActive,
                playerId,
                cardIndex,
                claimCallIndex,
                newBingoCount,
                markedCellKeys,
                claimedNumbers,
                idempotencyKey);
            return new PrototypeMultiplayerClaimSubmitResult(resolution, BuildPresentationState());
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitBingoClaimAndBuildResult(
            PrototypeMultiplayerBingoClaimRequest request)
        {
            if (request == null)
            {
                return new PrototypeMultiplayerClaimSubmitResult(null, BuildPresentationState());
            }

            return TrySubmitBingoClaimAndBuildResult(
                request.RoundIsActive,
                request.PlayerId,
                request.CardIndex,
                request.ClaimCallIndex,
                request.NewBingoCount,
                request.MarkedCellKeys,
                request.ClaimedNumbers,
                request.IdempotencyKey);
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            return gameplayBridge.TrySubmitJackpotStateClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
        }

        public MatchClaimResolution TrySubmitJackpotStateClaim(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            return gameplayBridge.TrySubmitJackpotStateClaim(
                roundIsActive,
                playerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                idempotencyKey);
        }

        public PrototypeMultiplayerPresentationState TrySubmitJackpotStateClaimAndBuildState(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            TrySubmitJackpotStateClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
            return BuildPresentationState();
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitJackpotStateClaimAndBuildResult(
            bool roundIsActive,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers)
        {
            MatchClaimResolution resolution = TrySubmitJackpotStateClaim(
                roundIsActive,
                cardIndex,
                claimCallIndex,
                claimedNumbers);
            return new PrototypeMultiplayerClaimSubmitResult(resolution, BuildPresentationState());
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitJackpotStateClaimAndBuildResult(
            bool roundIsActive,
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey = null)
        {
            MatchClaimResolution resolution = TrySubmitJackpotStateClaim(
                roundIsActive,
                playerId,
                cardIndex,
                claimCallIndex,
                claimedNumbers,
                idempotencyKey);
            return new PrototypeMultiplayerClaimSubmitResult(resolution, BuildPresentationState());
        }

        public PrototypeMultiplayerClaimSubmitResult TrySubmitJackpotStateClaimAndBuildResult(
            PrototypeMultiplayerJackpotClaimRequest request)
        {
            if (request == null)
            {
                return new PrototypeMultiplayerClaimSubmitResult(null, BuildPresentationState());
            }

            return TrySubmitJackpotStateClaimAndBuildResult(
                request.RoundIsActive,
                request.PlayerId,
                request.CardIndex,
                request.ClaimCallIndex,
                request.ClaimedNumbers,
                request.IdempotencyKey);
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            return gameplayBridge.TryPublishRoundEnd(
                alreadyPublished,
                decision,
                hostEarnedWheelspin);
        }

        public bool TryPublishRoundEnd(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            return gameplayBridge.TryPublishRoundEnd(
                alreadyPublished,
                decision,
                wheelspinEntitledPlayerIds);
        }

        public PrototypeMultiplayerRoundEndPublishResult TryPublishRoundEndAndBuildState(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            bool hostEarnedWheelspin)
        {
            bool wasPublished = TryPublishRoundEnd(
                alreadyPublished,
                decision,
                hostEarnedWheelspin);
            return new PrototypeMultiplayerRoundEndPublishResult(wasPublished, BuildPresentationState());
        }

        public PrototypeMultiplayerRoundEndPublishResult TryPublishRoundEndAndBuildState(
            PrototypeMultiplayerRoundEndRequest request)
        {
            if (request == null)
            {
                return new PrototypeMultiplayerRoundEndPublishResult(false, BuildPresentationState());
            }

            IReadOnlyList<string> wheelspinEntitledPlayerIds = request.WheelspinEntitledPlayerIds;
            if (wheelspinEntitledPlayerIds != null)
            {
                return TryPublishRoundEndAndBuildState(
                    request.AlreadyPublished,
                    request.Decision,
                    wheelspinEntitledPlayerIds);
            }

            return TryPublishRoundEndAndBuildState(
                request.AlreadyPublished,
                request.Decision,
                request.HostEarnedWheelspin);
        }

        public PrototypeMultiplayerRoundEndPublishResult TryPublishRoundEndAndBuildState(
            bool alreadyPublished,
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            bool wasPublished = TryPublishRoundEnd(
                alreadyPublished,
                decision,
                wheelspinEntitledPlayerIds);
            return new PrototypeMultiplayerRoundEndPublishResult(wasPublished, BuildPresentationState());
        }

        public PrototypeMultiplayerPostRoundHandoffResult ResolvePostRoundHandoffAndBuildState(string resolvedPlayerId)
        {
            MatchEndEvent endEvent = gameplayBridge.TryResolvePostRoundHandoff(resolvedPlayerId);
            return new PrototypeMultiplayerPostRoundHandoffResult(endEvent, BuildPresentationState());
        }

        public MultiplayerLobbyDisplayModel BuildLobbyDisplayModel()
        {
            return controller.BuildLobbyDisplayModel();
        }

        public MultiplayerRoomSessionDisplayModel BuildRoomSessionDisplayModel()
        {
            return controller.BuildRoomSessionDisplayModel();
        }

        public MultiplayerRoomSessionSnapshot BuildSessionSnapshot()
        {
            return controller.BuildSessionSnapshot();
        }

        public PrototypeMultiplayerUiSurfaceModel BuildUiSurfaceModel()
        {
            return PrototypeMultiplayerUiSurfacePresenter.Build(BuildPresentationState());
        }

        public int GetCurrentAuthoritativeCallIndex()
        {
            return controller.GetCurrentAuthoritativeCallIndex();
        }

        public void SyncMirroredStateFromPublishedEvents()
        {
            mirrorRelay.RelayUnseenPublishedEvents();
        }

        public MultiplayerRoomSessionSnapshot BuildMirroredSessionSnapshot()
        {
            SyncMirroredStateFromPublishedEvents();
            return MultiplayerRoomSessionSnapshotFactory.Build(controller.SyncAdapter.Mirror);
        }

        public LocalAuthoritativeMatchSummary BuildMatchSummary()
        {
            return controller.BuildMatchSummary();
        }

        public PrototypeMultiplayerPresentationState BuildPresentationState()
        {
            return BuildPresentationStateFromSnapshot(BuildSessionSnapshot(), BuildMatchSummary());
        }

        public PrototypeMultiplayerPresentationState BuildMirroredPresentationState()
        {
            return BuildPresentationStateFromSnapshot(BuildMirroredSessionSnapshot(), null);
        }

        private static PrototypeMultiplayerPresentationState BuildPresentationStateFromSnapshot(
            MultiplayerRoomSessionSnapshot snapshot,
            LocalAuthoritativeMatchSummary matchSummary)
        {
            MultiplayerRoomSessionDisplayModel sessionDisplayModel = MultiplayerRoomSessionPresenter.Build(snapshot);
            MultiplayerLobbyDisplayModel lobbyDisplayModel = MultiplayerLobbyPresenter.Build(sessionDisplayModel);
            MultiplayerLobbyStatusDisplayModel lobbyStatusDisplayModel = MultiplayerLobbyStatusPresenter.Build(lobbyDisplayModel);
            MultiplayerGameplayRoundDisplayModel gameplayRoundDisplayModel = MultiplayerGameplayRoundPresenter.Build(sessionDisplayModel);
            PrototypeMultiplayerPresentationState state = new PrototypeMultiplayerPresentationState(
                lobbyDisplayModel,
                lobbyStatusDisplayModel,
                sessionDisplayModel,
                gameplayRoundDisplayModel,
                null,
                snapshot,
                matchSummary);
            return new PrototypeMultiplayerPresentationState(
                state.Lobby,
                state.LobbyStatus,
                state.Session,
                state.GameplayRound,
                PrototypeMultiplayerUiSurfacePresenter.Build(state),
                state.Snapshot,
                state.MatchSummary);
        }
    }
}
