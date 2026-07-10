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
}
