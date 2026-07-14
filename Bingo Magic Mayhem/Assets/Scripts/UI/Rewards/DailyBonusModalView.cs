using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Rewards
{
    [DisallowMultipleComponent]
    public sealed class DailyBonusModalView : MonoBehaviour
    {
        [SerializeField] private Text headerTitleText;
        [SerializeField] private Text sectionTitleText;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button saveStreakButton;

        public event Action InfoRequested;
        public event Action SaveStreakRequested;

        public void ResetRuntimeBindings()
        {
            if (infoButton != null) infoButton.onClick.RemoveListener(HandleInfoRequested);
            if (saveStreakButton != null) saveStreakButton.onClick.RemoveListener(HandleSaveStreakRequested);

            headerTitleText = null;
            sectionTitleText = null;
            infoButton = null;
            saveStreakButton = null;
        }

        public void Initialize(Text headerTitle, Text sectionTitle, Button info, Button saveStreak)
        {
            headerTitleText = headerTitle;
            sectionTitleText = sectionTitle;
            infoButton = info;
            saveStreakButton = saveStreak;

            if (infoButton != null)
            {
                infoButton.onClick.RemoveListener(HandleInfoRequested);
                infoButton.onClick.AddListener(HandleInfoRequested);
            }

            if (saveStreakButton != null)
            {
                saveStreakButton.onClick.RemoveListener(HandleSaveStreakRequested);
                saveStreakButton.onClick.AddListener(HandleSaveStreakRequested);
            }
        }

        public void Apply(DailyBonusModalDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (headerTitleText != null) headerTitleText.text = displayModel.HeaderTitleText;
            if (sectionTitleText != null) sectionTitleText.text = displayModel.SectionTitleText;
            ApplyButton(infoButton, displayModel.InfoButtonText, true, true);
            ApplyButton(saveStreakButton, displayModel.SaveStreakButtonText, displayModel.SaveStreakButtonInteractable, displayModel.SaveStreakButtonVisible);
        }

        private static void ApplyButton(Button button, string label, bool interactable, bool visible)
        {
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(visible);
            if (!visible)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void OnDestroy()
        {
            if (infoButton != null) infoButton.onClick.RemoveListener(HandleInfoRequested);
            if (saveStreakButton != null) saveStreakButton.onClick.RemoveListener(HandleSaveStreakRequested);
        }

        private void HandleInfoRequested() => InfoRequested?.Invoke();
        private void HandleSaveStreakRequested() => SaveStreakRequested?.Invoke();
    }
}
