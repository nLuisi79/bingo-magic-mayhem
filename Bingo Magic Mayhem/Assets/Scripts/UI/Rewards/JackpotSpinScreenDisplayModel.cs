using UnityEngine;

namespace BingoMagicMayhem.UI.Rewards
{
    public sealed class PrototypeJackpotScreenState
    {
        public string CenterButtonLabel { get; set; }
        public string CollectButtonLabel { get; set; }
        public string SpinStatusText { get; set; }
        public string ResultText { get; set; }
        public Color CenterButtonColor { get; set; }
        public Color SpinStatusColor { get; set; }
        public bool CanUseCenterButton { get; set; }
        public bool CanUseCollectButton { get; set; }
        public bool ShouldBigCollectFromCenterAction { get; set; }
        public bool ShouldBigCollectFromCollectAction { get; set; }
    }

    public static class PrototypeJackpotFlowCoordinator
    {
        public static PrototypeJackpotScreenState BuildScreenState(
            bool isAnimating,
            bool hasStackToCollect,
            bool canSpinWheel,
            bool canBigCollect,
            int pendingJackpotSpins,
            string resultText)
        {
            bool canUseCenterButton = canSpinWheel || canBigCollect;
            string centerButtonLabel = isAnimating ? "SPINNING" : canSpinWheel ? "SPIN" : "COLLECT";
            string collectLabel = canBigCollect ? "Collect" : canSpinWheel && hasStackToCollect ? "Keep Spinning" : "Return Home";
            string spinStatus = isAnimating ? "Wheel spinning..." : canBigCollect ? "Ready to collect" : $"Spins Left: {pendingJackpotSpins}";
            Color centerButtonColor = canBigCollect
                ? new Color(0.86f, 0.55f, 0.06f)
                : canSpinWheel ? new Color(0.08f, 0.48f, 0.12f) : new Color(0.35f, 0.35f, 0.35f);
            Color spinStatusColor = canBigCollect ? new Color(1f, 0.9f, 0.32f) : new Color(0.8f, 1f, 0.55f);

            return new PrototypeJackpotScreenState
            {
                CenterButtonLabel = centerButtonLabel,
                CollectButtonLabel = collectLabel,
                SpinStatusText = spinStatus,
                ResultText = string.IsNullOrEmpty(resultText)
                    ? "Spin each wheelspin, stack the rewards, then collect once."
                    : resultText,
                CenterButtonColor = centerButtonColor,
                SpinStatusColor = spinStatusColor,
                CanUseCenterButton = canUseCenterButton,
                CanUseCollectButton = !isAnimating && (!hasStackToCollect || canBigCollect),
                ShouldBigCollectFromCenterAction = pendingJackpotSpins <= 0 && hasStackToCollect,
                ShouldBigCollectFromCollectAction = pendingJackpotSpins <= 0 && hasStackToCollect
            };
        }
    }

    public sealed class JackpotSpinScreenDisplayModel
    {
        public string TitleText { get; set; } = string.Empty;
        public string SubtitleText { get; set; } = string.Empty;
        public string RoomNameText { get; set; } = string.Empty;
        public string SpinStatusText { get; set; } = string.Empty;
        public string ResultText { get; set; } = string.Empty;
        public string CenterButtonText { get; set; } = string.Empty;
        public string CollectButtonText { get; set; } = string.Empty;
        public bool CanUseCenterButton { get; set; }
        public bool CanUseCollectButton { get; set; }
    }
}
