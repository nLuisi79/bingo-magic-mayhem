using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class JackpotResultPanelView : MonoBehaviour
    {
        [SerializeField] private Text eyebrowText;
        [SerializeField] private Text labelText;
        [SerializeField] private Text amountText;
        [SerializeField] private Text accentText;
        [SerializeField] private Text descriptionText;

        public void ResetRuntimeBindings()
        {
            eyebrowText = null;
            labelText = null;
            amountText = null;
            accentText = null;
            descriptionText = null;
        }

        public void Initialize(Text eyebrow, Text label, Text amount, Text accent, Text description)
        {
            eyebrowText = eyebrow;
            labelText = label;
            amountText = amount;
            accentText = accent;
            descriptionText = description;
        }

        public void Apply(JackpotResultPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (eyebrowText != null) eyebrowText.text = displayModel.EyebrowText;
            if (labelText != null) labelText.text = displayModel.LabelText;
            if (amountText != null) amountText.text = displayModel.AmountText;
            if (accentText != null) accentText.text = displayModel.AccentText;
            if (descriptionText != null) descriptionText.text = displayModel.DescriptionText;
        }
    }
}
