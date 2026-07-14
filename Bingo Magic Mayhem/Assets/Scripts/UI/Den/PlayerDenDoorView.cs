using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenDoorView : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Button button;

        public event Action Selected;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleSelected);
            }

            titleText = null;
            subtitleText = null;
            button = null;
        }

        public void Initialize(Text title, Text subtitle, Button doorButton)
        {
            titleText = title;
            subtitleText = subtitle;
            button = doorButton;
            if (button != null)
            {
                button.onClick.RemoveListener(HandleSelected);
                button.onClick.AddListener(HandleSelected);
            }
        }

        public void Apply(PlayerDenDoorDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = displayModel.TitleText;
            }

            if (subtitleText != null)
            {
                subtitleText.text = displayModel.SubtitleText;
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleSelected);
            }
        }

        private void HandleSelected()
        {
            Selected?.Invoke();
        }
    }
}
