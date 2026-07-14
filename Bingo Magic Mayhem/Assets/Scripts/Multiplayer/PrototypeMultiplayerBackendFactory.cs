using System;

namespace BingoMagicMayhem.Multiplayer
{
    /// <summary>
    /// Preferred backend targets for prototype multiplayer runtime selection.
    /// Local remains the active implementation until a UGS-backed runtime is approved
    /// and constructed behind the same provider contract.
    /// </summary>
    public enum PrototypeMultiplayerBackendMode
    {
        Local = 0,
        Ugs = 1
    }

    /// <summary>
    /// Stable provider contract for acquiring a multiplayer runtime bundle by backend mode.
    /// </summary>
    public interface IPrototypeMultiplayerRuntimeProvider
    {
        PrototypeMultiplayerRuntime CreateRuntime(PrototypeMultiplayerBackendMode backendMode, string hostPlayerId);
    }

    /// <summary>
    /// Default prototype runtime provider. UGS currently falls back to the local seam
    /// until live package-backed construction is explicitly approved and implemented.
    /// </summary>
    public sealed class PrototypeMultiplayerRuntimeProvider : IPrototypeMultiplayerRuntimeProvider
    {
        public PrototypeMultiplayerRuntime CreateRuntime(PrototypeMultiplayerBackendMode backendMode, string hostPlayerId)
        {
            switch (backendMode)
            {
                case PrototypeMultiplayerBackendMode.Ugs:
                    return PrototypeMultiplayerComposition.CreateUgsStubRuntime(hostPlayerId);
                case PrototypeMultiplayerBackendMode.Local:
                default:
                    return PrototypeMultiplayerComposition.CreateLocalRuntime(hostPlayerId);
            }
        }
    }
}
