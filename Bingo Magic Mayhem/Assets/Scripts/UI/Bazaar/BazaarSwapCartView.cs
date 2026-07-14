using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Bazaar
{
    [DisallowMultipleComponent]
    public sealed class BazaarSwapCartView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button actionButton;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            subtitleText = null;
            actionButton = null;
        }

        public void Initialize(Text title, Text subtitle, Button button)
        {
            titleText = title;
            subtitleText = subtitle;
            actionButton = button;
        }

        public void Apply(BazaarSwapCartDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }

            if (subtitleText != null)
            {
                subtitleText.text = displayModel.SubtitleText;
            }

            if (actionButton != null)
            {
                Text buttonText = actionButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.ActionButtonText;
                }
            }
        }
    }
}
