using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class JackpotCollectionConfirmationPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text amountText;
        [SerializeField] private Text accentText;
        [SerializeField] private Text descriptionText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            amountText = null;
            accentText = null;
            descriptionText = null;
        }

        public void Initialize(Text title, Text amount, Text accent, Text description)
        {
            titleText = title;
            amountText = amount;
            accentText = accent;
            descriptionText = description;
        }

        public void Apply(JackpotCollectionConfirmationPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (amountText != null) amountText.text = displayModel.AmountText;
            if (accentText != null) accentText.text = displayModel.AccentText;
            if (descriptionText != null) descriptionText.text = displayModel.DescriptionText;
        }
    }
}
