using System;
using System.Collections.Generic;
using BingoMagicMayhem.UI.Common;
using BingoMagicMayhem.UI.Navigation;
using UnityEngine;

namespace BingoMagicMayhem.UI.Profile
{
    [DisallowMultipleComponent]
    public sealed class PlayerProfileModalView : MonoBehaviour
    {
        [SerializeField] private UIModalChromeView modalChromeView;

        private readonly List<PlayerProfileTabButtonView> tabs = new List<PlayerProfileTabButtonView>();

        public event Action CloseRequested;
        public event Action<string> TabRequested;

        public void ResetRuntimeBindings()
        {
            if (modalChromeView != null)
            {
                modalChromeView.CloseRequested -= HandleCloseRequested;
            }

            for (int index = 0; index < tabs.Count; index++)
            {
                tabs[index].TabSelected -= HandleTabRequested;
            }

            tabs.Clear();
            modalChromeView = null;
        }

        public void Initialize(UIModalChromeView chromeView)
        {
            if (modalChromeView != null)
            {
                modalChromeView.CloseRequested -= HandleCloseRequested;
            }

            modalChromeView = chromeView;

            if (modalChromeView != null)
            {
                modalChromeView.CloseRequested += HandleCloseRequested;
            }
        }

        public void RegisterTab(PlayerProfileTabButtonView tabView)
        {
            if (tabView == null)
            {
                return;
            }

            tabView.TabSelected -= HandleTabRequested;
            if (!tabs.Contains(tabView))
            {
                tabs.Add(tabView);
            }
            tabView.TabSelected += HandleTabRequested;
        }

        public void Apply(PlayerProfileModalDisplayModel displayModel)
        {
            if (modalChromeView != null)
            {
                modalChromeView.Apply(displayModel.Title, displayModel.Subtitle, displayModel.CloseButtonText);
            }
        }

        public void SetActiveTab(string tabId)
        {
            for (int index = 0; index < tabs.Count; index++)
            {
                tabs[index].SetSelected(tabs[index].Matches(tabId));
            }
        }

        private void OnDestroy()
        {
            if (modalChromeView != null)
            {
                modalChromeView.CloseRequested -= HandleCloseRequested;
            }

            for (int index = 0; index < tabs.Count; index++)
            {
                tabs[index].TabSelected -= HandleTabRequested;
            }
        }

        private void HandleCloseRequested()
        {
            CloseRequested?.Invoke();
        }

        private void HandleTabRequested(string tabId)
        {
            TabRequested?.Invoke(tabId);
        }
    }
}
