using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    /// <summary>
    /// Visual tab-state binder that raises selection intent upward.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class UITabButtonBinder : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Graphic[] selectedGraphics;
        [SerializeField] private Graphic[] unselectedGraphics;
        [SerializeField] private GameObject[] selectedOnlyObjects;
        [SerializeField] private GameObject[] unselectedOnlyObjects;
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.7f);
        [SerializeField] private bool startSelected;

        public event Action<UITabButtonBinder> Selected;

        public bool IsSelected { get; private set; }

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.AddListener(HandleClicked);
            ApplySelection(startSelected);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleClicked);
            }
        }

        public void SetSelected(bool isSelected)
        {
            ApplySelection(isSelected);
        }

        private void HandleClicked()
        {
            Selected?.Invoke(this);
        }

        private void ApplySelection(bool isSelected)
        {
            IsSelected = isSelected;

            SetGraphicColors(selectedGraphics, isSelected ? selectedColor : unselectedColor);
            SetGraphicColors(unselectedGraphics, isSelected ? unselectedColor : selectedColor);
            SetObjectsActive(selectedOnlyObjects, isSelected);
            SetObjectsActive(unselectedOnlyObjects, !isSelected);
        }

        private static void SetGraphicColors(Graphic[] graphics, Color color)
        {
            if (graphics == null)
            {
                return;
            }

            for (int index = 0; index < graphics.Length; index++)
            {
                if (graphics[index] != null)
                {
                    graphics[index].color = color;
                }
            }
        }

        private static void SetObjectsActive(GameObject[] objects, bool isActive)
        {
            if (objects == null)
            {
                return;
            }

            for (int index = 0; index < objects.Length; index++)
            {
                if (objects[index] != null)
                {
                    objects[index].SetActive(isActive);
                }
            }
        }
    }
}
