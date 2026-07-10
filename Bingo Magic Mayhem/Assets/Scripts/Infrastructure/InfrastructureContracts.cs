using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    public enum ServiceEnvironment
    {
        Local,
        Prototype,
        Beta
    }

    public enum JournalRecordKind
    {
        Action,
        StatusTransition
    }

    public enum JournalActionStatus
    {
        Pending,
        AppliedLocal,
        Synced,
        Rejected,
        Compensated
    }

    [Serializable]
    public sealed class IdentitySession
    {
        public string PlayerId = "";
        public string Provider = "local_guest";
        public bool IsCloudAuthenticated;
        public bool IsAnonymous = true;
    }

    [Serializable]
    public sealed class ActionJournalRecord
    {
        public string ActionId = "";
        public string PlayerId = "";
        public long Sequence;
        public string CreatedAtUtc = "";
        public string Source = "";
        public string Type = "";
        public string PayloadJson = "{}";
        public JournalRecordKind RecordKind;
        public JournalActionStatus Status;
        public string IdempotencyKey = "";
    }

    [Serializable]
    public sealed class RemoteConfigEntry
    {
        public string Key = "";
        public string Value = "";

        public RemoteConfigEntry()
        {
        }

        public RemoteConfigEntry(string key, string value)
        {
            Key = key ?? "";
            Value = value ?? "";
        }
    }

    public interface ILocalDurableStateStore
    {
        bool TryLoad<T>(string stateName, out T value) where T : class;
        void Save<T>(string stateName, T value) where T : class;
    }

    public interface IActionJournal
    {
        ActionJournalRecord RecordAction(
            string playerId,
            string source,
            string type,
            string payloadJson = "{}",
            string idempotencyKey = "",
            JournalActionStatus status = JournalActionStatus.Pending,
            string actionId = "");

        ActionJournalRecord RecordStatus(
            string actionId,
            string playerId,
            string source,
            string type,
            JournalActionStatus status,
            string payloadJson = "{}");

        IReadOnlyList<ActionJournalRecord> ReadAll();
    }

    public interface IIdentityFacade
    {
        IdentitySession Current { get; }
        Task<IdentitySession> InitializeAsync(CancellationToken cancellationToken = default);
    }

    public interface IAnalyticsFacade
    {
        void Track(string eventName, string safePayloadJson = "{}");
        Task FlushAsync(CancellationToken cancellationToken = default);
    }

    public interface IRemoteConfigFacade
    {
        string Source { get; }
        string Revision { get; }
        Task LoadAsync(CancellationToken cancellationToken = default);
        bool HasKey(string key);
        string GetString(string key, string fallback = "");
        int GetInt(string key, int fallback = 0);
        float GetFloat(string key, float fallback = 0f);
        bool GetBool(string key, bool fallback = false);
    }
}
