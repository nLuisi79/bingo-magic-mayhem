using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    [DisallowMultipleComponent]
    public sealed class UIInteractionHandler : MonoBehaviour
    {
        [SerializeField] private Button button;

        public event Action Requested;

        public void Initialize(Button targetButton)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
            }

            button = targetButton;
            if (button != null)
            {
                button.onClick.RemoveListener(HandleRequested);
                button.onClick.AddListener(HandleRequested);
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
