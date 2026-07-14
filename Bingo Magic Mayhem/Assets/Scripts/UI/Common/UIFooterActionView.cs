using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UIFooterActionView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public event Action Requested;

        public void ResetRuntimeBindings()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
            }

            button = null;
        }

        public void Initialize(Button targetButton)
        {
            button = targetButton;
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
                button.onClick.AddListener(HandleRequested);
            }
        }

        public void Apply(UIFooterActionDisplayModel displayModel)
        {
            if (displayModel == null || button == null)
            {
                return;
            }

            button.interactable = displayModel.Interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = displayModel.ButtonText;
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
            }
        }

        private void HandleRequested()
        {
            Requested?.Invoke();
        }
    }
}
