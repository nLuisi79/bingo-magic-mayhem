using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Den
{
    [DisallowMultipleComponent]
    public sealed class PlayerDenDoorView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public event Action Selected;

        public void Initialize(Button doorButton)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleSelected);
            }

            button = doorButton;
            if (button != null)
            {
                button.onClick.RemoveListener(HandleSelected);
                button.onClick.AddListener(HandleSelected);
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
