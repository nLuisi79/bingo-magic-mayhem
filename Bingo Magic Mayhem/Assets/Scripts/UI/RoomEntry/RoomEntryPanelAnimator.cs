using System.Collections;
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
        [SerializeField] private float introDuration = 0.18f;
        [SerializeField] private float emphasisDuration = 0.16f;

        private Coroutine activeRoutine;

        private void Awake()
        {
            if (targetCanvasGroup == null)
            {
                targetCanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void ResetRuntimeBindings()
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }

            targetCanvasGroup = null;
        }

        public void ShowIdleState()
        {
            FadeTo(idleAlpha, introDuration);
        }

        public void ShowEmphasisState()
        {
            FadeTo(emphasisAlpha, emphasisDuration);
        }

        public void PlayScreenIntro()
        {
            if (targetCanvasGroup == null)
            {
                return;
            }

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(PlayScreenIntroRoutine());
        }

        private void ApplyAlpha(float alpha)
        {
            if (targetCanvasGroup != null)
            {
                targetCanvasGroup.alpha = alpha;
            }
        }

        private void FadeTo(float targetAlpha, float duration)
        {
            if (targetCanvasGroup == null)
            {
                return;
            }

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
        }

        private IEnumerator PlayScreenIntroRoutine()
        {
            targetCanvasGroup.alpha = 0.82f;
            transform.localScale = new Vector3(0.985f, 0.985f, 1f);

            float elapsed = 0f;
            float duration = Mathf.Max(0.001f, introDuration);
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                targetCanvasGroup.alpha = Mathf.Lerp(0.82f, idleAlpha, progress);
                float scale = Mathf.Lerp(0.985f, 1f, progress);
                transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            ApplyAlpha(idleAlpha);
            transform.localScale = Vector3.one;
            activeRoutine = null;
        }

        private IEnumerator FadeRoutine(float targetAlpha, float duration)
        {
            float startAlpha = targetCanvasGroup.alpha;
            float elapsed = 0f;
            float clampedDuration = Mathf.Max(0.001f, duration);

            while (elapsed < clampedDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / clampedDuration);
                ApplyAlpha(Mathf.Lerp(startAlpha, targetAlpha, progress));
                yield return null;
            }

            ApplyAlpha(targetAlpha);
            activeRoutine = null;
        }
    }
}
