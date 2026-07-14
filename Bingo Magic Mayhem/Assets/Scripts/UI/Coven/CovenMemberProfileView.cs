using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Coven
{
    [DisallowMultipleComponent]
    public sealed class CovenMemberProfileView : MonoBehaviour
    {
        [SerializeField] private Text summaryText;
        [SerializeField] private Text notesText;
        [SerializeField] private Text queueText;

        public void ResetRuntimeBindings()
        {
            summaryText = null;
            notesText = null;
            queueText = null;
        }

        public void Initialize(Text summary, Text notes, Text queue)
        {
            summaryText = summary;
            notesText = notes;
            queueText = queue;
        }

        public void Apply(CovenMemberProfileDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (summaryText != null) summaryText.text = displayModel.SummaryText;
            if (notesText != null) notesText.text = displayModel.NotesText;
            if (queueText != null) queueText.text = displayModel.QueueText;
        }
    }
}
