using UnityEngine;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Lightweight hook for room-entry presentation emphasis and future animation wiring.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryPanelAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup targetCanvasGroup;
        [SerializeField] private float idleAlpha = 1f;
        [SerializeField] private float emphasisAlpha = 1f;

        private void Awake()
        {
            if (targetCanvasGroup == null)
            {
                targetCanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void ResetRuntimeBindings()
        {
            targetCanvasGroup = null;
        }

        public void ShowIdleState()
        {
            ApplyAlpha(idleAlpha);
        }

        public void ShowEmphasisState()
        {
            ApplyAlpha(emphasisAlpha);
        }

        private void ApplyAlpha(float alpha)
        {
            if (targetCanvasGroup != null)
            {
                targetCanvasGroup.alpha = alpha;
            }
        }
    }
}
