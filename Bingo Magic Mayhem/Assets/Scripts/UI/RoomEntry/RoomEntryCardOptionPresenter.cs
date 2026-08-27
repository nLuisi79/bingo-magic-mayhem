using UnityEngine;

namespace BingoMagicMayhem.UI.RoomEntry
{
    public static class RoomEntryCardOptionPresenter
    {
        public static RoomEntryCardOptionDisplayModel Build(
            int cardCount,
            int selectedCardCount,
            int manaBetPerCard,
            bool canAfford,
            bool blockedByHandoff,
            bool blockedByReconnect,
            string flavorText,
            string ingredientText,
            string chanceText)
        {
            int entryCost = cardCount * manaBetPerCard;
            bool canPlay = canAfford && !blockedByHandoff && !blockedByReconnect;
            string title = $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}";
            string actionText = blockedByReconnect
                ? "Reconnecting"
                : blockedByHandoff
                    ? "Syncing Room"
                    : canAfford
                        ? $"Play {entryCost} Mana"
                        : $"Need {entryCost} Mana";
            Color backgroundColor = cardCount == selectedCardCount
                ? new Color(0.92f, 0.82f, 0.42f)
                : new Color(0.97f, 0.89f, 0.68f);
            Color actionColor = canPlay
                ? new Color(0.12f, 0.55f, 0.08f)
                : new Color(0.36f, 0.36f, 0.36f);

            return new RoomEntryCardOptionDisplayModel(
                cardCount,
                title,
                flavorText,
                ingredientText,
                chanceText,
                actionText,
                canPlay,
                backgroundColor,
                actionColor);
        }
    }
}
