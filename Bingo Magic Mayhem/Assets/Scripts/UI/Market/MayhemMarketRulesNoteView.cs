using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Market
{
    [DisallowMultipleComponent]
    public sealed class MayhemMarketRulesNoteView : MonoBehaviour
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

        public void Apply(MayhemMarketRulesNoteDisplayModel displayModel)
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
