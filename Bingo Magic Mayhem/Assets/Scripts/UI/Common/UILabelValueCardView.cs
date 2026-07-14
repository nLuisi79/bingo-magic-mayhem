using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UILabelValueCardView : MonoBehaviour
    {
        [SerializeField] private Text labelText;
        [SerializeField] private Text valueText;

        public void ResetRuntimeBindings()
        {
            labelText = null;
            valueText = null;
        }

        public void Initialize(Text label, Text value)
        {
            labelText = label;
            valueText = value;
        }

        public void Apply(string label, string value)
        {
            if (labelText != null)
            {
                labelText.text = label;
            }

            if (valueText != null)
            {
                valueText.text = value;
            }
        }
    }
}
