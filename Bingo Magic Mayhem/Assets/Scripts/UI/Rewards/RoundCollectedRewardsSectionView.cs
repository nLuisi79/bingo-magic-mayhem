using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RoundCollectedRewardsSectionView : MonoBehaviour
    {
        [SerializeField] private Text titleText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
        }

        public void Initialize(Text title)
        {
            titleText = title;
        }

        public void Apply(RoundCollectedRewardsSectionDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }
        }
    }
}
