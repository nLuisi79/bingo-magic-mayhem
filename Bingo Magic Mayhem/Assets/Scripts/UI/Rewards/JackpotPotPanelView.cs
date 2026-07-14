using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class JackpotPotPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text potValueText;
        [SerializeField] private Text accentText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            potValueText = null;
            accentText = null;
        }

        public void Initialize(Text title, Text potValue, Text accent)
        {
            titleText = title;
            potValueText = potValue;
            accentText = accent;
        }

        public void Apply(JackpotPotPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (potValueText != null) potValueText.text = displayModel.PotValueText;
            if (accentText != null) accentText.text = displayModel.AccentText;
        }
    }
}
