using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Friends
{
    [DisallowMultipleComponent]
    public sealed class FriendsModalView : MonoBehaviour
    {
        [SerializeField] private Text summaryText;
        [SerializeField] private Text footerStatusText;

        public void ResetRuntimeBindings()
        {
            summaryText = null;
            footerStatusText = null;
        }

        public void Initialize(Text summary, Text footerStatus)
        {
            summaryText = summary;
            footerStatusText = footerStatus;
        }

        public void Apply(FriendsModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (summaryText != null) summaryText.text = displayModel.SummaryText;
            if (footerStatusText != null) footerStatusText.text = displayModel.FooterStatusText;
        }
    }
}
