using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyBonusHeaderView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text streakText;
        [SerializeField] private Text nextChestText;
        [SerializeField] private Text streakWarningText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            streakText = null;
            nextChestText = null;
            streakWarningText = null;
        }

        public void Initialize(Text title, Text streak, Text nextChest, Text streakWarning)
        {
            titleText = title;
            streakText = streak;
            nextChestText = nextChest;
            streakWarningText = streakWarning;
        }

        public void Apply(DailyBonusHeaderDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.Title;
            if (streakText != null) streakText.text = displayModel.StreakText;
            if (nextChestText != null) nextChestText.text = displayModel.NextChestText;
            if (streakWarningText != null) streakWarningText.text = displayModel.StreakWarningText;
        }
    }
}
