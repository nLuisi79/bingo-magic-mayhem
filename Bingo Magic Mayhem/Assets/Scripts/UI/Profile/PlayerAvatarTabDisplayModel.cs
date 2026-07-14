namespace BingoMagicMayhem.UI.Profile
{
    public sealed class PlayerAvatarTabDisplayModel
    {
        public PlayerAvatarTabDisplayModel(
            string avatarName,
            string cosmeticsSummary,
            string avatarValue,
            string frameValue,
            string dauberValue,
            string footnote)
        {
            AvatarName = avatarName;
            CosmeticsSummary = cosmeticsSummary;
            AvatarValue = avatarValue;
            FrameValue = frameValue;
            DauberValue = dauberValue;
            Footnote = footnote;
        }

        public string AvatarName { get; }
        public string CosmeticsSummary { get; }
        public string AvatarValue { get; }
        public string FrameValue { get; }
        public string DauberValue { get; }
        public string Footnote { get; }
    }
}
