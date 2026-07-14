using System.Collections.Generic;
using UnityEngine;

namespace BingoMagicMayhem.UI.RoomEntry
{
    /// <summary>
    /// Thin room-entry presentation shell for runtime-generated lobby content.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RoomEntryPanelView : MonoBehaviour
    {
        private readonly List<RoomEntryCardOptionView> cardOptionViews = new List<RoomEntryCardOptionView>();
        private RoomEntryBetRowView betRowView;
        private RoomEntryHeaderView headerView;
        private RoomEntryRestorePanelView restorePanelView;
        private RoomEntryJackpotView jackpotView;
        private RoomEntryPanelAnimator animator;

        public void ResetRuntimeBindings()
        {
            cardOptionViews.Clear();
            betRowView = null;
            headerView = null;
            restorePanelView = null;
            jackpotView = null;
            animator = null;
        }

        public void RegisterCardOption(RoomEntryCardOptionView view)
        {
            if (view != null)
            {
                if (!cardOptionViews.Contains(view))
                {
                    cardOptionViews.Add(view);
                }
            }
        }

        public void BindBetRow(RoomEntryBetRowView view)
        {
            betRowView = view;
        }

        public void BindHeader(RoomEntryHeaderView view)
        {
            headerView = view;
        }

        public void BindRestorePanel(RoomEntryRestorePanelView view)
        {
            restorePanelView = view;
        }

        public void BindJackpotPanel(RoomEntryJackpotView view)
        {
            jackpotView = view;
        }

        public void BindAnimator(RoomEntryPanelAnimator viewAnimator)
        {
            animator = viewAnimator;
        }

        public void SetBetSummary(string summary)
        {
            betRowView?.SetSummary(summary);
        }

        public void ApplyHeader(RoomEntryHeaderDisplayModel displayModel)
        {
            headerView?.Apply(displayModel);
        }

        public void ApplyRestorePanel(RoomEntryRestorePanelDisplayModel displayModel)
        {
            restorePanelView?.Apply(displayModel);
        }

        public void ApplyJackpotPanel(RoomEntryJackpotDisplayModel displayModel)
        {
            jackpotView?.Apply(displayModel);
        }

        public void ShowIdleState()
        {
            animator?.ShowIdleState();
        }
    }
}
