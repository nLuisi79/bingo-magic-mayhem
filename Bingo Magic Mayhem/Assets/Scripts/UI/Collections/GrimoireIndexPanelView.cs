using System;
using UnityEngine;
using UnityEngine.UI;

namespace BingoMagicMayhem.UI.Collections
{
    [DisallowMultipleComponent]
    public sealed class GrimoireIndexPanelView : MonoBehaviour
    {
        [SerializeField] private Text collectedCountText;
        [SerializeField] private Text completionRewardText;
        [SerializeField] private Button claimButton;

        public event Action ClaimRequested;

        public void ResetRuntimeBindings()
        {
            if (claimButton != null) claimButton.onClick.RemoveListener(HandleClaimRequested);

            collectedCountText = null;
            completionRewardText = null;
            claimButton = null;
        }

        public void Initialize(
            Text collectedCount,
            Text completionReward,
            Button claim)
        {
            collectedCountText = collectedCount;
            completionRewardText = completionReward;
            claimButton = claim;

            if (claimButton != null)
            {
                claimButton.onClick.RemoveListener(HandleClaimRequested);
                claimButton.onClick.AddListener(HandleClaimRequested);
            }
        }

        public void Apply(GrimoireIndexPanelDisplayModel displayModel)
        {
            if (displayModel == null)
            {
                return;
            }

            if (collectedCountText != null) collectedCountText.text = displayModel.CollectedCountText;
            if (completionRewardText != null) completionRewardText.text = displayModel.CompletionRewardText;

            ApplyButton(claimButton, displayModel.ClaimButtonText, displayModel.ClaimButtonInteractable);
        }

        private static void ApplyButton(Button button, string label, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) text.text = label;
        }

        private void HandleClaimRequested() => ClaimRequested?.Invoke();
    }
}
