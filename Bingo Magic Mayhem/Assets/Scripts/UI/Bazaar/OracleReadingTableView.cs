using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Bazaar
{
    [DisallowMultipleComponent]
    public sealed class OracleReadingTableView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text statusText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            statusText = null;
        }

        public void Initialize(Text title, Text status)
        {
            titleText = title;
            statusText = status;
        }

        public void Apply(OracleReadingTableDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }

            if (statusText != null)
            {
                statusText.text = displayModel.StatusText;
            }
        }
    }
}
