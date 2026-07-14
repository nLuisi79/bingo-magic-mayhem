using System;
using System.Collections.Generic;
using UnityEngine;

namespace BingoMagicMayhem.Multiplayer
{
    public enum MultiplayerRoomLifecycleState
    {
        None = 0,
        Lobby = 1,
        MatchInProgress = 2,
        Closed = 3
    }

    public enum MatchAuthorityLifecycleState
    {
        None = 0,
        Lobby = 1,
        InRound = 2,
        Ended = 3
    }

    public enum MatchClaimType
    {
        Bingo = 0,
        JackpotState = 1,
        Blackout = 2
    }

    public enum MatchClaimResolutionKind
    {
        Accepted = 0,
        Rejected = 1,
        Duplicate = 2,
        RoundClosed = 3
    }

    public sealed class MultiplayerParticipantSnapshot
    {
        public string PlayerId = "";
        public string DisplayName = "";
        public bool IsHost;
        public bool IsReady;
        public bool IsConnected = true;
    }

    public sealed class MultiplayerRoomSnapshot
    {
        public string RoomId = "";
        public string RoomCode = "";
        public MultiplayerRoomLifecycleState State = MultiplayerRoomLifecycleState.None;
        public string HostPlayerId = "";
        public int SelectedRealmIndex;
        public int SelectedRoomIndex;
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public readonly List<MultiplayerParticipantSnapshot> Participants = new List<MultiplayerParticipantSnapshot>();
    }

    public sealed class AuthoritativeMatchStartRequest
    {
        public int RealmIndex;
        public int RoomIndex;
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public int RoundSeed;
        public int MaxCallCount;
        public float AutoCallIntervalSeconds;
    }

    public sealed class MatchAuthorityState
    {
        public string MatchId = "";
        public string RoomId = "";
        public string HostPlayerId = "";
        public MatchAuthorityLifecycleState State = MatchAuthorityLifecycleState.None;
        public int RealmIndex;
        public int RoomIndex;
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public int RoundSeed;
        public int MaxCallCount;
        public float AutoCallIntervalSeconds;
        public int CurrentCallIndex = -1;
        public string PendingEndReason = "";
        public string EndedReason = "";
    }

    public sealed class MatchCallEvent
    {
        public string MatchId = "";
        public int CallIndex;
        public int CalledNumber;
        public long EmittedUtcTicks;
    }

    public sealed class MatchClaimAttempt
    {
        public string MatchId = "";
        public string PlayerId = "";
        public MatchClaimType ClaimType = MatchClaimType.Bingo;
        public int CardIndex;
        public int ClaimCallIndex;
        public readonly List<string> MarkedCellKeys = new List<string>();
        public readonly List<int> ClaimedNumbers = new List<int>();
        public string IdempotencyKey = "";
    }

    public sealed class MatchClaimResolution
    {
        public string MatchId = "";
        public string PlayerId = "";
        public MatchClaimType ClaimType = MatchClaimType.Bingo;
        public MatchClaimResolutionKind Result = MatchClaimResolutionKind.Rejected;
        public int AcceptedCallIndex = -1;
        public int ValidatedNumberCount;
        public string Reason = "";
    }

    public sealed class MatchEndEvent
    {
        public string MatchId = "";
        public BingoMagicMayhem.Rounds.BingoRoundEndReasonKind EndReasonKind = BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None;
        public string EndReason = "";
        public int FinalCallIndex = -1;
        public readonly List<string> WheelspinEntitledPlayerIds = new List<string>();
    }

    public interface IMultiplayerSessionFacade
    {
        MultiplayerRoomSnapshot CurrentRoom { get; }
        MatchAuthorityState CurrentMatch { get; }
        IReadOnlyList<MatchCallEvent> CallLog { get; }
        IReadOnlyList<MatchClaimResolution> ClaimLog { get; }
        IReadOnlyList<MatchEndEvent> MatchEndLog { get; }

        MultiplayerRoomSnapshot CreateRoom(string hostPlayerId, string hostDisplayName, int realmIndex, int roomIndex, int selectedCardCount, int manaBetPerCard);
        MultiplayerRoomSnapshot AddLocalParticipant(string playerId, string displayName);
        MultiplayerRoomSnapshot SetParticipantReady(string playerId, bool isReady);
        MatchAuthorityState StartAuthoritativeMatch(AuthoritativeMatchStartRequest request);
        MatchCallEvent EmitNextCall();
        MatchCallEvent RegisterObservedCall(int calledNumber, long emittedUtcTicks = 0);
        MatchClaimResolution ResolveClaim(MatchClaimAttempt attempt);
        MatchEndEvent PublishMatchEnd(BingoMagicMayhem.Rounds.BingoRoundEndDecision decision, IEnumerable<string> wheelspinEntitledPlayerIds);
    }

    /// <summary>
    /// Backend-facing contract for multiplayer room/session lifecycle behavior.
    /// This is the first service seam intended to remain stable while local or
    /// future UGS-backed implementations swap underneath.
    /// </summary>
    public interface IMultiplayerRoomSessionService
    {
        MultiplayerRoomSnapshot CurrentRoom { get; }

        MultiplayerRoomSnapshot EnsureLocalRoom(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard);

        MultiplayerRoomSnapshot AddOrUpdateLocalParticipant(string playerId, string displayName, bool isReady);

        MultiplayerRoomSnapshot EnsureLocalLobby(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            IReadOnlyList<LocalAuthoritativeMatchParticipant> localParticipants = null);

        MultiplayerRoomSnapshot SetParticipantReady(string playerId, bool isReady);

        MultiplayerRoomSnapshot EnsureHostReady(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard);

        MultiplayerRoomSnapshot BeginLocalAuthoritativeRound(
            string hostPlayerId,
            string hostDisplayName,
            int realmIndex,
            int roomIndex,
            int selectedCardCount,
            int manaBetPerCard,
            int roundSeed,
            int maxCallCount,
            float autoCallIntervalSeconds,
            IReadOnlyList<LocalAuthoritativeMatchParticipant> otherParticipants = null);
    }

    /// <summary>
    /// Backend-facing contract for authoritative multiplayer match events that occur
    /// after room/session setup has completed.
    /// </summary>
    public interface IMultiplayerMatchAuthorityService
    {
        MatchCallEvent RecordObservedCall(int calledNumber, long emittedUtcTicks = 0);

        MatchClaimResolution SubmitBingoClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<string> markedCellKeys,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey);

        MatchClaimResolution SubmitJackpotStateClaim(
            string playerId,
            int cardIndex,
            int claimCallIndex,
            IReadOnlyList<int> claimedNumbers,
            string idempotencyKey);

        MatchEndEvent PublishRoundEnd(
            BingoMagicMayhem.Rounds.BingoRoundEndDecision decision,
            IReadOnlyList<string> wheelspinEntitledPlayerIds);
    }

    /// <summary>
    /// Local-only greybox room/session facade. This intentionally avoids real transport
    /// and exists to prove authoritative call/claim/state contracts before any UGS or
    /// third-party networking choice is made.
    /// </summary>
    public sealed class LocalMultiplayerSessionFacade : IMultiplayerSessionFacade
    {
        private readonly BingoCaller caller = new BingoCaller();
        private readonly List<MatchCallEvent> callLog = new List<MatchCallEvent>();
        private readonly List<MatchClaimResolution> claimLog = new List<MatchClaimResolution>();
        private readonly List<MatchEndEvent> matchEndLog = new List<MatchEndEvent>();
        private readonly HashSet<string> resolvedClaimKeys = new HashSet<string>();
        private readonly HashSet<int> authoritativeCalledNumbers = new HashSet<int>();
        private int roomSequence;
        private int matchSequence;

        public MultiplayerRoomSnapshot CurrentRoom { get; private set; }
        public MatchAuthorityState CurrentMatch { get; private set; }
        public IReadOnlyList<MatchCallEvent> CallLog => callLog;
        public IReadOnlyList<MatchClaimResolution> ClaimLog => claimLog;
        public IReadOnlyList<MatchEndEvent> MatchEndLog => matchEndLog;

        public MultiplayerRoomSnapshot CreateRoom(string hostPlayerId, string hostDisplayName, int realmIndex, int roomIndex, int selectedCardCount, int manaBetPerCard)
        {
            CurrentRoom = new MultiplayerRoomSnapshot
            {
                RoomId = $"local_room_{++roomSequence}",
                RoomCode = BuildRoomCode(roomSequence),
                State = MultiplayerRoomLifecycleState.Lobby,
                HostPlayerId = hostPlayerId ?? "",
                SelectedRealmIndex = realmIndex,
                SelectedRoomIndex = roomIndex,
                SelectedCardCount = selectedCardCount,
                ManaBetPerCard = manaBetPerCard
            };
            CurrentRoom.Participants.Clear();
            CurrentRoom.Participants.Add(new MultiplayerParticipantSnapshot
            {
                PlayerId = hostPlayerId ?? "",
                DisplayName = string.IsNullOrEmpty(hostDisplayName) ? "Host" : hostDisplayName,
                IsHost = true,
                IsReady = false,
                IsConnected = true
            });

            CurrentMatch = null;
            callLog.Clear();
            claimLog.Clear();
            matchEndLog.Clear();
            resolvedClaimKeys.Clear();
            authoritativeCalledNumbers.Clear();
            caller.Reset();
            return CurrentRoom;
        }

        public MultiplayerRoomSnapshot AddLocalParticipant(string playerId, string displayName)
        {
            if (CurrentRoom == null)
            {
                throw new InvalidOperationException("A room must exist before participants can be added.");
            }

            if (FindParticipant(playerId) != null)
            {
                return CurrentRoom;
            }

            CurrentRoom.Participants.Add(new MultiplayerParticipantSnapshot
            {
                PlayerId = playerId ?? "",
                DisplayName = string.IsNullOrEmpty(displayName) ? "Guest" : displayName,
                IsHost = false,
                IsReady = false,
                IsConnected = true
            });
            return CurrentRoom;
        }

        public MultiplayerRoomSnapshot SetParticipantReady(string playerId, bool isReady)
        {
            MultiplayerParticipantSnapshot participant = FindParticipant(playerId);
            if (participant == null)
            {
                throw new InvalidOperationException($"Unknown participant '{playerId}'.");
            }

            participant.IsReady = isReady;
            return CurrentRoom;
        }

        public MatchAuthorityState StartAuthoritativeMatch(AuthoritativeMatchStartRequest request)
        {
            if (CurrentRoom == null)
            {
                throw new InvalidOperationException("A room must exist before a match can start.");
            }

            caller.Reset();
            callLog.Clear();
            claimLog.Clear();
            matchEndLog.Clear();
            resolvedClaimKeys.Clear();
            authoritativeCalledNumbers.Clear();

            CurrentMatch = new MatchAuthorityState
            {
                MatchId = $"local_match_{++matchSequence}",
                RoomId = CurrentRoom.RoomId,
                HostPlayerId = CurrentRoom.HostPlayerId,
                State = MatchAuthorityLifecycleState.InRound,
                RealmIndex = request?.RealmIndex ?? 0,
                RoomIndex = request?.RoomIndex ?? 0,
                SelectedCardCount = request?.SelectedCardCount ?? CurrentRoom.SelectedCardCount,
                ManaBetPerCard = request?.ManaBetPerCard ?? CurrentRoom.ManaBetPerCard,
                RoundSeed = request?.RoundSeed ?? 0,
                MaxCallCount = Mathf.Max(1, request?.MaxCallCount ?? 1),
                AutoCallIntervalSeconds = Mathf.Max(0.1f, request?.AutoCallIntervalSeconds ?? 1f),
                CurrentCallIndex = -1,
                PendingEndReason = "",
                EndedReason = ""
            };

            CurrentRoom.State = MultiplayerRoomLifecycleState.MatchInProgress;
            return CurrentMatch;
        }

        public MatchCallEvent EmitNextCall()
        {
            if (CurrentMatch == null || CurrentMatch.State != MatchAuthorityLifecycleState.InRound)
            {
                return null;
            }

            if (!caller.TryCallNext(out int calledNumber))
            {
                return null;
            }

            MatchCallEvent next = new MatchCallEvent
            {
                MatchId = CurrentMatch.MatchId,
                CallIndex = callLog.Count,
                CalledNumber = calledNumber,
                EmittedUtcTicks = DateTime.UtcNow.Ticks
            };
            authoritativeCalledNumbers.Add(calledNumber);
            callLog.Add(next);
            CurrentMatch.CurrentCallIndex = next.CallIndex;
            return next;
        }

        public MatchCallEvent RegisterObservedCall(int calledNumber, long emittedUtcTicks = 0)
        {
            if (CurrentMatch == null || CurrentMatch.State != MatchAuthorityLifecycleState.InRound)
            {
                return null;
            }

            if (calledNumber < 1 || calledNumber > 75 || authoritativeCalledNumbers.Contains(calledNumber))
            {
                return null;
            }

            MatchCallEvent next = new MatchCallEvent
            {
                MatchId = CurrentMatch.MatchId,
                CallIndex = callLog.Count,
                CalledNumber = calledNumber,
                EmittedUtcTicks = emittedUtcTicks > 0 ? emittedUtcTicks : DateTime.UtcNow.Ticks
            };
            authoritativeCalledNumbers.Add(calledNumber);
            callLog.Add(next);
            CurrentMatch.CurrentCallIndex = next.CallIndex;
            return next;
        }

        public MatchClaimResolution ResolveClaim(MatchClaimAttempt attempt)
        {
            MatchClaimResolution resolution = new MatchClaimResolution
            {
                MatchId = attempt?.MatchId ?? CurrentMatch?.MatchId ?? "",
                PlayerId = attempt?.PlayerId ?? "",
                ClaimType = attempt?.ClaimType ?? MatchClaimType.Bingo,
                Result = MatchClaimResolutionKind.Rejected,
                AcceptedCallIndex = -1,
                ValidatedNumberCount = 0,
                Reason = "Rejected"
            };

            if (CurrentMatch == null || CurrentMatch.State != MatchAuthorityLifecycleState.InRound)
            {
                resolution.Result = MatchClaimResolutionKind.RoundClosed;
                resolution.Reason = "Round is not active.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (attempt == null)
            {
                resolution.Reason = "Missing claim attempt.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (!string.Equals(attempt.MatchId, CurrentMatch.MatchId, StringComparison.Ordinal))
            {
                resolution.Reason = "Match id does not match active room state.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (FindParticipant(attempt.PlayerId) == null)
            {
                resolution.Reason = "Claiming player is not present in the active room.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (!string.IsNullOrEmpty(attempt.IdempotencyKey) && !resolvedClaimKeys.Add(attempt.IdempotencyKey))
            {
                resolution.Result = MatchClaimResolutionKind.Duplicate;
                resolution.Reason = "Claim idempotency key has already been resolved.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (attempt.ClaimCallIndex < 0)
            {
                resolution.Reason = "Claim must reference a valid authoritative call index.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (attempt.ClaimCallIndex > CurrentMatch.CurrentCallIndex)
            {
                resolution.Reason = "Claim references a call index that has not happened yet.";
                claimLog.Add(resolution);
                return resolution;
            }

            if (attempt.ClaimedNumbers.Count <= 0)
            {
                resolution.Reason = "Claim did not include any numbers for authority validation.";
                claimLog.Add(resolution);
                return resolution;
            }

            HashSet<int> uniqueClaimedNumbers = new HashSet<int>();
            for (int index = 0; index < attempt.ClaimedNumbers.Count; index++)
            {
                int claimedNumber = attempt.ClaimedNumbers[index];
                if (claimedNumber < 1 || claimedNumber > 75)
                {
                    resolution.Reason = "Claim included an out-of-range bingo number.";
                    claimLog.Add(resolution);
                    return resolution;
                }

                if (!uniqueClaimedNumbers.Add(claimedNumber))
                {
                    continue;
                }

                if (!authoritativeCalledNumbers.Contains(claimedNumber))
                {
                    resolution.Reason = $"Claim included uncalled number {claimedNumber}.";
                    claimLog.Add(resolution);
                    return resolution;
                }
            }

            resolution.Result = MatchClaimResolutionKind.Accepted;
            resolution.AcceptedCallIndex = attempt.ClaimCallIndex;
            resolution.ValidatedNumberCount = uniqueClaimedNumbers.Count;
            resolution.Reason = "Accepted by local authority.";
            claimLog.Add(resolution);
            return resolution;
        }

        public MatchEndEvent PublishMatchEnd(BingoMagicMayhem.Rounds.BingoRoundEndDecision decision, IEnumerable<string> wheelspinEntitledPlayerIds)
        {
            if (CurrentMatch == null)
            {
                throw new InvalidOperationException("A match must exist before it can end.");
            }

            MatchEndEvent endEvent = new MatchEndEvent
            {
                MatchId = CurrentMatch.MatchId,
                EndReasonKind = decision?.Kind ?? BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None,
                EndReason = decision?.Reason ?? "",
                FinalCallIndex = CurrentMatch.CurrentCallIndex
            };

            if (wheelspinEntitledPlayerIds != null)
            {
                foreach (string playerId in wheelspinEntitledPlayerIds)
                {
                    if (!string.IsNullOrEmpty(playerId))
                    {
                        endEvent.WheelspinEntitledPlayerIds.Add(playerId);
                    }
                }
            }

            CurrentMatch.State = MatchAuthorityLifecycleState.Ended;
            CurrentMatch.EndedReason = endEvent.EndReason;
            CurrentRoom.State = MultiplayerRoomLifecycleState.Closed;
            matchEndLog.Add(endEvent);
            return endEvent;
        }

        private MultiplayerParticipantSnapshot FindParticipant(string playerId)
        {
            if (CurrentRoom == null)
            {
                return null;
            }

            for (int index = 0; index < CurrentRoom.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = CurrentRoom.Participants[index];
                if (string.Equals(participant.PlayerId, playerId, StringComparison.Ordinal))
                {
                    return participant;
                }
            }

            return null;
        }

        private static string BuildRoomCode(int sequence)
        {
            return $"L{sequence:0000}";
        }
    }
}
