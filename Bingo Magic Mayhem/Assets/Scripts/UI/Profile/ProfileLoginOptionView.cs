using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class ProfileLoginOptionView : MonoBehaviour
    {
        [SerializeField] private Button providerButton;

        public void ResetRuntimeBindings()
        {
            providerButton = null;
        }

        public void Initialize(Button button)
        {
            providerButton = button;
        }

        public void Apply(ProfileLoginOptionDisplayModel displayModel)
        {
            if (displayModel == null || providerButton == null)
            {
                return;
            }

            providerButton.interactable = displayModel.IsAvailable;
            Text buttonText = providerButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = displayModel.ProviderName;
            }
        }
    }
}
