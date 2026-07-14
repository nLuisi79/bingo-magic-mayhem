using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class RoundIngredientProgressTileView : MonoBehaviour
    {
        [SerializeField] private Text quantityText;
        [SerializeField] private Text nameText;

        public void ResetRuntimeBindings()
        {
            quantityText = null;
            nameText = null;
        }

        public void Initialize(Text quantity, Text name)
        {
            quantityText = quantity;
            nameText = name;
        }

        public void Apply(RoundIngredientProgressTileDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (quantityText != null) quantityText.text = displayModel.QuantityText;
            if (nameText != null) nameText.text = displayModel.NameText;
        }
    }
}
