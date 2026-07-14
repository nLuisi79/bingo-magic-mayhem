using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Friends
{
    [DisallowMultipleComponent]
    public sealed class FriendsSectionPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text emptyStateText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            emptyStateText = null;
        }

        public void Initialize(Text title, Text emptyState)
        {
            titleText = title;
            emptyStateText = emptyState;
        }

        public void Apply(FriendsSectionPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }

            if (emptyStateText != null)
            {
                emptyStateText.gameObject.SetActive(!string.IsNullOrWhiteSpace(displayModel.EmptyStateText));
                emptyStateText.text = displayModel.EmptyStateText;
            }
        }
    }
}
