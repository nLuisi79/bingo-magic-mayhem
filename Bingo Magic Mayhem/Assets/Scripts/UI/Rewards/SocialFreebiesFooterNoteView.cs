using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class SocialFreebiesFooterNoteView : MonoBehaviour
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

        public void Apply(SocialFreebiesFooterNoteDisplayModel displayModel)
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
