using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Leaderboard
{
    [DisallowMultipleComponent]
    public sealed class LeaderboardBoardView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text rankHeaderText;
        [SerializeField] private Text playerHeaderText;
        [SerializeField] private Text covenHeaderText;
        [SerializeField] private Text weeklyScoreHeaderText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            rankHeaderText = null;
            playerHeaderText = null;
            covenHeaderText = null;
            weeklyScoreHeaderText = null;
        }

        public void Initialize(
            Text title,
            Text rankHeader,
            Text playerHeader,
            Text covenHeader,
            Text weeklyScoreHeader)
        {
            titleText = title;
            rankHeaderText = rankHeader;
            playerHeaderText = playerHeader;
            covenHeaderText = covenHeader;
            weeklyScoreHeaderText = weeklyScoreHeader;
        }

        public void Apply(LeaderboardBoardDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }

            if (rankHeaderText != null)
            {
                rankHeaderText.text = displayModel.RankHeaderText;
            }

            if (playerHeaderText != null)
            {
                playerHeaderText.text = displayModel.PlayerHeaderText;
            }

            if (covenHeaderText != null)
            {
                covenHeaderText.text = displayModel.CovenHeaderText;
            }

            if (weeklyScoreHeaderText != null)
            {
                weeklyScoreHeaderText.text = displayModel.WeeklyScoreHeaderText;
            }
        }
    }
}
