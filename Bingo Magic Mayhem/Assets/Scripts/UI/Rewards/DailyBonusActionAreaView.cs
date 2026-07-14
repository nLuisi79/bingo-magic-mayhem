using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyBonusActionAreaView : MonoBehaviour
    {
        [SerializeField] private Button claimButton;
        [SerializeField] private Button closeButton;

        public void ResetRuntimeBindings()
        {
            claimButton = null;
            closeButton = null;
        }

        public void Initialize(Button claim, Button close)
        {
            claimButton = claim;
            closeButton = close;
        }

        public void Apply(DailyBonusActionAreaDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            ApplyButtonText(claimButton, displayModel.ClaimButtonText);
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
