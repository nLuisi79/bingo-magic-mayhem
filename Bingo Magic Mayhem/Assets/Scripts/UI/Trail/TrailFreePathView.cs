using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Trail
{
    [DisallowMultipleComponent]
    public sealed class TrailFreePathView : MonoBehaviour
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

        public void Apply(TrailFreePathDisplayModel displayModel)
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
