using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class BookOfShadowsLibraryPanelView : MonoBehaviour
    {
        [SerializeField] private Button fullClaimButton;
        [SerializeField] private Text footerNoteText;

        public event Action FullClaimRequested;

        public void ResetRuntimeBindings()
        {
            if (fullClaimButton != null) fullClaimButton.onClick.RemoveListener(HandleFullClaimRequested);

            fullClaimButton = null;
            footerNoteText = null;
        }

        public void Initialize(Button fullClaim, Text footerNote)
        {
            fullClaimButton = fullClaim;
            footerNoteText = footerNote;

            if (fullClaimButton != null)
            {
                fullClaimButton.onClick.RemoveListener(HandleFullClaimRequested);
                fullClaimButton.onClick.AddListener(HandleFullClaimRequested);
            }
        }

        public void Apply(BookOfShadowsLibraryPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (footerNoteText != null) footerNoteText.text = displayModel.FooterNoteText;

            if (fullClaimButton != null)
            {
                fullClaimButton.interactable = displayModel.FullClaimButtonInteractable;
                Text text = fullClaimButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.FullClaimButtonText;
            }
        }

        private void HandleFullClaimRequested() => FullClaimRequested?.Invoke();
    }
}
