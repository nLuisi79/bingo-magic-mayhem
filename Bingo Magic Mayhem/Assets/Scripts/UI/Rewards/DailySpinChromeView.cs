using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailySpinChromeView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button footerButton;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            subtitleText = null;
            footerButton = null;
        }

        public void Initialize(Text title, Text subtitle, Button footer)
        {
            titleText = title;
            subtitleText = subtitle;
            footerButton = footer;
        }

        public void Apply(DailySpinChromeDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.Title;
            if (subtitleText != null) subtitleText.text = displayModel.Subtitle;
            if (footerButton != null)
            {
                Text footerText = footerButton.GetComponentInChildren<Text>();
                if (footerText != null)
                {
                    footerText.text = displayModel.FooterButtonText;
                }
            }
        }
    }
}
