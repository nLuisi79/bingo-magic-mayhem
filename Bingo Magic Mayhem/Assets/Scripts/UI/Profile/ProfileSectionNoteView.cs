using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class ProfileSectionNoteView : MonoBehaviour
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

        public void Apply(ProfileSectionNoteDisplayModel displayModel)
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
