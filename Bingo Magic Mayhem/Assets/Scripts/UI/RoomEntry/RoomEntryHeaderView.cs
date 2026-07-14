using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Presentation wrapper for the runtime-built room-entry header.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryHeaderView : MonoBehaviour
    {
        [SerializeField] private Text roomNameText;
        [SerializeField] private Text realmNameText;
        [SerializeField] private Text potionLabelText;
        [SerializeField] private Text rankText;
        [SerializeField] private Image modeBadgeBackground;
        [SerializeField] private Text modeBadgeText;
        [SerializeField] private Text modeIconText;

        public void ResetRuntimeBindings()
        {
            roomNameText = null;
            realmNameText = null;
            potionLabelText = null;
            rankText = null;
            modeBadgeBackground = null;
            modeBadgeText = null;
            modeIconText = null;
        }

        public void Initialize(
            Text roomNameLabel,
            Text realmNameLabel,
            Text potionLabel,
            Text rankLabel,
            Image modeBadgeImage,
            Text modeBadgeLabel,
            Text modeIconLabel)
        {
            roomNameText = roomNameLabel;
            realmNameText = realmNameLabel;
            potionLabelText = potionLabel;
            rankText = rankLabel;
            modeBadgeBackground = modeBadgeImage;
            modeBadgeText = modeBadgeLabel;
            modeIconText = modeIconLabel;
        }

        public void Apply(RoomEntryHeaderDisplayModel displayModel)
        {
            if (roomNameText != null)
            {
                roomNameText.text = displayModel.RoomName;
            }

            if (realmNameText != null)
            {
                realmNameText.text = displayModel.RealmName;
            }

            if (potionLabelText != null)
            {
                potionLabelText.text = displayModel.PotionLabel;
            }

            if (rankText != null)
            {
                rankText.text = displayModel.RankText;
            }

            if (modeBadgeBackground != null)
            {
                modeBadgeBackground.color = displayModel.IsSpecialMode
                    ? new Color(0.38f, 0.08f, 0.58f)
                    : new Color(0.18f, 0.11f, 0.34f);
            }

            if (modeBadgeText != null)
            {
                modeBadgeText.text = displayModel.ModeLabel;
            }

            if (modeIconText != null)
            {
                modeIconText.text = displayModel.ModeIconText;
                modeIconText.color = displayModel.IsSpecialMode
                    ? new Color(0.35f, 0.06f, 0.52f)
                    : new Color(0.9f, 0.18f, 1f);
            }
        }
    }
}
