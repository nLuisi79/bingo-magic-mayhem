using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Leaderboard
{
    [DisallowMultipleComponent]
    public sealed class LeaderboardRowView : MonoBehaviour
    {
        [SerializeField] private Text rankText;
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text covenNameText;
        [SerializeField] private Text scoreText;

        public void ResetRuntimeBindings()
        {
            rankText = null;
            playerNameText = null;
            covenNameText = null;
            scoreText = null;
        }

        public void Initialize(Text rank, Text playerName, Text covenName, Text score)
        {
            rankText = rank;
            playerNameText = playerName;
            covenNameText = covenName;
            scoreText = score;
        }

        public void Apply(LeaderboardRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (rankText != null)
            {
                rankText.text = displayModel.RankText;
            }

            if (playerNameText != null)
            {
                playerNameText.text = displayModel.PlayerNameText;
            }

            if (covenNameText != null)
            {
                covenNameText.text = displayModel.CovenNameText;
            }

            if (scoreText != null)
            {
                scoreText.text = displayModel.ScoreText;
            }
        }
    }
}
