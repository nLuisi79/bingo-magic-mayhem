using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailySpinActionAreaView : MonoBehaviour
    {
        [SerializeField] private Button spinButton;
        [SerializeField] private Button closeButton;

        public void ResetRuntimeBindings()
        {
            spinButton = null;
            closeButton = null;
        }

        public void Initialize(Button spin, Button close)
        {
            spinButton = spin;
            closeButton = close;
        }

        public void Apply(DailySpinActionAreaDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            ApplyButtonText(spinButton, displayModel.SpinButtonText);
            ApplyButtonText(closeButton, displayModel.CloseButtonText);
        }

        private static void ApplyButtonText(Button button, string text)
        {
            if (button == null)
            {
                return;
            }

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
    }
}
