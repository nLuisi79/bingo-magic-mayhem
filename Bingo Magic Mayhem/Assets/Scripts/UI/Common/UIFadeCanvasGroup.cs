using System.Collections;
using UnityEngine;

namespace BingoMagicMayhem.UI.Common
{
    /// <summary>
    /// Presentation-only CanvasGroup fade helper for panel and modal reveals.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIFadeCanvasGroup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private bool deactivateWhenHidden = true;

        private Coroutine activeFade;

        public bool IsVisible => canvasGroup != null && canvasGroup.alpha > 0.001f && gameObject.activeSelf;

        private void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void Show()
        {
            FadeTo(1f, true);
        }

        public void Hide()
        {
            FadeTo(0f, false);
        }

        public void ShowInstant()
        {
            SetVisibleInstant(true);
        }

        public void HideInstant()
        {
            SetVisibleInstant(false);
        }

        public void SetVisibleInstant(bool isVisible)
        {
            if (activeFade != null)
            {
                StopCoroutine(activeFade);
                activeFade = null;
            }

            if (isVisible)
            {
                gameObject.SetActive(true);
            }

            ApplyState(isVisible ? 1f : 0f, isVisible);

            if (!isVisible && deactivateWhenHidden)
            {
                gameObject.SetActive(false);
            }
        }

        private void FadeTo(float targetAlpha, bool visibleWhenComplete)
        {
            if (activeFade != null)
            {
                StopCoroutine(activeFade);
            }

            if (visibleWhenComplete)
            {
                gameObject.SetActive(true);
            }

            activeFade = StartCoroutine(FadeRoutine(targetAlpha, visibleWhenComplete));
        }

        private IEnumerator FadeRoutine(float targetAlpha, bool visibleWhenComplete)
        {
            float startAlpha = canvasGroup.alpha;
            float duration = Mathf.Max(0.001f, fadeDuration);
            float elapsed = 0f;

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                ApplyState(alpha, visibleWhenComplete && progress >= 1f);
                yield return null;
            }

            ApplyState(targetAlpha, visibleWhenComplete);

            if (!visibleWhenComplete && deactivateWhenHidden)
            {
                gameObject.SetActive(false);
            }

            activeFade = null;
        }

        private void ApplyState(float alpha, bool interactable)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = interactable;
            canvasGroup.interactable = interactable;
        }
    }
}
