using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class PlayerProfileSummaryRowView : MonoBehaviour
    {
        [SerializeField] private Text labelText;
        [SerializeField] private Text valueText;

        public Text ValueText => valueText;

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
