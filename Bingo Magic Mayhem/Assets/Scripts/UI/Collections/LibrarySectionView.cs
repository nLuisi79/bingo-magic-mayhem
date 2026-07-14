using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibrarySectionView : MonoBehaviour
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

        public void Apply(LibrarySectionDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
        }
    }
}
