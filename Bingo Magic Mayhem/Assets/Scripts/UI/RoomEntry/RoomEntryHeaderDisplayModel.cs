namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class RoomEntryHeaderDisplayModel
    {
        public RoomEntryHeaderDisplayModel(
            string roomName,
            string realmName,
            string potionLabel,
            string rankText,
            string modeLabel,
            string modeIconText,
            bool isSpecialMode)
        {
            RoomName = roomName;
            RealmName = realmName;
            PotionLabel = potionLabel;
            RankText = rankText;
            ModeLabel = modeLabel;
            ModeIconText = modeIconText;
            IsSpecialMode = isSpecialMode;
        }

        public string RoomName { get; }
        public string RealmName { get; }
        public string PotionLabel { get; }
        public string RankText { get; }
        public string ModeLabel { get; }
        public string ModeIconText { get; }
        public bool IsSpecialMode { get; }
    }
}
