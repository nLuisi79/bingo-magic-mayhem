using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailySpinPrizePanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text sectionLabelText;
        [SerializeField] private Text mainText;
        [SerializeField] private Text noteText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            subtitleText = null;
            sectionLabelText = null;
            mainText = null;
            noteText = null;
        }

        public void Initialize(Text title, Text subtitle, Text sectionLabel, Text main, Text note)
        {
            titleText = title;
            subtitleText = subtitle;
            sectionLabelText = sectionLabel;
            mainText = main;
            noteText = note;
        }

        public void Apply(DailySpinPrizePanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.Title;
            if (subtitleText != null) subtitleText.text = displayModel.Subtitle;
            if (sectionLabelText != null) sectionLabelText.text = displayModel.SectionLabel;
            if (mainText != null) mainText.text = displayModel.MainText;
            if (noteText != null) noteText.text = displayModel.NoteText;
        }
    }
}
