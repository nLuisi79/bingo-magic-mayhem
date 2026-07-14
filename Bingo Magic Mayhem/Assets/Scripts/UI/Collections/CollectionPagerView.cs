using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class CollectionPagerView : MonoBehaviour
    {
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        public event Action PreviousRequested;
        public event Action NextRequested;

        public void ResetRuntimeBindings()
        {
            if (previousButton != null) previousButton.onClick.RemoveListener(HandlePreviousRequested);
            if (nextButton != null) nextButton.onClick.RemoveListener(HandleNextRequested);

            previousButton = null;
            nextButton = null;
        }

        public void Initialize(Button previous, Button next)
        {
            previousButton = previous;
            nextButton = next;

            if (previousButton != null)
            {
                previousButton.onClick.RemoveListener(HandlePreviousRequested);
                previousButton.onClick.AddListener(HandlePreviousRequested);
            }

            if (nextButton != null)
            {
                nextButton.onClick.RemoveListener(HandleNextRequested);
                nextButton.onClick.AddListener(HandleNextRequested);
            }
        }

        public void Apply(CollectionPagerDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            ApplyButton(previousButton, displayModel.PreviousButtonText, displayModel.PreviousButtonInteractable);
            ApplyButton(nextButton, displayModel.NextButtonText, displayModel.NextButtonInteractable);
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandlePreviousRequested() => PreviousRequested?.Invoke();
        private void HandleNextRequested() => NextRequested?.Invoke();
    }
}
