namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class PrototypeResourceTileDisplayModel
    {
        public PrototypeResourceTileDisplayModel(
            string label,
            string value,
            string iconText,
            float width,
            float x,
            float y,
            bool activatesPlayerDen)
        {
            Label = label;
            Value = value;
            IconText = iconText;
            Width = width;
            X = x;
            Y = y;
            ActivatesPlayerDen = activatesPlayerDen;
        }

        public string Label { get; }
        public string Value { get; }
        public string IconText { get; }
        public float Width { get; }
        public float X { get; }
        public float Y { get; }
        public bool ActivatesPlayerDen { get; }
    }

    public sealed class PrototypeGameplayHeaderDisplayModel
    {
        public PrototypeGameplayHeaderDisplayModel(
            string roomName,
            string realmName,
            string manaText,
            string crystalText,
            string ballsLeftText)
        {
            RoomName = roomName;
            RealmName = realmName;
            ManaText = manaText;
            CrystalText = crystalText;
            BallsLeftText = ballsLeftText;
        }

        public string RoomName { get; }
        public string RealmName { get; }
        public string ManaText { get; }
        public string CrystalText { get; }
        public string BallsLeftText { get; }
    }

    public sealed class PrototypeGameplayLeftRailDisplayModel
    {
        public PrototypeGameplayLeftRailDisplayModel(
            string primaryLabel,
            string primaryValue,
            string autoClaimLabel,
            string autoClaimStatus,
            string historyLabel,
            string historyText)
        {
            PrimaryLabel = primaryLabel;
            PrimaryValue = primaryValue;
            AutoClaimLabel = autoClaimLabel;
            AutoClaimStatus = autoClaimStatus;
            HistoryLabel = historyLabel;
            HistoryText = historyText;
        }

        public string PrimaryLabel { get; }
        public string PrimaryValue { get; }
        public string AutoClaimLabel { get; }
        public string AutoClaimStatus { get; }
        public string HistoryLabel { get; }
        public string HistoryText { get; }
    }

    public sealed class PrototypeGameplayCenterDisplayModel
    {
        public PrototypeGameplayCenterDisplayModel(string bannerText, string roundSummaryText)
        {
            BannerText = bannerText;
            RoundSummaryText = roundSummaryText;
        }

        public string BannerText { get; }
        public string RoundSummaryText { get; }
    }

    public sealed class PrototypeGameplayRightRailDisplayModel
    {
        public PrototypeGameplayRightRailDisplayModel(
            string nextCallLabel,
            string currentCallText,
            string timerText,
            string queueLabel,
            string[] queueEntries)
        {
            NextCallLabel = nextCallLabel;
            CurrentCallText = currentCallText;
            TimerText = timerText;
            QueueLabel = queueLabel;
            QueueEntries = queueEntries;
        }

        public string NextCallLabel { get; }
        public string CurrentCallText { get; }
        public string TimerText { get; }
        public string QueueLabel { get; }
        public string[] QueueEntries { get; }
    }

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

    public static class PrototypeHeaderDisplayFactory
    {
        public static PrototypeGameplayHeaderDisplayModel BuildGameplayHeader(
            string roomName,
            string realmName,
            string manaText,
            string crystalText,
            int ballsLeft)
        {
            return new PrototypeGameplayHeaderDisplayModel(
                roomName,
                realmName,
                manaText,
                crystalText,
                ballsLeft.ToString());
        }

        public static PrototypeResourceTileDisplayModel BuildLobbyResourceTile(
            string label,
            string value,
            string iconText,
            float width,
            float x,
            float y = 0f,
            bool activatesPlayerDen = false)
        {
            return new PrototypeResourceTileDisplayModel(
                label,
                value,
                iconText,
                width,
                x,
                y,
                activatesPlayerDen);
        }

        public static PrototypeGameplayLeftRailDisplayModel BuildGameplayLeftRail(
            bool isBlackoutRoom,
            int totalBingos,
            string autoClaimStatus,
            string calledHistoryText)
        {
            return new PrototypeGameplayLeftRailDisplayModel(
                isBlackoutRoom ? "BLACKOUTS" : "BINGOS",
                totalBingos.ToString(),
                "AUTO-CLAIM",
                autoClaimStatus,
                "ALL CALLED\nNUMBERS",
                calledHistoryText);
        }

        public static PrototypeGameplayCenterDisplayModel BuildGameplayCenter(
            string bannerText,
            string roundSummaryText)
        {
            return new PrototypeGameplayCenterDisplayModel(bannerText, roundSummaryText);
        }

        public static PrototypeGameplayRightRailDisplayModel BuildGameplayRightRail(
            string currentCallText,
            string timerText,
            string[] queueEntries)
        {
            return new PrototypeGameplayRightRailDisplayModel(
                "NEXT CALL",
                currentCallText,
                timerText,
                "CALL QUEUE",
                queueEntries ?? new string[0]);
        }
    }
}
