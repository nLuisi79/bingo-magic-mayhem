using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BingoMagicMayhem.Cosmetics;
using BingoMagicMayhem.Infrastructure;
using BingoMagicMayhem.Jackpot;
using BingoMagicMayhem.Multiplayer;
using BingoMagicMayhem.Rounds;
using BingoMagicMayhem.UI.Common;
using BingoMagicMayhem.UI.Bazaar;
using BingoMagicMayhem.UI.Collections;
using BingoMagicMayhem.UI.Coven;
using BingoMagicMayhem.UI.Den;
using BingoMagicMayhem.UI.Friends;
using BingoMagicMayhem.UI.Inbox;
using BingoMagicMayhem.UI.Leaderboard;
using BingoMagicMayhem.UI.Market;
using BingoMagicMayhem.UI.Rewards;
using BingoMagicMayhem.UI.RoomEntry;
using BingoMagicMayhem.UI.Trail;
using BingoMagicMayhem.UI.Navigation;
using BingoMagicMayhem.UI.Profile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Daily Bonus and Daily Spin screens. Their rules live in DailyRewardsState (through PlayerInventoryState); this part of
// BingoPrototype only builds and animates the UI. Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildDailyBonusUi()
    {
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailySpinModal();
        RemoveDailyBonusModal();

        GameObject panel = CreateAnchoredPanel(contentRoot, "DailyBonusModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1220, 650, 0f, -34f);
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, -34f, panel.transform.localPosition.z);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        Text headerTitleText = CreateAnchoredText(panel.transform, string.Empty, 54, FontStyle.Bold, gold, 800, 66, 0f, 270f);
        Button info = CreateAnchoredButton(panel.transform, string.Empty, 22, 42, 42, new Color(0.18f, 0.16f, 0.28f), 338f, 272f);
        Text streakText = CreateAnchoredText(panel.transform, $"Daily Streak: {inventory.DailyBonusStreak} days", 26, FontStyle.Bold, Color.white, 420, 42, -350f, 214f);
        int nextChest = inventory.GetNextDailyBonusChestDay();
        Text nextChestText = CreateAnchoredText(panel.transform, $"Next Chest: Day {nextChest}", 26, FontStyle.Bold, Color.white, 420, 42, 350f, 214f);
        Text streakWarningText = null;
        Button saveStreak = null;

        if (inventory.HasMissedDailyBonusStreak())
        {
            streakWarningText = CreateAnchoredText(panel.transform, "You missed a day. Save the streak or claim to restart at Day 1.", 17, FontStyle.Bold, new Color(1f, 0.86f, 0.62f), 760, 28, 0f, 184f);
            saveStreak = CreateAnchoredButton(panel.transform, string.Empty, 16, 280, 38, inventory.CanUseDailyBonusStreakSave() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, 184f);
            saveStreak.interactable = inventory.CanUseDailyBonusStreakSave();
        }

        DailyBonusHeaderView headerView = panel.GetComponent<DailyBonusHeaderView>();
        if (headerView == null)
        {
            headerView = panel.AddComponent<DailyBonusHeaderView>();
        }

        headerView.ResetRuntimeBindings();
        headerView.Initialize(headerTitleText, streakText, nextChestText, streakWarningText);
        headerView.Apply(new DailyBonusHeaderDisplayModel(
            "DAILY BONUS",
            $"Daily Streak: {inventory.DailyBonusStreak} days",
            $"Next Chest: Day {nextChest}",
            inventory.HasMissedDailyBonusStreak() ? "You missed a day. Save the streak or claim to restart at Day 1." : string.Empty));

        CreateDailyBonusMilestoneRail(panel.transform, nextChest);
        Text sectionTitleText = CreateAnchoredText(panel.transform, string.Empty, 34, FontStyle.Bold, gold, 620, 48, 0f, 62f);

        DailyBonusModalView modalView = panel.GetComponent<DailyBonusModalView>();
        if (modalView == null)
        {
            modalView = panel.AddComponent<DailyBonusModalView>();
        }

        modalView.ResetRuntimeBindings();
        modalView.Initialize(headerTitleText, sectionTitleText, info, saveStreak);
        modalView.InfoRequested += ShowDailyBonusInfoRequested;
        if (saveStreak != null)
        {
            modalView.SaveStreakRequested += SaveDailyBonusStreakFromDen;
        }

        modalView.Apply(new DailyBonusModalDisplayModel
        {
            HeaderTitleText = "DAILY BONUS",
            SectionTitleText = "7-DAY DAILY BONUS",
            InfoButtonText = "i",
            SaveStreakButtonText = inventory.GetDailyBonusStreakSaveSummary(),
            SaveStreakButtonVisible = inventory.HasMissedDailyBonusStreak(),
            SaveStreakButtonInteractable = inventory.CanUseDailyBonusStreakSave()
        });

        for (int day = 1; day <= 7; day++)
        {
            DailyBonusRewardDefinition reward = inventory.GetDailyBonusRewardForDay(day);
            CreateDailyRewardTile(panel.transform, reward, day, -480f + (day - 1) * 160f, -82f);
        }

        string claimText = inventory.HasMissedDailyBonusStreak() ? "Let Streak Reset" : inventory.CanClaimDailyBonus() ? "Claim" : "Claimed Today";
        Button claim = CreateAnchoredButton(panel.transform, claimText, 24, 270, 54, inventory.CanClaimDailyBonus() ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, -262f);
        claim.interactable = inventory.CanClaimDailyBonus();
        claim.onClick.AddListener(ClaimDailyBonusFromDen);

        Button close = CreateDenReturnFooter(panel.transform, -470f, -262f, BuildPlayerDenUi);
        close.GetComponent<Image>().color = new Color(0.18f, 0.16f, 0.22f);
        DailyBonusActionAreaView actionAreaView = panel.GetComponent<DailyBonusActionAreaView>();
        if (actionAreaView == null)
        {
            actionAreaView = panel.AddComponent<DailyBonusActionAreaView>();
        }

        actionAreaView.ResetRuntimeBindings();
        actionAreaView.Initialize(claim, close);
        actionAreaView.Apply(new DailyBonusActionAreaDisplayModel(claimText, "Back to Den"));
    }

    private void CreateDailyBonusMilestoneRail(Transform parent, int nextChest)
    {
        int[] milestones = { 7, 14, 30, 60, 100, 180, 365 };
        float railWidth = 860f;
        int railMax = Mathf.Max(100, nextChest);
        CreateAnchoredPanel(parent, "DailyBonusRail", new Color(0.86f, 0.53f, 0.05f), railWidth, 16, 0f, 150f);
        for (int index = 0; index < milestones.Length; index++)
        {
            int day = milestones[index];
            if (day > railMax)
            {
                continue;
            }

            float x = -railWidth * 0.5f + (day / (float)railMax) * railWidth;
            bool reached = inventory.DailyBonusStreak >= day;
            bool target = day == nextChest;
            GameObject marker = CreateAnchoredPanel(parent, $"DailyMilestone_{day}", target ? new Color(0.55f, 0.18f, 0.78f) : reached ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.2f, 0.12f, 0.28f), target ? 74 : 58, target ? 58 : 46, x, 150f);
            Text badgeText = CreateAnchoredText(marker.transform, target ? "CHEST" : "GIFT", target ? 13 : 11, FontStyle.Bold, Color.white, target ? 66 : 50, 18, 0f, 8f);
            Text dayText = CreateAnchoredText(parent, day.ToString(), 22, FontStyle.Bold, Color.white, 80, 26, x, 108f);
            DailyBonusMilestoneMarkerView markerView = marker.GetComponent<DailyBonusMilestoneMarkerView>();
            if (markerView == null)
            {
                markerView = marker.AddComponent<DailyBonusMilestoneMarkerView>();
            }

            markerView.ResetRuntimeBindings();
            markerView.Initialize(badgeText, dayText);
            markerView.Apply(new DailyBonusMilestoneMarkerDisplayModel(target ? "CHEST" : "GIFT", day.ToString()));
        }

        int progressTarget = Mathf.Max(1, nextChest);
        string progressTextValue = $"{Mathf.Min(inventory.DailyBonusStreak, progressTarget)}/{progressTarget}";
        Text progressText = CreateAnchoredText(parent, progressTextValue, 24, FontStyle.Bold, Color.white, 160, 34, 0f, 108f);
        DailyBonusMilestoneRailView railView = parent.gameObject.GetComponent<DailyBonusMilestoneRailView>();
        if (railView == null)
        {
            railView = parent.gameObject.AddComponent<DailyBonusMilestoneRailView>();
        }

        railView.ResetRuntimeBindings();
        railView.Initialize(progressText);
        railView.Apply(new DailyBonusMilestoneRailDisplayModel(progressTextValue));
    }

    private void CreateDailyRewardTile(Transform parent, DailyBonusRewardDefinition reward, int day, float x, float y)
    {
        bool current = inventory.GetDailyBonusDisplayDay() == day;
        bool claimedCurrent = current && !inventory.CanClaimDailyBonus();
        bool ready = current && inventory.CanClaimDailyBonus();
        Color tileColor = ready ? new Color(0.72f, 0.44f, 0.08f) : new Color(0.18f, 0.08f, 0.28f);
        GameObject tile = CreateAnchoredPanel(parent, $"DailyReward_{day}", tileColor, 142, 190, x, y);
        Text dayText = CreateAnchoredText(tile.transform, $"DAY {day}", 23, FontStyle.Bold, Color.white, 124, 28, 0f, 68f);
        Text iconText = CreateAnchoredText(tile.transform, GetDailyRewardIcon(reward), 38, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 120, 54, 0f, 18f);
        string rewardText = reward.Label == "Daily Chest" ? "Daily Bonus\nChest" : reward.Summary;
        Text rewardSummaryText = CreateAnchoredText(tile.transform, rewardText, 13, FontStyle.Bold, new Color(0.92f, 0.88f, 1f), 124, 54, 0f, -46f);
        string status = string.Empty;

        if (claimedCurrent)
        {
            status = "CLAIMED";
        }
        else if (ready)
        {
            status = "READY";
        }

        Text statusText = CreateAnchoredText(tile.transform, status, 14, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 112, 24, 0f, -78f);
        DailyRewardTileView tileView = tile.GetComponent<DailyRewardTileView>();
        if (tileView == null)
        {
            tileView = tile.AddComponent<DailyRewardTileView>();
        }

        tileView.ResetRuntimeBindings();
        tileView.Initialize(dayText, iconText, rewardSummaryText, statusText);
        tileView.Apply(new DailyRewardTileDisplayModel($"DAY {day}", GetDailyRewardIcon(reward), rewardText, status));
    }

    private string GetDailyRewardIcon(DailyBonusRewardDefinition reward)
    {
        if (reward.Label == "Daily Chest") return "CHEST";
        if (reward.Crystals > 0) return "<>";
        if (reward.ItemName == "Regular Card") return "CARD";
        if (reward.ItemName == "Clairvoyance") return "EYE";
        if (reward.ItemName.Contains("Sigil")) return "SIGIL";
        return "*";
    }

    private void ClaimDailyBonusFromDen()
    {
        DailyBonusRewardDefinition reward = inventory.ClaimDailyBonus();
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.DailyBonusClaimed,
            PrototypeAnalyticsPayloadFactory.CreateDailyBonusClaimedJson(reward, inventory.DailyBonusStreak));
        coven.AddPrototypeChatLine($"Daily bonus collected: {reward.Summary}");
        RefreshPowerUpDisplays();
        BuildDailyBonusUi();
    }

    private void SaveDailyBonusStreakFromDen()
    {
        bool saved = inventory.TryUseDailyBonusStreakSave();
        coven.AddPrototypeChatLine(saved ? "Daily streak saved. You can claim without resetting it." : "Daily streak save unavailable.");
        RefreshPowerUpDisplays();
        BuildDailyBonusUi();
    }

    private void ShowDailyBonusInfoRequested()
    {
        coven.AddPrototypeChatLine("Daily Bonus: claim once per day. The 7-day track is the main loop; the streak rail tracks long-running chest milestones.");
    }

    private void BuildDailySpinUi()
    {
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateModalShell("DailySpinModal", 1320, 700, new Color(0.035f, 0.014f, 0.055f, 0.985f));
        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, -42f, panel.transform.localPosition.z);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        bool ready = inventory.CanClaimDailySpin() && !dailySpinAnimating;

        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        CreateAnchoredPanel(panel.transform, "DailySpinHeaderRibbon", new Color(0.14f, 0.04f, 0.18f), 1040, 84, 0f, 270f);
        Text spinHeaderTitleText = CreateAnchoredText(panel.transform, "Daily Wheelspin", 54, FontStyle.Bold, gold, 760, 64, -30f, 284f);
        Text spinHeaderSubtitleText = CreateAnchoredText(panel.transform, "One free daily spin. Separate from Daily Bonus and jackpot wheelspin.", 18, FontStyle.Bold, muted, 800, 28, -30f, 238f);

        GameObject wheel = CreateAnchoredPanel(panel.transform, "DailySpinWheel", new Color(0.13f, 0.04f, 0.19f), 540, 540, -350f, 6f);
        BuildDailySpinWheel(wheel.transform, rewards, dailySpinAnimating ? -1 : lastDailySpinRewardIndex);

        GameObject prizePanel = CreateAnchoredPanel(panel.transform, "DailySpinPrizePanel", new Color(0.08f, 0.12f, 0.08f), 480, 426, 340f, 22f);
        string panelSubtitle = dailySpinAnimating ? "Wheel spinning..." : ready ? "Free spin ready" : "Come back tomorrow";
        string sectionLabel;
        string panelMainText;
        string panelNoteText;
        if (!dailySpinAnimating && lastDailySpinRewardIndex >= 0 && lastDailySpinRewardIndex < rewards.Count)
        {
            PrototypeRewardGrant result = rewards[lastDailySpinRewardIndex];
            sectionLabel = "RESULT";
            panelMainText = BuildRewardGrantSummary(result);
            panelNoteText = "Reward added directly to inventory.";
        }
        else
        {
            sectionLabel = "SPIN FOR";
            panelMainText = "Mana, Crystals,\nSigils, Boosts";
            panelNoteText = "Prototype reward values and odds are not final.";
        }

        Text prizeTitleText = CreateAnchoredText(prizePanel.transform, "TODAY'S SPIN", 28, FontStyle.Bold, gold, 420, 42, 0f, 162f);
        Text prizeSubtitleText = CreateAnchoredText(prizePanel.transform, panelSubtitle, 20, FontStyle.Bold, Color.white, 400, 30, 0f, 122f);
        Text prizeSectionText = CreateAnchoredText(prizePanel.transform, sectionLabel, 18, FontStyle.Bold, muted, 380, 26, 0f, 64f);
        Text prizeMainText = CreateAnchoredText(prizePanel.transform, panelMainText, sectionLabel == "RESULT" ? 36 : 34, FontStyle.Bold, gold, 420, sectionLabel == "RESULT" ? 58 : 94, 0f, sectionLabel == "RESULT" ? 16f : 10f);
        Text prizeNoteText = CreateAnchoredText(prizePanel.transform, panelNoteText, 16, FontStyle.Bold, muted, 380, sectionLabel == "RESULT" ? 28 : 32, 0f, sectionLabel == "RESULT" ? -34f : -62f);
        DailySpinPrizePanelView prizeView = prizePanel.GetComponent<DailySpinPrizePanelView>();
        if (prizeView == null)
        {
            prizeView = prizePanel.AddComponent<DailySpinPrizePanelView>();
        }

        prizeView.ResetRuntimeBindings();
        prizeView.Initialize(prizeTitleText, prizeSubtitleText, prizeSectionText, prizeMainText, prizeNoteText);
        prizeView.Apply(new DailySpinPrizePanelDisplayModel("TODAY'S SPIN", panelSubtitle, sectionLabel, panelMainText, panelNoteText));

        if (!string.IsNullOrWhiteSpace(lastDailySpinSummary))
        {
            CreateAnchoredText(panel.transform, lastDailySpinSummary, 18, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 820, 36, 0f, -278f);
        }

        Button spin = CreateAnchoredButton(panel.transform, dailySpinAnimating ? "SPINNING" : ready ? "SPIN" : "SPUN", dailySpinAnimating ? 21 : 34, 180, 180, ready ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), -350f, 6f);
        spin.interactable = ready;
        spin.onClick.AddListener(ClaimDailySpinFromDen);

        Button close = CreateDenReturnFooter(panel.transform, 340f, -214f, BuildPlayerDenUi);
        close.GetComponent<Image>().color = new Color(0.18f, 0.16f, 0.22f);
        close.interactable = !dailySpinAnimating;
        DailySpinActionAreaView actionAreaView = panel.GetComponent<DailySpinActionAreaView>();
        if (actionAreaView == null)
        {
            actionAreaView = panel.AddComponent<DailySpinActionAreaView>();
        }

        string spinButtonText = dailySpinAnimating ? "SPINNING" : ready ? "SPIN" : "SPUN";
        actionAreaView.ResetRuntimeBindings();
        actionAreaView.Initialize(spin, close);
        actionAreaView.Apply(new DailySpinActionAreaDisplayModel(spinButtonText, "Back to Den"));
        DailySpinChromeView chromeView = panel.GetComponent<DailySpinChromeView>();
        if (chromeView == null)
        {
            chromeView = panel.AddComponent<DailySpinChromeView>();
        }

        chromeView.ResetRuntimeBindings();
        chromeView.Initialize(spinHeaderTitleText, spinHeaderSubtitleText, close);
        chromeView.Apply(new DailySpinChromeDisplayModel(
            "Daily Wheelspin",
            "One free daily spin. Separate from Daily Bonus and jackpot wheelspin.",
            "Back to Den"));
    }

    private void BuildDailySpinWheel(Transform wheel, IReadOnlyList<PrototypeRewardGrant> rewards, int winningIndex)
    {
        CreateAnchoredPanel(wheel, "DailySpinOuterRing", new Color(0.78f, 0.48f, 0.06f), 558, 558, 0f, 0f);
        CreateAnchoredPanel(wheel, "DailySpinInnerPlate", new Color(0.09f, 0.02f, 0.14f), 500, 500, 0f, 0f);
        Color[] colors =
        {
            new Color(0.15f, 0.32f, 0.10f),
            new Color(0.17f, 0.08f, 0.34f),
            new Color(0.07f, 0.22f, 0.34f),
            new Color(0.42f, 0.20f, 0.05f),
            new Color(0.08f, 0.35f, 0.32f),
            new Color(0.34f, 0.08f, 0.34f)
        };

        int segmentCount = Mathf.Max(1, rewards.Count);
        float angleStep = 360f / segmentCount;
        for (int index = 0; index < rewards.Count; index++)
        {
            float angle = dailySpinWheelRotation + 90f - index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians) * 168f;
            float y = Mathf.Sin(radians) * 168f;
            bool winner = index == winningIndex;
            Color color = winner ? new Color(0.88f, 0.58f, 0.08f) : colors[index % colors.Length];
            float segmentWidth = segmentCount > 12 ? 118f : 162f;
            float segmentHeight = segmentCount > 12 ? 82f : 96f;
            GameObject segment = CreateAnchoredPanel(wheel, $"DailySpinSegment_{index}", color, winner ? segmentWidth + 18f : segmentWidth, segmentHeight, x, y);
            RectTransform rect = segment.GetComponent<RectTransform>();
            rect.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            float labelRadius = 186f;
            float labelX = Mathf.Cos(radians) * labelRadius;
            float labelY = Mathf.Sin(radians) * labelRadius;
            string labelText = GetDailySpinWheelLabel(rewards[index]);
            int labelSize = rewards[index].Mana > 0 ? 28 : 17;
            Text label = CreateAnchoredText(wheel, labelText, winner ? labelSize + 3 : labelSize, FontStyle.Bold, Color.white, 104, 42, labelX, labelY);
            label.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            DailySpinWheelSegmentView segmentView = segment.GetComponent<DailySpinWheelSegmentView>();
            if (segmentView == null)
            {
                segmentView = segment.AddComponent<DailySpinWheelSegmentView>();
            }

            segmentView.ResetRuntimeBindings();
            segmentView.Initialize(label);
            segmentView.Apply(new DailySpinWheelSegmentDisplayModel(labelText));
        }

        CreateAnchoredPanel(wheel, "DailySpinPointer", new Color(0.78f, 0.48f, 0.06f), 42, 66, 0f, 262f);
        Text pointerText = CreateAnchoredText(wheel, "v", 30, FontStyle.Bold, new Color(0.18f, 0.04f, 0.2f), 42, 42, 0f, 268f);
        CreateAnchoredPanel(wheel, "DailySpinHub", new Color(0.43f, 0.12f, 0.55f), 204, 204, 0f, 0f);
        DailySpinWheelChromeView chromeView = wheel.gameObject.GetComponent<DailySpinWheelChromeView>();
        if (chromeView == null)
        {
            chromeView = wheel.gameObject.AddComponent<DailySpinWheelChromeView>();
        }

        chromeView.ResetRuntimeBindings();
        chromeView.Initialize(pointerText);
        chromeView.Apply(new DailySpinWheelChromeDisplayModel("v"));
    }

    private string GetDailySpinWheelLabel(PrototypeRewardGrant reward)
    {
        if (reward.Mana > 0) return reward.Mana.ToString("N0");
        if (reward.Crystals > 0) return $"{reward.Crystals}<>";
        if (reward.ClairvoyanceMinutes > 0) return $"{reward.ClairvoyanceMinutes}m Boost";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Sigil")) return "SIGIL";
        if (reward.Items.Count > 0) return GetDailySpinFallbackLabel(reward.Items[0]);
        return "Reward";
    }

    private string GetDailySpinFallbackLabel(PrototypeRewardItem item)
    {
        return item.Quantity > 1 ? $"{item.Name}\nx{item.Quantity}" : item.Name;
    }

    private string GetDailySpinRewardIcon(PrototypeRewardGrant reward)
    {
        if (reward.Mana > 0) return "MANA";
        if (reward.Crystals > 0) return "<>";
        if (reward.ClairvoyanceMinutes > 0) return "EYE";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Pandora")) return "BOX";
        if (reward.Items.Count > 0 && reward.Items[0].Name.Contains("Sigil")) return "SIGIL";
        return "*";
    }

    private void ClaimDailySpinFromDen()
    {
        if (!inventory.CanClaimDailySpin())
        {
            lastDailySpinSummary = "Already spun today.";
            BuildDailySpinUi();
            return;
        }

        PrototypeRewardGrant reward = inventory.ClaimDailySpin();
        string summary = BuildRewardGrantSummary(reward);
        lastDailySpinRewardIndex = FindDailySpinRewardIndex(reward);
        lastDailySpinSummary = "Wheel spinning...";
        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.DailySpinClaimed,
            PrototypeAnalyticsPayloadFactory.CreateDailySpinClaimedJson(reward, lastDailySpinRewardIndex));
        coven.AddPrototypeChatLine($"Daily Spin collected: {summary}");
        RefreshPowerUpDisplays();
        dailySpinAnimating = true;
        dailySpinAnimationSequence++;
        int animationSequence = dailySpinAnimationSequence;
        BuildDailySpinUi();
        StartCoroutine(AnimateDailySpinWheel(animationSequence, summary, lastDailySpinRewardIndex));
    }

    private IEnumerator AnimateDailySpinWheel(int animationSequence, string summary, int targetIndex)
    {
        float startRotation = dailySpinWheelRotation;
        float targetRotation = startRotation + 1080f + GetDailySpinWheelDeltaToTarget(startRotation, targetIndex);
        float elapsed = 0f;
        const float duration = 1.35f;
        while (elapsed < duration)
        {
            if (animationSequence != dailySpinAnimationSequence)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            dailySpinWheelRotation = Mathf.Lerp(startRotation, targetRotation, eased);
            BuildDailySpinUi();
            yield return null;
        }

        if (animationSequence != dailySpinAnimationSequence)
        {
            yield break;
        }

        dailySpinWheelRotation = targetRotation;
        dailySpinAnimating = false;
        lastDailySpinSummary = $"Won: {summary}";
        BuildDailySpinUi();
    }

    private float GetDailySpinWheelDeltaToTarget(float startRotation, int targetIndex)
    {
        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        if (rewards.Count <= 0 || targetIndex < 0)
        {
            return 0f;
        }

        float angleStep = 360f / rewards.Count;
        float targetMod = Mathf.Repeat(targetIndex * angleStep, 360f);
        float currentMod = Mathf.Repeat(startRotation, 360f);
        return Mathf.Repeat(targetMod - currentMod, 360f);
    }

    private int FindDailySpinRewardIndex(PrototypeRewardGrant reward)
    {
        IReadOnlyList<PrototypeRewardGrant> rewards = inventory.GetDailySpinPrototypeRewards();
        for (int index = 0; index < rewards.Count; index++)
        {
            PrototypeRewardGrant candidate = rewards[index];
            if (candidate.Mana == reward.Mana &&
                candidate.Crystals == reward.Crystals &&
                candidate.ClairvoyanceMinutes == reward.ClairvoyanceMinutes &&
                candidate.Items.Count == reward.Items.Count)
            {
                if (candidate.Items.Count == 0 ||
                    (candidate.Items[0].Name == reward.Items[0].Name && candidate.Items[0].Quantity == reward.Items[0].Quantity))
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private void RemoveDailyBonusModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DailyBonusModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }

    private void RemoveDailySpinModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("DailySpinModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }
}
