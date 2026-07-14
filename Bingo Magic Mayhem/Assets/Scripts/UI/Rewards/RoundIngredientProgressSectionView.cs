using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RoundIngredientProgressSectionView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text potionNameText;
        [SerializeField] private Text summaryText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            potionNameText = null;
            summaryText = null;
        }

        public void Initialize(Text title, Text potionName, Text summary)
        {
            titleText = title;
            potionNameText = potionName;
            summaryText = summary;
        }

        public void Apply(RoundIngredientProgressSectionDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (potionNameText != null) potionNameText.text = displayModel.PotionNameText;
            if (summaryText != null) summaryText.text = displayModel.SummaryText;
        }
    }
}
