using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailySpinWheelChromeView : MonoBehaviour
    {
        [SerializeField] private Text pointerText;

        public void ResetRuntimeBindings()
        {
            pointerText = null;
        }

        public void Initialize(Text pointer)
        {
            pointerText = pointer;
        }

        public void Apply(DailySpinWheelChromeDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (pointerText != null)
            {
                pointerText.text = displayModel.PointerText;
            }
        }
    }
}
