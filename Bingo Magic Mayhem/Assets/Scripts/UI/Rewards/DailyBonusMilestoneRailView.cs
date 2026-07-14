using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyBonusMilestoneRailView : MonoBehaviour
    {
        [SerializeField] private Text progressText;

        public void ResetRuntimeBindings()
        {
            progressText = null;
        }

        public void Initialize(Text progress)
        {
            progressText = progress;
        }

        public void Apply(DailyBonusMilestoneRailDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (progressText != null)
            {
                progressText.text = displayModel.ProgressText;
            }
        }
    }
}
