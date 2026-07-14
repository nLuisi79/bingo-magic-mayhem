using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Bazaar
{
    [DisallowMultipleComponent]
    public sealed class BazaarFooterNoteView : MonoBehaviour
    {
        [SerializeField] private Text noteText;

        public void ResetRuntimeBindings()
        {
            noteText = null;
        }

        public void Initialize(Text note)
        {
            noteText = note;
        }

        public void Apply(BazaarFooterNoteDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (noteText != null)
            {
                noteText.text = displayModel.NoteText;
            }
        }
    }
}
