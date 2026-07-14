using System;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Bundles the local controller-side multiplayer dependencies so the prototype can
    /// swap backend construction behind one composition seam later.
    /// </summary>
    public sealed class PrototypeMultiplayerControllerDependencies
    {
        public PrototypeMultiplayerControllerDependencies(
            LocalMultiplayerSessionFacade sessionFacade,
            LocalAuthoritativeMatchSimulator matchSimulator,
            PrototypeMultiplayerAuthorityBridge authorityBridge,
            IMultiplayerRoomSessionSyncAdapter syncAdapter)
        {
            SessionFacade = sessionFacade ?? throw new ArgumentNullException(nameof(sessionFacade));
            MatchSimulator = matchSimulator ?? throw new ArgumentNullException(nameof(matchSimulator));
            AuthorityBridge = authorityBridge ?? throw new ArgumentNullException(nameof(authorityBridge));
            SyncAdapter = syncAdapter ?? throw new ArgumentNullException(nameof(syncAdapter));
        }

        public LocalMultiplayerSessionFacade SessionFacade { get; }
        public LocalAuthoritativeMatchSimulator MatchSimulator { get; }
        public PrototypeMultiplayerAuthorityBridge AuthorityBridge { get; }
        public IMultiplayerRoomSessionSyncAdapter SyncAdapter { get; }
    }

    /// <summary>
    /// Holds the gameplay-facing multiplayer seams that the prototype can use without
    /// knowing how controller/session services were constructed.
    /// </summary>
    public sealed class PrototypeMultiplayerRuntime
    {
        public PrototypeMultiplayerRuntime(
            PrototypeMultiplayerBackendMode backendMode,
            IMultiplayerRoomSessionService roomSessionService,
            IMultiplayerMatchAuthorityService matchAuthorityService,
            PrototypeMultiplayerRoomSessionController controller,
            PrototypeMultiplayerGameplayBridge gameplayBridge)
        {
            BackendMode = backendMode;
            RoomSessionService = roomSessionService ?? throw new ArgumentNullException(nameof(roomSessionService));
            MatchAuthorityService = matchAuthorityService ?? throw new ArgumentNullException(nameof(matchAuthorityService));
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            GameplayBridge = gameplayBridge ?? throw new ArgumentNullException(nameof(gameplayBridge));
        }

        public PrototypeMultiplayerBackendMode BackendMode { get; }
        public IMultiplayerRoomSessionService RoomSessionService { get; }
        public IMultiplayerMatchAuthorityService MatchAuthorityService { get; }
        public PrototypeMultiplayerRoomSessionController Controller { get; }
        public PrototypeMultiplayerGameplayBridge GameplayBridge { get; }
    }

    /// <summary>
    /// Local-only composition root for prototype multiplayer. Future UGS-backed wiring
    /// can replace this seam without changing gameplay or UI roots.
    /// </summary>
    public static class PrototypeMultiplayerComposition
    {
        public static PrototypeMultiplayerControllerDependencies CreateControllerDependencies(
            PrototypeMultiplayerBackendMode backendMode,
            LocalMultiplayerSessionFacade sessionFacade = null,
            IMultiplayerRoomSessionSyncAdapter syncAdapter = null,
            IPrototypeMultiplayerRoomSessionSyncAdapterFactory syncAdapterFactory = null)
        {
            LocalMultiplayerSessionFacade resolvedSessionFacade = sessionFacade ?? new LocalMultiplayerSessionFacade();
            IPrototypeMultiplayerRoomSessionSyncAdapterFactory resolvedSyncAdapterFactory =
                syncAdapterFactory ?? new PrototypeMultiplayerRoomSessionSyncAdapterFactory();
            IMultiplayerRoomSessionSyncAdapter resolvedSyncAdapter =
                syncAdapter ?? resolvedSyncAdapterFactory.CreateAdapter(backendMode);
            LocalAuthoritativeMatchSimulator matchSimulator = new LocalAuthoritativeMatchSimulator(resolvedSessionFacade);
            PrototypeMultiplayerAuthorityBridge authorityBridge = new PrototypeMultiplayerAuthorityBridge(matchSimulator);
            return new PrototypeMultiplayerControllerDependencies(
                resolvedSessionFacade,
                matchSimulator,
                authorityBridge,
                resolvedSyncAdapter);
        }

        public static PrototypeMultiplayerControllerDependencies CreateLocalControllerDependencies(
            LocalMultiplayerSessionFacade sessionFacade = null,
            IMultiplayerRoomSessionSyncAdapter syncAdapter = null)
        {
            return CreateControllerDependencies(
                PrototypeMultiplayerBackendMode.Local,
                sessionFacade,
                syncAdapter);
        }

        private static PrototypeMultiplayerRuntime CreateControllerBackedRuntime(
            PrototypeMultiplayerBackendMode backendMode,
            string hostPlayerId,
            PrototypeMultiplayerControllerDependencies dependencies)
        {
            PrototypeMultiplayerRoomSessionController controller = new PrototypeMultiplayerRoomSessionController(dependencies);
            IMultiplayerRoomSessionService roomSessionService = controller;
            IMultiplayerMatchAuthorityService matchAuthorityService = controller;
            PrototypeMultiplayerGameplayBridge gameplayBridge = new PrototypeMultiplayerGameplayBridge(roomSessionService, matchAuthorityService, hostPlayerId);
            return new PrototypeMultiplayerRuntime(
                backendMode,
                roomSessionService,
                matchAuthorityService,
                controller,
                gameplayBridge);
        }

        public static PrototypeMultiplayerRuntime CreateLocalRuntime(string hostPlayerId)
        {
            PrototypeMultiplayerControllerDependencies dependencies = CreateLocalControllerDependencies();
            return CreateControllerBackedRuntime(
                PrototypeMultiplayerBackendMode.Local,
                hostPlayerId,
                dependencies);
        }

        public static PrototypeMultiplayerRuntime CreateUgsStubRuntime(string hostPlayerId)
        {
            PrototypeMultiplayerControllerDependencies dependencies = CreateControllerDependencies(PrototypeMultiplayerBackendMode.Ugs);
            PrototypeMultiplayerRuntime localFallbackRuntime = CreateControllerBackedRuntime(
                PrototypeMultiplayerBackendMode.Local,
                hostPlayerId,
                dependencies);
            PrototypeMultiplayerUgsRuntimeAdapter adapter = new PrototypeMultiplayerUgsRuntimeAdapter(localFallbackRuntime);
            PrototypeMultiplayerGameplayBridge gameplayBridge = new PrototypeMultiplayerGameplayBridge(adapter, adapter, hostPlayerId);
            return new PrototypeMultiplayerRuntime(
                PrototypeMultiplayerBackendMode.Ugs,
                adapter,
                adapter,
                localFallbackRuntime.Controller,
                gameplayBridge);
        }
    }
}
