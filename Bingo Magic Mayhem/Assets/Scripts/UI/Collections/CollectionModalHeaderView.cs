using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class CollectionModalHeaderView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            subtitleText = null;
        }

        public void Initialize(Text title, Text subtitle)
        {
            titleText = title;
            subtitleText = subtitle;
        }

        public void Apply(CollectionModalHeaderDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (subtitleText != null) subtitleText.text = displayModel.SubtitleText;
        }
    }
}
