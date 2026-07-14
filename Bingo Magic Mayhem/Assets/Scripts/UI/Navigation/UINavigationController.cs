using System.Collections.Generic;
using UnityEngine;

namespace BingoMagicMayhem.UI.Navigation
{
    /// <summary>
    /// Lightweight back-stack for presentation panels. Gameplay state remains elsewhere.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UINavigationController : MonoBehaviour
    {
        [SerializeField] private UIModalController rootPanel;

        private readonly Stack<UIModalController> panelStack = new Stack<UIModalController>();

        public int Depth => panelStack.Count;

        private void Start()
        {
            if (rootPanel != null)
            {
                ShowRoot(rootPanel);
            }
        }

        public void ShowRoot(UIModalController panel)
        {
            if (panel == null)
            {
                return;
            }

            while (panelStack.Count > 0)
            {
                UIModalController stackedPanel = panelStack.Pop();
                if (stackedPanel != null && stackedPanel != panel)
                {
                    stackedPanel.HideInstant();
                }
            }

            panel.ShowInstant();
            panelStack.Push(panel);
        }

        public void Push(UIModalController panel)
        {
            if (panel == null)
            {
                return;
            }

            UIModalController current = Peek();
            if (current == panel)
            {
                panel.Show();
                return;
            }

            if (current != null)
            {
                current.Hide();
            }

            panel.Show();
            panelStack.Push(panel);
        }

        public void Pop()
        {
            if (panelStack.Count <= 1)
            {
                return;
            }

            UIModalController current = panelStack.Pop();
            if (current != null)
            {
                current.Hide();
            }

            UIModalController previous = panelStack.Peek();
            previous?.Show();
        }

        public void ClearToRoot()
        {
            if (panelStack.Count == 0)
            {
                return;
            }

            while (panelStack.Count > 1)
            {
                UIModalController panel = panelStack.Pop();
                panel?.HideInstant();
            }

            UIModalController root = panelStack.Peek();
            root?.ShowInstant();
        }

        private UIModalController Peek()
        {
            return panelStack.Count > 0 ? panelStack.Peek() : null;
        }
    }
}
