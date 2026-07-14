using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class JackpotCollectedStackPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text amountText;
        [SerializeField] private Text accentText;
        [SerializeField] private Text stackText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            amountText = null;
            accentText = null;
            stackText = null;
        }

        public void Initialize(Text title, Text amount, Text accent, Text stack)
        {
            titleText = title;
            amountText = amount;
            accentText = accent;
            stackText = stack;
        }

        public void Apply(JackpotCollectedStackPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (amountText != null) amountText.text = displayModel.AmountText;
            if (accentText != null) accentText.text = displayModel.AccentText;
            if (stackText != null) stackText.text = displayModel.StackText;
        }
    }
}
