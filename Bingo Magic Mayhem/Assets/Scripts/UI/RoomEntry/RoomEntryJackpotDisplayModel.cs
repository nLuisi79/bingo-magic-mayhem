namespace BingoMagicMayhem.UI.RoomEntry
{
    public sealed class RoomEntryJackpotDisplayModel
    {
        public RoomEntryJackpotDisplayModel(
            string title,
            string detail,
            bool showSpinPendingButton,
            string spinPendingButtonText)
        {
            Title = title;
            Detail = detail;
            ShowSpinPendingButton = showSpinPendingButton;
            SpinPendingButtonText = spinPendingButtonText;
        }

        public string Title { get; }
        public string Detail { get; }
        public bool ShowSpinPendingButton { get; }
        public string SpinPendingButtonText { get; }
    }
}
