using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Trail
{
    [DisallowMultipleComponent]
    public sealed class TrailRulesCardView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text noteText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            bodyText = null;
            noteText = null;
        }

        public void Initialize(Text title, Text body, Text note)
        {
            titleText = title;
            bodyText = body;
            noteText = note;
        }

        public void Apply(TrailRulesCardDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.Title;
            if (bodyText != null) bodyText.text = displayModel.BodyText;
            if (noteText != null) noteText.text = displayModel.NoteText;
        }
    }
}
