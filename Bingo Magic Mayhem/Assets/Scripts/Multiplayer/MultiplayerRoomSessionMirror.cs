using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    public enum MultiplayerSessionSyncEventKind
    {
        None = 0,
        RoomSync = 1,
        Readiness = 2,
        MatchStart = 3,
        CallBroadcast = 4,
        ClaimSubmit = 5,
        ClaimResolution = 6,
        MatchEnd = 7
    }

    public sealed class MultiplayerSessionSyncEnvelope
    {
        public long SequenceId;
        public MultiplayerSessionSyncEventKind Kind = MultiplayerSessionSyncEventKind.None;
        public long RecordedUtcTicks;
        public string RoomId = "";
        public string MatchId = "";
    }

    public interface IMultiplayerRoomSessionMirrorView
    {
        MultiplayerRoomSnapshot MirroredRoom { get; }
        MatchAuthorityState MirroredMatch { get; }
        IReadOnlyList<MatchCallEvent> MirroredCalls { get; }
        IReadOnlyList<MultiplayerClaimSubmitPayload> MirroredClaimSubmissions { get; }
        IReadOnlyList<MatchClaimResolution> MirroredClaimResolutions { get; }
        IReadOnlyList<MatchEndEvent> MirroredMatchEnds { get; }
        IReadOnlyList<MultiplayerSessionSyncEnvelope> AppliedEventLog { get; }
        int DuplicateIgnoredCount { get; }
    }

    /// <summary>
    /// Local receive/apply mirror for transport-neutral room/session payloads.
    /// This models what a remote client or future UGS-backed session consumer would
    /// rebuild from synced events, without binding to a real network stack yet.
    /// </summary>
    public sealed class LocalMultiplayerRoomSessionMirror : IMultiplayerRoomSessionMirrorView
    {
        private readonly List<MatchCallEvent> mirroredCalls = new List<MatchCallEvent>();
        private readonly List<MultiplayerClaimSubmitPayload> mirroredClaimSubmissions = new List<MultiplayerClaimSubmitPayload>();
        private readonly List<MatchClaimResolution> mirroredClaimResolutions = new List<MatchClaimResolution>();
        private readonly List<MatchEndEvent> mirroredMatchEnds = new List<MatchEndEvent>();
        private readonly List<MultiplayerSessionSyncEnvelope> appliedEventLog = new List<MultiplayerSessionSyncEnvelope>();
        private readonly HashSet<string> appliedEventKeys = new HashSet<string>();
        private long nextSequenceId;

        public MultiplayerRoomSnapshot MirroredRoom { get; private set; }
        public MatchAuthorityState MirroredMatch { get; private set; }
        public IReadOnlyList<MatchCallEvent> MirroredCalls => mirroredCalls;
        public IReadOnlyList<MultiplayerClaimSubmitPayload> MirroredClaimSubmissions => mirroredClaimSubmissions;
        public IReadOnlyList<MatchClaimResolution> MirroredClaimResolutions => mirroredClaimResolutions;
        public IReadOnlyList<MatchEndEvent> MirroredMatchEnds => mirroredMatchEnds;
        public IReadOnlyList<MultiplayerSessionSyncEnvelope> AppliedEventLog => appliedEventLog;
        public int DuplicateIgnoredCount { get; private set; }

        public void ApplyRoomSync(MultiplayerRoomSyncPayload payload)
        {
            if (!TryBeginApply(BuildRoomSyncKey(payload), MultiplayerSessionSyncEventKind.RoomSync, payload?.RoomId ?? "", MirroredMatch?.MatchId ?? ""))
            {
                return;
            }

            MultiplayerRoomSnapshot room = new MultiplayerRoomSnapshot
            {
                RoomId = payload?.RoomId ?? "",
                RoomCode = payload?.RoomCode ?? "",
                HostPlayerId = payload?.HostPlayerId ?? "",
                State = payload?.RoomState ?? MultiplayerRoomLifecycleState.None,
                SelectedRealmIndex = payload?.RealmIndex ?? 0,
                SelectedRoomIndex = payload?.RoomIndex ?? 0,
                SelectedCardCount = payload?.SelectedCardCount ?? 0,
                ManaBetPerCard = payload?.ManaBetPerCard ?? 0
            };

            if (payload != null)
            {
                for (int index = 0; index < payload.Participants.Count; index++)
                {
                    MultiplayerParticipantSyncPayload participant = payload.Participants[index];
                    if (participant == null)
                    {
                        continue;
                    }

                    room.Participants.Add(new MultiplayerParticipantSnapshot
                    {
                        PlayerId = participant.PlayerId ?? "",
                        DisplayName = participant.DisplayName ?? "",
                        IsHost = participant.IsHost,
                        IsReady = participant.IsReady,
                        IsConnected = participant.IsConnected
                    });
                }
            }

            MirroredRoom = room;
        }

        public void ApplyReadinessUpdate(MultiplayerReadinessUpdatePayload payload)
        {
            if (!TryBeginApply(BuildReadinessKey(payload), MultiplayerSessionSyncEventKind.Readiness, payload?.RoomId ?? "", MirroredMatch?.MatchId ?? ""))
            {
                return;
            }

            if (MirroredRoom == null)
            {
                MirroredRoom = new MultiplayerRoomSnapshot
                {
                    RoomId = payload?.RoomId ?? "",
                    State = MultiplayerRoomLifecycleState.Lobby
                };
            }

            if (payload != null)
            {
                MultiplayerParticipantSnapshot participant = FindParticipant(payload.PlayerId);
                if (participant == null)
                {
                    participant = new MultiplayerParticipantSnapshot
                    {
                        PlayerId = payload.PlayerId ?? "",
                        DisplayName = payload.PlayerId ?? "",
                        IsConnected = true
                    };
                    MirroredRoom.Participants.Add(participant);
                }

                participant.IsReady = payload.IsReady;
            }
        }

        public void ApplyMatchStart(MultiplayerMatchStartPayload payload)
        {
            if (!TryBeginApply(BuildMatchStartKey(payload), MultiplayerSessionSyncEventKind.MatchStart, payload?.RoomId ?? "", payload?.MatchId ?? ""))
            {
                return;
            }

            MirroredMatch = new MatchAuthorityState
            {
                RoomId = payload?.RoomId ?? "",
                MatchId = payload?.MatchId ?? "",
                HostPlayerId = payload?.HostPlayerId ?? "",
                State = MatchAuthorityLifecycleState.InRound,
                RealmIndex = payload?.RealmIndex ?? 0,
                RoomIndex = payload?.RoomIndex ?? 0,
                SelectedCardCount = payload?.SelectedCardCount ?? 0,
                ManaBetPerCard = payload?.ManaBetPerCard ?? 0,
                RoundSeed = payload?.RoundSeed ?? 0,
                MaxCallCount = payload?.MaxCallCount ?? 0,
                AutoCallIntervalSeconds = payload?.AutoCallIntervalSeconds ?? 0f,
                CurrentCallIndex = -1
            };

            if (MirroredRoom == null)
            {
                MirroredRoom = new MultiplayerRoomSnapshot
                {
                    RoomId = payload?.RoomId ?? "",
                    HostPlayerId = payload?.HostPlayerId ?? "",
                    State = MultiplayerRoomLifecycleState.MatchInProgress
                };
            }
            else
            {
                MirroredRoom.State = MultiplayerRoomLifecycleState.MatchInProgress;
            }

            mirroredCalls.Clear();
            mirroredClaimSubmissions.Clear();
            mirroredClaimResolutions.Clear();
            mirroredMatchEnds.Clear();
        }

        public void ApplyCallBroadcast(MultiplayerCallBroadcastPayload payload)
        {
            if (!TryBeginApply(BuildCallBroadcastKey(payload), MultiplayerSessionSyncEventKind.CallBroadcast, MirroredRoom?.RoomId ?? "", payload?.MatchId ?? ""))
            {
                return;
            }

            MatchCallEvent callEvent = new MatchCallEvent
            {
                MatchId = payload?.MatchId ?? "",
                CallIndex = payload?.CallIndex ?? 0,
                CalledNumber = payload?.CalledNumber ?? 0,
                EmittedUtcTicks = payload?.EmittedUtcTicks ?? 0
            };

            mirroredCalls.Add(callEvent);
            if (MirroredMatch != null)
            {
                MirroredMatch.CurrentCallIndex = callEvent.CallIndex;
            }
        }

        public void ApplyClaimSubmit(MultiplayerClaimSubmitPayload payload)
        {
            if (!TryBeginApply(BuildClaimSubmitKey(payload), MultiplayerSessionSyncEventKind.ClaimSubmit, MirroredRoom?.RoomId ?? "", payload?.MatchId ?? ""))
            {
                return;
            }

            mirroredClaimSubmissions.Add(payload ?? new MultiplayerClaimSubmitPayload());
        }

        public void ApplyClaimResolution(MultiplayerClaimResolutionPayload payload)
        {
            if (!TryBeginApply(BuildClaimResolutionKey(payload), MultiplayerSessionSyncEventKind.ClaimResolution, MirroredRoom?.RoomId ?? "", payload?.MatchId ?? ""))
            {
                return;
            }

            MatchClaimResolution resolution = new MatchClaimResolution
            {
                MatchId = payload?.MatchId ?? "",
                PlayerId = payload?.PlayerId ?? "",
                ClaimType = payload?.ClaimType ?? MatchClaimType.Bingo,
                Result = payload?.Result ?? MatchClaimResolutionKind.Rejected,
                AcceptedCallIndex = payload?.AcceptedCallIndex ?? -1,
                ValidatedNumberCount = payload?.ValidatedNumberCount ?? 0,
                Reason = payload?.Reason ?? ""
            };

            mirroredClaimResolutions.Add(resolution);
        }

        public void ApplyMatchEnd(MultiplayerMatchEndPayload payload)
        {
            if (!TryBeginApply(BuildMatchEndKey(payload), MultiplayerSessionSyncEventKind.MatchEnd, MirroredRoom?.RoomId ?? "", payload?.MatchId ?? ""))
            {
                return;
            }

            MatchEndEvent endEvent = new MatchEndEvent
            {
                MatchId = payload?.MatchId ?? "",
                EndReasonKind = payload?.EndReasonKind ?? BingoMagicMayhem.Rounds.BingoRoundEndReasonKind.None,
                EndReason = payload?.EndReason ?? "",
                FinalCallIndex = payload?.FinalCallIndex ?? -1
            };

            if (payload != null)
            {
                for (int index = 0; index < payload.WheelspinEntitledPlayerIds.Count; index++)
                {
                    string playerId = payload.WheelspinEntitledPlayerIds[index];
                    if (!string.IsNullOrEmpty(playerId))
                    {
                        endEvent.WheelspinEntitledPlayerIds.Add(playerId);
                    }
                }
            }

            mirroredMatchEnds.Add(endEvent);
            if (MirroredMatch != null)
            {
                MirroredMatch.State = MatchAuthorityLifecycleState.Ended;
                MirroredMatch.EndedReason = endEvent.EndReason;
                MirroredMatch.CurrentCallIndex = endEvent.FinalCallIndex;
            }

            if (MirroredRoom != null)
            {
                MirroredRoom.State = MultiplayerRoomLifecycleState.Closed;
            }
        }

        public void Reset()
        {
            MirroredRoom = null;
            MirroredMatch = null;
            mirroredCalls.Clear();
            mirroredClaimSubmissions.Clear();
            mirroredClaimResolutions.Clear();
            mirroredMatchEnds.Clear();
            appliedEventLog.Clear();
            appliedEventKeys.Clear();
            nextSequenceId = 0;
            DuplicateIgnoredCount = 0;
        }

        private MultiplayerParticipantSnapshot FindParticipant(string playerId)
        {
            if (MirroredRoom == null || string.IsNullOrEmpty(playerId))
            {
                return null;
            }

            for (int index = 0; index < MirroredRoom.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = MirroredRoom.Participants[index];
                if (participant != null && string.Equals(participant.PlayerId, playerId, StringComparison.Ordinal))
                {
                    return participant;
                }
            }

            return null;
        }

        private void RecordEvent(MultiplayerSessionSyncEventKind kind, string roomId, string matchId)
        {
            appliedEventLog.Add(new MultiplayerSessionSyncEnvelope
            {
                SequenceId = ++nextSequenceId,
                Kind = kind,
                RecordedUtcTicks = DateTime.UtcNow.Ticks,
                RoomId = roomId ?? "",
                MatchId = matchId ?? ""
            });
        }

        private bool TryBeginApply(string eventKey, MultiplayerSessionSyncEventKind kind, string roomId, string matchId)
        {
            string safeEventKey = string.IsNullOrEmpty(eventKey) ? $"{kind}:empty" : eventKey;
            if (!appliedEventKeys.Add(safeEventKey))
            {
                DuplicateIgnoredCount++;
                return false;
            }

            RecordEvent(kind, roomId, matchId);
            return true;
        }

        private static string BuildRoomSyncKey(MultiplayerRoomSyncPayload payload)
        {
            if (payload == null)
            {
                return "room_sync:null";
            }

            return $"room_sync:{payload.RoomId}:{payload.RoomState}:{payload.RealmIndex}:{payload.RoomIndex}:{payload.SelectedCardCount}:{payload.ManaBetPerCard}:{payload.Participants.Count}";
        }

        private static string BuildReadinessKey(MultiplayerReadinessUpdatePayload payload)
        {
            if (payload == null)
            {
                return "readiness:null";
            }

            return $"readiness:{payload.RoomId}:{payload.PlayerId}:{payload.IsReady}:{payload.UpdatedUtcTicks}";
        }

        private static string BuildMatchStartKey(MultiplayerMatchStartPayload payload)
        {
            if (payload == null)
            {
                return "match_start:null";
            }

            return $"match_start:{payload.RoomId}:{payload.MatchId}:{payload.RoundSeed}:{payload.MaxCallCount}:{payload.AutoCallIntervalSeconds}";
        }

        private static string BuildCallBroadcastKey(MultiplayerCallBroadcastPayload payload)
        {
            if (payload == null)
            {
                return "call:null";
            }

            return $"call:{payload.MatchId}:{payload.CallIndex}:{payload.CalledNumber}:{payload.EmittedUtcTicks}";
        }

        private static string BuildClaimSubmitKey(MultiplayerClaimSubmitPayload payload)
        {
            if (payload == null)
            {
                return "claim_submit:null";
            }

            return $"claim_submit:{payload.MatchId}:{payload.PlayerId}:{payload.ClaimType}:{payload.CardIndex}:{payload.ClaimCallIndex}:{payload.IdempotencyKey}";
        }

        private static string BuildClaimResolutionKey(MultiplayerClaimResolutionPayload payload)
        {
            if (payload == null)
            {
                return "claim_resolution:null";
            }

            return $"claim_resolution:{payload.MatchId}:{payload.PlayerId}:{payload.ClaimType}:{payload.Result}:{payload.AcceptedCallIndex}:{payload.Reason}";
        }

        private static string BuildMatchEndKey(MultiplayerMatchEndPayload payload)
        {
            if (payload == null)
            {
                return "match_end:null";
            }

            return $"match_end:{payload.MatchId}:{payload.EndReasonKind}:{payload.FinalCallIndex}:{payload.EndReason}";
        }
    }
}
