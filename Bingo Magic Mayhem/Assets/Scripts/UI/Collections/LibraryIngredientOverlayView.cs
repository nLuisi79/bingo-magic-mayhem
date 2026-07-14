using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class LibraryIngredientOverlayView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text helperText;
        [SerializeField] private Button backButton;

        public event Action BackRequested;

        public void ResetRuntimeBindings()
        {
            if (backButton != null) backButton.onClick.RemoveListener(HandleBackRequested);

            titleText = null;
            progressText = null;
            helperText = null;
            backButton = null;
        }

        public void Initialize(Text title, Text progress, Text helper, Button back)
        {
            titleText = title;
            progressText = progress;
            helperText = helper;
            backButton = back;

            if (backButton != null)
            {
                backButton.onClick.RemoveListener(HandleBackRequested);
                backButton.onClick.AddListener(HandleBackRequested);
            }
        }

        public void Apply(LibraryIngredientOverlayDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null) titleText.text = displayModel.TitleText;
            if (progressText != null) progressText.text = displayModel.ProgressText;
            if (helperText != null) helperText.text = displayModel.HelperText;

            if (backButton != null)
            {
                Text text = backButton.GetComponentInChildren<Text>();
                if (text != null) text.text = displayModel.BackButtonText;
            }
        }

        private void HandleBackRequested() => BackRequested?.Invoke();
    }
}
