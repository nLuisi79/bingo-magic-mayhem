using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryStatView : MonoBehaviour
    {
        [SerializeField] private Text valueText;
        [SerializeField] private Text labelText;

        public void ResetRuntimeBindings()
        {
            valueText = null;
            labelText = null;
        }

        public void Initialize(Text value, Text label)
        {
            valueText = value;
            labelText = label;
        }

        public void Apply(LibraryStatDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (valueText != null) valueText.text = displayModel.ValueText;
            if (labelText != null) labelText.text = displayModel.LabelText;
        }
    }
}
