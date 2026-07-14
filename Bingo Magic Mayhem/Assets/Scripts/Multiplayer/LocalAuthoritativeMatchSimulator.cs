using System;
using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    public sealed class LocalAuthoritativeMatchParticipant
    {
        public string PlayerId = "";
        public string DisplayName = "";
        public bool Ready = true;
    }

    public sealed class LocalAuthoritativeMatchSummary
    {
        public string RoomId = "";
        public string MatchId = "";
        public MatchAuthorityLifecycleState MatchState = MatchAuthorityLifecycleState.None;
        public int ParticipantCount;
        public int ReadyParticipantCount;
        public int CallCount;
        public int ClaimCount;
        public int EndEventCount;
        public string LastEndedReason = "";
        public MatchCallEvent LastCall;
        public MatchClaimResolution LastClaimResolution;
        public MatchEndEvent LastMatchEnd;
    }

    /// <summary>
    /// Gameplay-facing local simulation layer for multiplayer authority flow.
    /// This sits above the room/session facade so prototype gameplay can evolve
    /// toward multiplayer without binding directly to transport or service choices.
    /// </summary>
    public sealed class LocalAuthoritativeMatchSimulator
    {
        private readonly IMultiplayerSessionFacade sessionFacade;

        public LocalAuthoritativeMatchSimulator(IMultiplayerSessionFacade sessionFacade)
        {
            this.sessionFacade = sessionFacade ?? throw new ArgumentNullException(nameof(sessionFacade));
        }

        public MultiplayerRoomSnapshot CreateRoomAndStartMatch(
            string hostPlayerId,
            string hostDisplayName,
            IReadOnlyList<LocalAuthoritativeMatchParticipant> otherParticipants,
            AuthoritativeMatchStartRequest request)
        {
            MultiplayerRoomSnapshot room = sessionFacade.CreateRoom(
                hostPlayerId,
                hostDisplayName,
                request?.RealmIndex ?? 0,
                request?.RoomIndex ?? 0,
                request?.SelectedCardCount ?? 0,
                request?.ManaBetPerCard ?? 0);

            sessionFacade.SetParticipantReady(hostPlayerId, true);

            if (otherParticipants != null)
            {
                for (int index = 0; index < otherParticipants.Count; index++)
                {
                    LocalAuthoritativeMatchParticipant participant = otherParticipants[index];
                    if (participant == null || string.IsNullOrEmpty(participant.PlayerId))
                    {
                        continue;
                    }

                    sessionFacade.AddLocalParticipant(participant.PlayerId, participant.DisplayName);
                    sessionFacade.SetParticipantReady(participant.PlayerId, participant.Ready);
                }
            }

            sessionFacade.StartAuthoritativeMatch(request);
            return room;
        }

        public MatchCallEvent EmitNextHostCall()
        {
            return sessionFacade.EmitNextCall();
        }

        public MatchCallEvent RecordObservedHostCall(int calledNumber, long emittedUtcTicks = 0)
        {
            return sessionFacade.RegisterObservedCall(calledNumber, emittedUtcTicks);
        }

        public MatchClaimResolution SubmitClaim(
            string playerId,
            MatchClaimType claimType,
            int cardIndex,
            int claimCallIndex,
            string idempotencyKey,
            IReadOnlyList<string> markedCellKeys = null,
            IReadOnlyList<int> claimedNumbers = null)
        {
            MatchClaimAttempt attempt = new MatchClaimAttempt
            {
                MatchId = sessionFacade.CurrentMatch?.MatchId ?? "",
                PlayerId = playerId ?? "",
                ClaimType = claimType,
                CardIndex = cardIndex,
                ClaimCallIndex = claimCallIndex,
                IdempotencyKey = idempotencyKey ?? ""
            };

            if (markedCellKeys != null)
            {
                for (int index = 0; index < markedCellKeys.Count; index++)
                {
                    string key = markedCellKeys[index];
                    if (!string.IsNullOrEmpty(key))
                    {
                        attempt.MarkedCellKeys.Add(key);
                    }
                }
            }

            if (claimedNumbers != null)
            {
                for (int index = 0; index < claimedNumbers.Count; index++)
                {
                    int claimedNumber = claimedNumbers[index];
                    if (claimedNumber > 0)
                    {
                        attempt.ClaimedNumbers.Add(claimedNumber);
                    }
                }
            }

            return sessionFacade.ResolveClaim(attempt);
        }

        public MatchEndEvent EndMatch(BingoMagicMayhem.Rounds.BingoRoundEndDecision decision, IReadOnlyList<string> wheelspinEntitledPlayerIds)
        {
            return sessionFacade.PublishMatchEnd(decision, wheelspinEntitledPlayerIds);
        }

        public LocalAuthoritativeMatchSummary BuildSummary()
        {
            IReadOnlyList<MatchCallEvent> calls = sessionFacade.CallLog;
            IReadOnlyList<MatchClaimResolution> claims = sessionFacade.ClaimLog;
            IReadOnlyList<MatchEndEvent> ends = sessionFacade.MatchEndLog;
            MultiplayerRoomSnapshot room = sessionFacade.CurrentRoom;
            int readyParticipantCount = 0;
            if (room != null)
            {
                for (int index = 0; index < room.Participants.Count; index++)
                {
                    if (room.Participants[index].IsReady)
                    {
                        readyParticipantCount++;
                    }
                }
            }

            return new LocalAuthoritativeMatchSummary
            {
                RoomId = room?.RoomId ?? "",
                MatchId = sessionFacade.CurrentMatch?.MatchId ?? "",
                MatchState = sessionFacade.CurrentMatch?.State ?? MatchAuthorityLifecycleState.None,
                ParticipantCount = room?.Participants.Count ?? 0,
                ReadyParticipantCount = readyParticipantCount,
                CallCount = calls.Count,
                ClaimCount = claims.Count,
                EndEventCount = ends.Count,
                LastEndedReason = sessionFacade.CurrentMatch?.EndedReason ?? "",
                LastCall = calls.Count > 0 ? calls[calls.Count - 1] : null,
                LastClaimResolution = claims.Count > 0 ? claims[claims.Count - 1] : null,
                LastMatchEnd = ends.Count > 0 ? ends[ends.Count - 1] : null
            };
        }
    }
}
