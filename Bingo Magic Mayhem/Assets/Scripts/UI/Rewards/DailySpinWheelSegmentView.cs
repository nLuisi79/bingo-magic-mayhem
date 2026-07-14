using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailySpinWheelSegmentView : MonoBehaviour
    {
        [SerializeField] private Text labelText;

        public void ResetRuntimeBindings()
        {
            labelText = null;
        }

        public void Initialize(Text label)
        {
            labelText = label;
        }

        public void Apply(DailySpinWheelSegmentDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (labelText != null)
            {
                labelText.text = displayModel.LabelText;
            }
        }
    }
}
