using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Leaderboard
{
    [DisallowMultipleComponent]
    public sealed class LeaderboardScopeNoteView : MonoBehaviour
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

        public void Apply(LeaderboardScopeNoteDisplayModel displayModel)
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
