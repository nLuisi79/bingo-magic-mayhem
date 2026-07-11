using System.Threading.Tasks;

namespace BingoMagicMayhem.Infrastructure
{
    /// <summary>
    /// Process-wide lifecycle owner. It initializes the composition root once and
    /// gives feature code a single replaceable service entry point.
    /// </summary>
    public static class GameInfrastructureRuntime
    {
        private static readonly object Sync = new object();
        private static Task<GameInfrastructureServices> initializationTask;
        private static InfrastructureCompositionOptions pendingOptions;

        public static GameInfrastructureServices Current { get; private set; }
        public static bool IsInitialized => Current != null;

        public static Task<GameInfrastructureServices> InitializeAsync(InfrastructureCompositionOptions options = null)
        {
            lock (Sync)
            {
                pendingOptions ??= options ?? new InfrastructureCompositionOptions();
                return initializationTask ??= InitializeCoreAsync();
            }
        }

        private static async Task<GameInfrastructureServices> InitializeCoreAsync()
        {
            GameInfrastructureServices services = GameInfrastructureServices.CreateConfigured(
                compositionOptions: pendingOptions ?? new InfrastructureCompositionOptions());
            await services.InitializeAsync();
            Current = services;
            return services;
        }
    }
}
