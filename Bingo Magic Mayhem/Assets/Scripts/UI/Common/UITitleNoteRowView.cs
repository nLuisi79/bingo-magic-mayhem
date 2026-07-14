using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UITitleNoteRowView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text noteText;

        public void ResetRuntimeBindings()
        {
            titleText = null;
            noteText = null;
        }

        public void Initialize(Text title, Text note)
        {
            titleText = title;
            noteText = note;
        }

        public void Apply(string title, string note)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }

            if (noteText != null)
            {
                noteText.text = note;
            }
        }
    }
}
