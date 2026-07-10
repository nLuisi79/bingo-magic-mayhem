using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Stable offline guest identity. A future Unity Authentication adapter can
    /// replace this facade without changing feature code.
    /// </summary>
    public sealed class LocalIdentityFacade : IIdentityFacade
    {
        private const string IdentityStateName = "identity";
        private readonly ILocalDurableStateStore stateStore;

        public IdentitySession Current { get; private set; }

        public LocalIdentityFacade(ILocalDurableStateStore stateStore)
        {
            this.stateStore = stateStore ?? throw new ArgumentNullException(nameof(stateStore));
        }

        public Task<IdentitySession> InitializeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Current != null)
            {
                return Task.FromResult(Current);
            }

            if (!stateStore.TryLoad(IdentityStateName, out LocalIdentityState saved) ||
                string.IsNullOrWhiteSpace(saved.LocalGuestId))
            {
                saved = new LocalIdentityState
                {
                    LocalGuestId = "guest_" + Guid.NewGuid().ToString("N"),
                    CreatedAtUtc = DateTime.UtcNow.ToString("O")
                };
                stateStore.Save(IdentityStateName, saved);
            }

            Current = new IdentitySession
            {
                PlayerId = saved.LocalGuestId,
                Provider = "local_guest",
                IsCloudAuthenticated = false,
                IsAnonymous = true
            };
            return Task.FromResult(Current);
        }

        [Serializable]
        private sealed class LocalIdentityState
        {
            public string LocalGuestId = "";
            public string CreatedAtUtc = "";
        }
    }

    public static class IdentitySafetyDiagnostics
    {
        public const string PolicyVersion = "identity_safety_v0.1";

        public static IdentitySafetySnapshot Capture(IIdentityFacade identity, RemoteConfigSafetySnapshot remoteConfigSafety)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            IdentitySession session = identity.Current ?? new IdentitySession();
            bool remoteFlagsRequestLiveAuth = remoteConfigSafety != null && (
                remoteConfigSafety.UgsAdaptersEnabled ||
                remoteConfigSafety.CloudProfileSyncEnabled ||
                remoteConfigSafety.JournalUploadEnabled);

            IdentitySafetySnapshot snapshot = new IdentitySafetySnapshot
            {
                PolicyVersion = PolicyVersion,
                Provider = string.IsNullOrWhiteSpace(session.Provider) ? "uninitialized" : session.Provider,
                IsCloudAuthenticated = session.IsCloudAuthenticated,
                IsAnonymous = session.IsAnonymous,
                AdapterCompiled = IsAdapterDefineEnabled(),
                ProjectLinked = false,
                EnvironmentApproved = false,
                ConsentApproved = false,
                AllowsCloudSignIn = false,
                AllowsAccountLink = false,
                AllowsRecovery = false,
                RemoteFlagsRequestLiveAuth = remoteFlagsRequestLiveAuth,
                LiveRuntimeChangeAllowed = false,
                Reason = "Identity remains on the local guest path until Unity Authentication project link, consent/privacy, account-linking, and recovery policy are approved."
            };

            AddCheck(
                snapshot,
                "Local guest identity",
                snapshot.Provider == "local_guest" && !snapshot.IsCloudAuthenticated
                    ? BackendPreflightStatus.Pass
                    : BackendPreflightStatus.Warning,
                snapshot.Provider == "local_guest" && !snapshot.IsCloudAuthenticated
                    ? "The current composition uses the durable offline guest identity path."
                    : "Identity provider differs from the expected local guest baseline; verify no live auth path was enabled unintentionally.");

            AddCheck(
                snapshot,
                "Project/environment link",
                BackendPreflightStatus.Blocked,
                "Unity Authentication remains blocked until the intended project and development environment are linked.");

            AddCheck(
                snapshot,
                "Consent/privacy",
                BackendPreflightStatus.Blocked,
                "Live sign-in and account data handling need approved consent, privacy, and deletion expectations.");

            AddCheck(
                snapshot,
                "Account linking",
                BackendPreflightStatus.Blocked,
                "Anonymous upgrade/linking behavior is not approved, so guest accounts remain isolated.");

            AddCheck(
                snapshot,
                "Recovery path",
                BackendPreflightStatus.Blocked,
                "No recovery or transfer flow is active for local guest identity.");

            AddCheck(
                snapshot,
                "Remote Config bypass",
                remoteFlagsRequestLiveAuth ? BackendPreflightStatus.Blocked : BackendPreflightStatus.Pass,
                remoteFlagsRequestLiveAuth
                    ? "Remote Config may request live/cloud infrastructure, but identity remains code-gated off."
                    : "Remote Config is not requesting live/cloud identity behavior.");

            return snapshot;
        }

        private static void AddCheck(
            IdentitySafetySnapshot snapshot,
            string name,
            BackendPreflightStatus status,
            string detail)
        {
            snapshot.Checks.Add(new BackendPreflightCheck
            {
                Name = name ?? "",
                Status = status,
                Detail = detail ?? ""
            });
        }

        private static bool IsAdapterDefineEnabled()
        {
#if BMM_UGS_ADAPTERS
            return true;
#else
            return false;
#endif
        }
    }
}
