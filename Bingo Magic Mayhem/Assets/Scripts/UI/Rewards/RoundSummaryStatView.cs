using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RoundSummaryStatView : MonoBehaviour
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

        public void Apply(RoundSummaryStatDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (labelText != null) labelText.text = displayModel.LabelText;
            if (valueText != null) valueText.text = displayModel.ValueText;
        }
    }
}
