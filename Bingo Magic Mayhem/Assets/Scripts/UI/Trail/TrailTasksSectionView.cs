using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Trail
{
    [DisallowMultipleComponent]
    public sealed class TrailTasksSectionView : MonoBehaviour
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

        public void Apply(TrailTasksSectionDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }
        }
    }
}
