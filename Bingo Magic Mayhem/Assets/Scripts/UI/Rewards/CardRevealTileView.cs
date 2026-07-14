using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class CardRevealTileView : MonoBehaviour
    {
        [SerializeField] private Text starsText;
        [SerializeField] private Text cardNameText;
        [SerializeField] private Text rarityText;
        [SerializeField] private Text potionNameText;
        [SerializeField] private Text statusText;

        public void ResetRuntimeBindings()
        {
            starsText = null;
            cardNameText = null;
            rarityText = null;
            potionNameText = null;
            statusText = null;
        }

        public void Initialize(Text stars, Text cardName, Text rarity, Text potionName, Text status)
        {
            starsText = stars;
            cardNameText = cardName;
            rarityText = rarity;
            potionNameText = potionName;
            statusText = status;
        }

        public void Apply(CardRevealTileDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (starsText != null) starsText.text = displayModel.StarsText;
            if (cardNameText != null) cardNameText.text = displayModel.CardName;
            if (rarityText != null) rarityText.text = displayModel.RarityText;
            if (potionNameText != null) potionNameText.text = displayModel.PotionName;

            if (statusText != null)
            {
                statusText.text = displayModel.StatusText;
                statusText.gameObject.SetActive(!string.IsNullOrWhiteSpace(displayModel.StatusText));
            }
        }
    }
}
