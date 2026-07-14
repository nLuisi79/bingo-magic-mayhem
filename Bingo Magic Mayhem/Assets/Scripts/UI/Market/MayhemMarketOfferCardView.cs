using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Market
{
    [DisallowMultipleComponent]
    public sealed class MayhemMarketOfferCardView : MonoBehaviour
    {
        [SerializeField] private Text categoryText;
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Button actionButton;

        public void ResetRuntimeBindings()
        {
            categoryText = null;
            titleText = null;
            detailText = null;
            actionButton = null;
        }

        public void Initialize(Text category, Text title, Text detail, Button button)
        {
            categoryText = category;
            titleText = title;
            detailText = detail;
            actionButton = button;
        }

        public void Apply(MayhemMarketOfferDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (categoryText != null)
            {
                categoryText.text = displayModel.Category;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.Detail;
            }

            if (actionButton != null)
            {
                Text buttonText = actionButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.ButtonText;
                }

                actionButton.interactable = displayModel.IsInteractable;
            }
        }
    }
}
