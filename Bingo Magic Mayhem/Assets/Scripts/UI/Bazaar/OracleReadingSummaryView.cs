using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Bazaar
{
    [DisallowMultipleComponent]
    public sealed class OracleReadingSummaryView : MonoBehaviour
    {
        [SerializeField] private Text summaryText;
        [SerializeField] private Button actionButton;

        public void ResetRuntimeBindings()
        {
            summaryText = null;
            actionButton = null;
        }

        public void Initialize(Text summary, Button action)
        {
            summaryText = summary;
            actionButton = action;
        }

        public void Apply(OracleReadingSummaryDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (summaryText != null)
            {
                summaryText.text = displayModel.SummaryText;
            }

            if (actionButton != null)
            {
                Text buttonText = actionButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = displayModel.ActionButtonText;
                }
            }
        }
    }
}
