using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyRewardTileView : MonoBehaviour
    {
        [SerializeField] private Text dayText;
        [SerializeField] private Text iconText;
        [SerializeField] private Text rewardText;
        [SerializeField] private Text statusText;

        public void ResetRuntimeBindings()
        {
            dayText = null;
            iconText = null;
            rewardText = null;
            statusText = null;
        }

        public void Initialize(Text day, Text icon, Text reward, Text status)
        {
            dayText = day;
            iconText = icon;
            rewardText = reward;
            statusText = status;
        }

        public void Apply(DailyRewardTileDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (dayText != null) dayText.text = displayModel.DayText;
            if (iconText != null) iconText.text = displayModel.IconText;
            if (rewardText != null) rewardText.text = displayModel.RewardText;
            if (statusText != null) statusText.text = displayModel.StatusText;
        }
    }
}
