using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Stable, detached snapshot of multiplayer room/session state for presenter and UI
    /// consumption. This avoids binding read models directly to mutable local facade
    /// state and creates a future-safe seam for transport-backed implementations.
    /// </summary>
    public sealed class MultiplayerRoomSessionSnapshot
    {
        public MultiplayerRoomSnapshot Room = new MultiplayerRoomSnapshot();
        public MatchAuthorityState Match;
        public readonly List<MatchCallEvent> CallLog = new List<MatchCallEvent>();
        public readonly List<MatchClaimResolution> ClaimLog = new List<MatchClaimResolution>();
        public readonly List<MatchEndEvent> MatchEndLog = new List<MatchEndEvent>();
        public long SnapshotUtcTicks;
    }

    public static class MultiplayerRoomSessionSnapshotFactory
    {
        public static MultiplayerRoomSessionSnapshot Build(IMultiplayerSessionFacade sessionFacade)
        {
            MultiplayerRoomSessionSnapshot snapshot = new MultiplayerRoomSessionSnapshot
            {
                SnapshotUtcTicks = DateTime.UtcNow.Ticks
            };

            if (sessionFacade == null)
            {
                return snapshot;
            }

            snapshot.Room = CloneRoom(sessionFacade.CurrentRoom) ?? new MultiplayerRoomSnapshot();
            snapshot.Match = CloneMatch(sessionFacade.CurrentMatch);
            CopyCalls(sessionFacade.CallLog, snapshot.CallLog);
            CopyClaims(sessionFacade.ClaimLog, snapshot.ClaimLog);
            CopyEnds(sessionFacade.MatchEndLog, snapshot.MatchEndLog);
            return snapshot;
        }

        private static MultiplayerRoomSnapshot CloneRoom(MultiplayerRoomSnapshot room)
        {
            if (room == null)
            {
                return null;
            }

            MultiplayerRoomSnapshot clone = new MultiplayerRoomSnapshot
            {
                RoomId = room.RoomId ?? "",
                RoomCode = room.RoomCode ?? "",
                State = room.State,
                HostPlayerId = room.HostPlayerId ?? "",
                SelectedRealmIndex = room.SelectedRealmIndex,
                SelectedRoomIndex = room.SelectedRoomIndex,
                SelectedCardCount = room.SelectedCardCount,
                ManaBetPerCard = room.ManaBetPerCard
            };

            for (int index = 0; index < room.Participants.Count; index++)
            {
                MultiplayerParticipantSnapshot participant = room.Participants[index];
                if (participant == null)
                {
                    continue;
                }

                clone.Participants.Add(new MultiplayerParticipantSnapshot
                {
                    PlayerId = participant.PlayerId ?? "",
                    DisplayName = participant.DisplayName ?? "",
                    IsHost = participant.IsHost,
                    IsReady = participant.IsReady,
                    IsConnected = participant.IsConnected
                });
            }

            return clone;
        }

        private static MatchAuthorityState CloneMatch(MatchAuthorityState match)
        {
            if (match == null)
            {
                return null;
            }

            return new MatchAuthorityState
            {
                MatchId = match.MatchId ?? "",
                RoomId = match.RoomId ?? "",
                HostPlayerId = match.HostPlayerId ?? "",
                State = match.State,
                RealmIndex = match.RealmIndex,
                RoomIndex = match.RoomIndex,
                SelectedCardCount = match.SelectedCardCount,
                ManaBetPerCard = match.ManaBetPerCard,
                RoundSeed = match.RoundSeed,
                MaxCallCount = match.MaxCallCount,
                AutoCallIntervalSeconds = match.AutoCallIntervalSeconds,
                CurrentCallIndex = match.CurrentCallIndex,
                PendingEndReason = match.PendingEndReason ?? "",
                EndedReason = match.EndedReason ?? ""
            };
        }

        private static void CopyCalls(IReadOnlyList<MatchCallEvent> source, List<MatchCallEvent> target)
        {
            if (source == null)
            {
                return;
            }

            for (int index = 0; index < source.Count; index++)
            {
                MatchCallEvent call = source[index];
                if (call == null)
                {
                    continue;
                }

                target.Add(new MatchCallEvent
                {
                    MatchId = call.MatchId ?? "",
                    CallIndex = call.CallIndex,
                    CalledNumber = call.CalledNumber,
                    EmittedUtcTicks = call.EmittedUtcTicks
                });
            }
        }

        private static void CopyClaims(IReadOnlyList<MatchClaimResolution> source, List<MatchClaimResolution> target)
        {
            if (source == null)
            {
                return;
            }

            for (int index = 0; index < source.Count; index++)
            {
                MatchClaimResolution claim = source[index];
                if (claim == null)
                {
                    continue;
                }

                target.Add(new MatchClaimResolution
                {
                    MatchId = claim.MatchId ?? "",
                    PlayerId = claim.PlayerId ?? "",
                    ClaimType = claim.ClaimType,
                    Result = claim.Result,
                    AcceptedCallIndex = claim.AcceptedCallIndex,
                    ValidatedNumberCount = claim.ValidatedNumberCount,
                    Reason = claim.Reason ?? ""
                });
            }
        }

        private static void CopyEnds(IReadOnlyList<MatchEndEvent> source, List<MatchEndEvent> target)
        {
            if (source == null)
            {
                return;
            }

            for (int index = 0; index < source.Count; index++)
            {
                MatchEndEvent end = source[index];
                if (end == null)
                {
                    continue;
                }

                MatchEndEvent clone = new MatchEndEvent
                {
                    MatchId = end.MatchId ?? "",
                    EndReasonKind = end.EndReasonKind,
                    EndReason = end.EndReason ?? "",
                    FinalCallIndex = end.FinalCallIndex
                };

                for (int wheelspinIndex = 0; wheelspinIndex < end.WheelspinEntitledPlayerIds.Count; wheelspinIndex++)
                {
                    string playerId = end.WheelspinEntitledPlayerIds[wheelspinIndex];
                    if (!string.IsNullOrEmpty(playerId))
                    {
                        clone.WheelspinEntitledPlayerIds.Add(playerId);
                    }
                }

                target.Add(clone);
            }
        }
    }
}
