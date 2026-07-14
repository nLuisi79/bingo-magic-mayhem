using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UITitleNoteCountRowView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text noteText;
        [SerializeField] private Text countText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            noteText = null;
            countText = null;
        }

        public void Initialize(Text title, Text note, Text count)
        {
            titleText = title;
            noteText = note;
            countText = count;
        }

        public void Apply(string title, string note, string count)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (noteText != null)
            {
                noteText.text = note;
            }

            if (countText != null)
            {
                countText.text = count;
            }
        }
    }
}
