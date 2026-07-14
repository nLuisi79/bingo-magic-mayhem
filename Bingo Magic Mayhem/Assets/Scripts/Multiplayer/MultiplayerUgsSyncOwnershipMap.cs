using System.Collections.Generic;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Thin code-side ownership seam for future UGS wiring. This mirrors the direction
    /// in Docs/14_UGS_PRODUCTION_OWNERSHIP_MAP.md without binding runtime code to live
    /// SDK packages yet.
    /// </summary>
    public enum MultiplayerUgsServiceOwner
    {
        None = 0,
        Lobby = 1,
        Relay = 2,
        CloudCode = 3,
        CloudSave = 4,
        Analytics = 5
    }

    public sealed class MultiplayerUgsSyncOwnershipEntry
    {
        public MultiplayerSessionSyncEventKind EventKind = MultiplayerSessionSyncEventKind.None;
        public MultiplayerUgsServiceOwner PrimaryOwner = MultiplayerUgsServiceOwner.None;
        public MultiplayerUgsServiceOwner SecondaryOwner = MultiplayerUgsServiceOwner.None;
        public string ResponsibilitySummary = "";
        public bool RequiresAuthoritativeValidation;
    }

    public static class MultiplayerUgsSyncOwnershipMap
    {
        private static readonly IReadOnlyDictionary<MultiplayerSessionSyncEventKind, MultiplayerUgsSyncOwnershipEntry> entries
            = new Dictionary<MultiplayerSessionSyncEventKind, MultiplayerUgsSyncOwnershipEntry>
        {
            {
                MultiplayerSessionSyncEventKind.RoomSync,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.RoomSync,
                    PrimaryOwner = MultiplayerUgsServiceOwner.Lobby,
                    SecondaryOwner = MultiplayerUgsServiceOwner.CloudSave,
                    ResponsibilitySummary = "Lobby/session metadata and player-visible room membership snapshot.",
                    RequiresAuthoritativeValidation = false
                }
            },
            {
                MultiplayerSessionSyncEventKind.Readiness,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.Readiness,
                    PrimaryOwner = MultiplayerUgsServiceOwner.Lobby,
                    SecondaryOwner = MultiplayerUgsServiceOwner.Analytics,
                    ResponsibilitySummary = "Transient lobby readiness state and session UX telemetry.",
                    RequiresAuthoritativeValidation = false
                }
            },
            {
                MultiplayerSessionSyncEventKind.MatchStart,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.MatchStart,
                    PrimaryOwner = MultiplayerUgsServiceOwner.CloudCode,
                    SecondaryOwner = MultiplayerUgsServiceOwner.Lobby,
                    ResponsibilitySummary = "Authoritative round bootstrap and validated match/session parameters.",
                    RequiresAuthoritativeValidation = true
                }
            },
            {
                MultiplayerSessionSyncEventKind.CallBroadcast,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.CallBroadcast,
                    PrimaryOwner = MultiplayerUgsServiceOwner.Relay,
                    SecondaryOwner = MultiplayerUgsServiceOwner.Analytics,
                    ResponsibilitySummary = "Real-time authoritative number-call delivery and timing telemetry.",
                    RequiresAuthoritativeValidation = true
                }
            },
            {
                MultiplayerSessionSyncEventKind.ClaimSubmit,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.ClaimSubmit,
                    PrimaryOwner = MultiplayerUgsServiceOwner.CloudCode,
                    SecondaryOwner = MultiplayerUgsServiceOwner.Relay,
                    ResponsibilitySummary = "Player bingo/jackpot claim submission routed for server validation.",
                    RequiresAuthoritativeValidation = true
                }
            },
            {
                MultiplayerSessionSyncEventKind.ClaimResolution,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.ClaimResolution,
                    PrimaryOwner = MultiplayerUgsServiceOwner.CloudCode,
                    SecondaryOwner = MultiplayerUgsServiceOwner.Analytics,
                    ResponsibilitySummary = "Authoritative claim verdict plus fraud/repeat protection telemetry.",
                    RequiresAuthoritativeValidation = true
                }
            },
            {
                MultiplayerSessionSyncEventKind.MatchEnd,
                new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = MultiplayerSessionSyncEventKind.MatchEnd,
                    PrimaryOwner = MultiplayerUgsServiceOwner.CloudCode,
                    SecondaryOwner = MultiplayerUgsServiceOwner.CloudSave,
                    ResponsibilitySummary = "Authoritative round closure, reward handoff seam, and recoverable end-state snapshot.",
                    RequiresAuthoritativeValidation = true
                }
            }
        };

        public static MultiplayerUgsSyncOwnershipEntry Get(MultiplayerSessionSyncEventKind eventKind)
        {
            return entries.TryGetValue(eventKind, out MultiplayerUgsSyncOwnershipEntry entry)
                ? entry
                : new MultiplayerUgsSyncOwnershipEntry
                {
                    EventKind = eventKind,
                    ResponsibilitySummary = "No mapped owner."
                };
        }
    }
}
