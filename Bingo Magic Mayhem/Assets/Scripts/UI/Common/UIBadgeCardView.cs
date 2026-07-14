using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UIBadgeCardView : MonoBehaviour
    {
        [SerializeField] private Graphic accentGraphic;
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;

        public void ResetRuntimeBindings()
        {
            accentGraphic = null;
            titleText = null;
            detailText = null;
        }

        public void Initialize(Graphic accent, Text title, Text detail)
        {
            accentGraphic = accent;
            titleText = title;
            detailText = detail;
        }

        public void Apply(string accentText, string title, string detail)
        {
            if (accentGraphic is Text accentTextGraphic)
            {
                accentTextGraphic.text = accentText;
            }

            if (titleText != null)
            {
                titleText.text = title;
            }

            if (detailText != null)
            {
                detailText.text = detail;
            }
        }
    }
}
