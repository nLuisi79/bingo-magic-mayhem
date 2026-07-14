namespace BingoMagicMayhem.UI.Trail
{
    public sealed class TrailTaskRowDisplayModel
    {
        public TrailTaskRowDisplayModel(string title, string progress)
        {
            Title = title;
            Progress = progress;
        }

        public string Title { get; }
        public string Progress { get; }
    }
}
