using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Trail
{
    [DisallowMultipleComponent]
    public sealed class TrailTaskRowView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text progressText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            progressText = null;
        }

        public void Initialize(Text title, Text progress)
        {
            titleText = title;
            progressText = progress;
        }

        public void Apply(TrailTaskRowDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.Title;
            if (progressText != null) progressText.text = displayModel.Progress;
        }
    }
}
