namespace BingoMagicMayhem.UI.Profile
{
    public sealed class PlayerSettingsTabDisplayModel
    {
        public PlayerSettingsTabDisplayModel(
            string displayName,
            string statusMessage,
            bool soundEnabled,
            bool notificationsEnabled)
        {
            DisplayName = displayName;
            StatusMessage = statusMessage;
            SoundEnabled = soundEnabled;
            NotificationsEnabled = notificationsEnabled;
        }

        public string DisplayName { get; }
        public string StatusMessage { get; }
        public bool SoundEnabled { get; }
        public bool NotificationsEnabled { get; }
    }
}
