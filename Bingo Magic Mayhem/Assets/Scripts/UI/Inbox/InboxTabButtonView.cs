using System;
using BingoMagicMayhem.UI.Common;
using UnityEngine;

namespace BingoMagicMayhem.UI.Inbox
{
    [DisallowMultipleComponent]
    public sealed class InboxTabButtonView : MonoBehaviour
    {
        [SerializeField] private UITabButtonBinder binder;
        [SerializeField] private string tabId;

        public event Action<string> TabSelected;

        public void ResetRuntimeBindings()
        {
            if (binder != null)
            {
                binder.Selected -= HandleSelected;
            }

            binder = null;
            tabId = string.Empty;
        }

        public void Initialize(UITabButtonBinder tabBinder, string id)
        {
            if (binder != null)
            {
                binder.Selected -= HandleSelected;
            }

            binder = tabBinder;
            tabId = id;
            if (binder != null)
            {
                binder.Selected += HandleSelected;
            }
        }

        private void OnDestroy()
        {
            if (binder != null)
            {
                binder.Selected -= HandleSelected;
            }
        }

        private void HandleSelected(UITabButtonBinder _)
        {
            TabSelected?.Invoke(tabId);
        }
    }
}
