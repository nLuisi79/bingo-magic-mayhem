using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Bazaar
{
    [DisallowMultipleComponent]
    public sealed class BazaarDestinationPanelView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Text hintText;
        [SerializeField] private Button button;

        public event Action Selected;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleButtonClicked);
            }

            titleText = null;
            subtitleText = null;
            detailText = null;
            hintText = null;
            button = null;
        }

        public void Initialize(Text title, Text subtitle, Text detail, Text hint, Button actionButton)
        {
            titleText = title;
            subtitleText = subtitle;
            detailText = detail;
            hintText = hint;
            button = actionButton;

            if (button != null)
            {
                button.onClick.RemoveListener(HandleButtonClicked);
                button.onClick.AddListener(HandleButtonClicked);
            }
        }

        public void Apply(BazaarDestinationPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.Title;
            }

            if (subtitleText != null)
            {
                subtitleText.text = displayModel.Subtitle;
            }

            if (detailText != null)
            {
                detailText.text = displayModel.Detail;
            }

            if (hintText != null)
            {
                hintText.text = displayModel.Hint;
            }
        }

        private void HandleButtonClicked()
        {
            Selected?.Invoke();
        }
    }
}
