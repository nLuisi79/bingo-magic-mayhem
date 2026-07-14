using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RoundCollectedRewardTileView : MonoBehaviour
    {
        [SerializeField] private Text iconText;
        [SerializeField] private Text bodyText;

        public void ResetRuntimeBindings()
        {
            iconText = null;
            bodyText = null;
        }

        public void Initialize(Text icon, Text body)
        {
            iconText = icon;
            bodyText = body;
        }

        public void Apply(RoundCollectedRewardTileDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (iconText != null) iconText.text = displayModel.IconText;
            if (bodyText != null) bodyText.text = displayModel.BodyText;
        }
    }
}
