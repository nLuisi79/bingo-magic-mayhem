using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class SocialFreebiesActionsView : MonoBehaviour
    {
        [SerializeField] private Button instagramButton;
        [SerializeField] private Button facebookButton;
        [SerializeField] private Button xButton;
        [SerializeField] private Button simulateButton;

        public void ResetRuntimeBindings()
        {
            instagramButton = null;
            facebookButton = null;
            xButton = null;
            simulateButton = null;
        }

        public void Initialize(Button instagram, Button facebook, Button x, Button simulate)
        {
            instagramButton = instagram;
            facebookButton = facebook;
            xButton = x;
            simulateButton = simulate;
        }

        public void Apply(SocialFreebiesActionsDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            ApplyButtonText(instagramButton, displayModel.InstagramButtonText);
            ApplyButtonText(facebookButton, displayModel.FacebookButtonText);
            ApplyButtonText(xButton, displayModel.XButtonText);
            ApplyButtonText(simulateButton, displayModel.SimulateButtonText);
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
