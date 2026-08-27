using System.Collections.Generic;

namespace BingoMagicMayhem.UI.Rewards
{
    public static class PrototypeRewardFlowCoordinator
    {
        public static bool ShouldShowCardRevealAfterRewardRedemption(IReadOnlyList<AwardedAlbumCardRecord> awardedCards)
        {
            return awardedCards != null && awardedCards.Count > 0;
        }

        public static bool ShouldShowJackpotSpinAfterRewardRedemption(bool jackpotSpinsEarned, int pendingJackpotSpins)
        {
            return jackpotSpinsEarned && pendingJackpotSpins > 0;
        }

        public static bool ShouldShowJackpotSpinAfterCardReveal(bool cardRevealShouldShowJackpotSpins, int pendingJackpotSpins)
        {
            return cardRevealShouldShowJackpotSpins && pendingJackpotSpins > 0;
        }
    }

    public sealed class RewardPreviewPanelDisplayModel
    {
        public string TitleText { get; set; } = string.Empty;
        public string RoundResultText { get; set; } = string.Empty;
        public string CollectedRewardsTitleText { get; set; } = string.Empty;
        public string PotionProgressTitleText { get; set; } = string.Empty;
        public string ContinueButtonText { get; set; } = string.Empty;
    }
}
