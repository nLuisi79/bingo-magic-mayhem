using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Market
{
    [DisallowMultipleComponent]
    public sealed class MayhemMarketWalletView : MonoBehaviour
    {
        [SerializeField] private Text walletText;

        public void ResetRuntimeBindings()
        {
            walletText = null;
        }

        public void Initialize(Text wallet)
        {
            walletText = wallet;
        }

        public void Apply(MayhemMarketWalletDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (walletText != null)
            {
                walletText.text = displayModel.WalletText;
            }
        }
    }
}
