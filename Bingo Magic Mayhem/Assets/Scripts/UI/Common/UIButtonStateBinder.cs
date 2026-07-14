using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Common
{
    /// <summary>
    /// Keeps button visuals separate from what the button means in gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIButtonStateBinder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private enum VisualState
        {
            Idle,
            Hover,
            Pressed,
            Disabled
        }

        [SerializeField] private Selectable selectable;
        [SerializeField] private Graphic[] targetGraphics;
        [SerializeField] private RectTransform targetTransform;
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 0.98f, 0.9f, 1f);
        [SerializeField] private Color pressedColor = new Color(0.95f, 0.9f, 0.8f, 1f);
        [SerializeField] private Color disabledColor = new Color(1f, 1f, 1f, 0.45f);
        [SerializeField] private Vector3 idleScale = Vector3.one;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.02f, 1.02f, 1f);
        [SerializeField] private Vector3 pressedScale = new Vector3(0.98f, 0.98f, 1f);
        [SerializeField] private bool refreshEveryFrame;

        private bool isHovered;
        private bool isPressed;

        private void Reset()
        {
            selectable = GetComponent<Selectable>();
            targetTransform = transform as RectTransform;
            if (targetGraphics == null || targetGraphics.Length == 0)
            {
                Graphic graphic = GetComponent<Graphic>();
                if (graphic != null)
                {
                    targetGraphics = new[] { graphic };
                }
            }
        }

        private void Awake()
        {
            if (selectable == null)
            {
                selectable = GetComponent<Selectable>();
            }

            if (targetTransform == null)
            {
                targetTransform = transform as RectTransform;
            }
        }

        private void OnEnable()
        {
            RefreshVisualState();
        }

        private void Update()
        {
            if (refreshEveryFrame)
            {
                RefreshVisualState();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            RefreshVisualState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isPressed = false;
            RefreshVisualState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            RefreshVisualState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            RefreshVisualState();
        }

        public void RefreshVisualState()
        {
            ApplyState(GetVisualState());
        }

        private VisualState GetVisualState()
        {
            if (selectable != null && !selectable.IsInteractable())
            {
                return VisualState.Disabled;
            }

            if (isPressed)
            {
                return VisualState.Pressed;
            }

            if (isHovered)
            {
                return VisualState.Hover;
            }

            return VisualState.Idle;
        }

        private void ApplyState(VisualState state)
        {
            Color color = idleColor;
            Vector3 scale = idleScale;

            switch (state)
            {
                case VisualState.Hover:
                    color = hoverColor;
                    scale = hoverScale;
                    break;
                case VisualState.Pressed:
                    color = pressedColor;
                    scale = pressedScale;
                    break;
                case VisualState.Disabled:
                    color = disabledColor;
                    scale = idleScale;
                    break;
            }

            if (targetGraphics != null)
            {
                for (int index = 0; index < targetGraphics.Length; index++)
                {
                    if (targetGraphics[index] != null)
                    {
                        targetGraphics[index].color = color;
                    }
                }
            }

            if (targetTransform != null)
            {
                targetTransform.localScale = scale;
            }
        }
    }
}
