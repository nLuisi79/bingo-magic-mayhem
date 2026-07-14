using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyBonusMilestoneMarkerView : MonoBehaviour
    {
        [SerializeField] private Text badgeText;
        [SerializeField] private Text dayText;

        public void ResetRuntimeBindings()
        {
            badgeText = null;
            dayText = null;
        }

        public void Initialize(Text badge, Text day)
        {
            badgeText = badge;
            dayText = day;
        }

        public void Apply(DailyBonusMilestoneMarkerDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (badgeText != null) badgeText.text = displayModel.BadgeText;
            if (dayText != null) dayText.text = displayModel.DayText;
        }
    }
}
