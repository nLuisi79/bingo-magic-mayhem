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

// Enchanted Trail prototype screen and its locally saved claim state. Rewards, tasks and durations are placeholders;
// see 09_OPEN_DECISIONS.md. Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private const string PrototypeTrailRewardsSaveKey = "BMM.Prototype.EnchantedTrail.ClaimedRewards";

    private readonly HashSet<int> prototypeClaimedTrailRewards = new HashSet<int>();

    private void BuildEnchantedTrailUi()
    {
        RemoveEnchantedTrailModal();
        GameObject panel = CreateModalShell("EnchantedTrailModal", 1140, 650, new Color(0.045f, 0.03f, 0.075f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "ENCHANTED TRAIL",
            "Medium-term task path prototype. Separate from Daily Bonus, Daily Spin, Freebies, Realm income, Inbox, and jackpot wheelspin.",
            "Back to Den",
            gold,
            muted,
            violet,
            980f,
            274f,
            980f,
            38f,
            230f);
        chromeView.CloseRequested += RemoveEnchantedTrailModal;

        GameObject path = CreateModalSectionPanel(panel.transform, "TrailFreePath", 1000, 170, 0f, 112f, new Color(0.12f, 0.055f, 0.17f));
        Text freePathTitleText = CreateAnchoredText(path.transform, "FREE PATH", 22, FontStyle.Bold, gold, 920, 28, 0f, 52f);
        TrailFreePathView freePathView = path.GetComponent<TrailFreePathView>();
        if (freePathView == null)
        {
            freePathView = path.AddComponent<TrailFreePathView>();
        }

        freePathView.ResetRuntimeBindings();
        freePathView.Initialize(freePathTitleText);
        freePathView.Apply(new TrailFreePathDisplayModel("FREE PATH"));
        for (int index = 0; index < 7; index++)
        {
            float x = -390f + index * 130f;
            CreateTrailRewardNode(path.transform, index + 1, x, -18f);
            if (index < 6)
            {
                CreateAnchoredPanel(path.transform, $"TrailConnector_{index}", new Color(0.46f, 0.28f, 0.64f), 72, 8, x + 65f, -18f);
            }
        }

        GameObject tasks = CreateModalSectionPanel(panel.transform, "TrailTasks", 640, 250, -210f, -106f, new Color(0.09f, 0.045f, 0.13f));
        Text tasksTitleText = CreateAnchoredText(tasks.transform, "TRAIL TASKS", 24, FontStyle.Bold, gold, 570, 32, 0f, 94f);
        TrailTasksSectionView tasksView = tasks.GetComponent<TrailTasksSectionView>();
        if (tasksView == null)
        {
            tasksView = tasks.AddComponent<TrailTasksSectionView>();
        }

        tasksView.ResetRuntimeBindings();
        tasksView.Initialize(tasksTitleText);
        tasksView.Apply(new TrailTasksSectionDisplayModel("TRAIL TASKS"));
        CreateTrailTaskRow(tasks.transform, "Daub called numbers", "18 / 50", 0.36f, -150f, 38f);
        CreateTrailTaskRow(tasks.transform, "Play bingo rounds", "2 / 5", 0.4f, 150f, 38f);
        CreateTrailTaskRow(tasks.transform, "Restore a room", "0 / 1", 0f, -150f, -50f);
        CreateTrailTaskRow(tasks.transform, "Open Pandora", "0 / 1", 0f, 150f, -50f);

        GameObject rules = CreateModalSectionPanel(panel.transform, "TrailRules", 300, 250, 386f, -106f, new Color(0.13f, 0.08f, 0.15f));
        string rulesBody = "Free path only\nNo premium lane\nNo final reward values\nClaim state only\nNo rank scaling yet";
        string rulesNote = "Collect persists locally; no rewards granted.";
        Text rulesTitleText = CreateAnchoredText(rules.transform, "SCOPE", 24, FontStyle.Bold, gold, 250, 34, 0f, 92f);
        Text rulesBodyText = CreateAnchoredText(rules.transform, rulesBody, 16, FontStyle.Bold, Color.white, 250, 140, 0f, 4f);
        Text rulesNoteText = CreateAnchoredText(rules.transform, rulesNote, 13, FontStyle.Bold, muted, 250, 34, 0f, -94f);
        TrailRulesCardView rulesView = rules.GetComponent<TrailRulesCardView>();
        if (rulesView == null)
        {
            rulesView = rules.AddComponent<TrailRulesCardView>();
        }

        rulesView.ResetRuntimeBindings();
        rulesView.Initialize(rulesTitleText, rulesBodyText, rulesNoteText);
        rulesView.Apply(new TrailRulesCardDisplayModel("SCOPE", rulesBody, rulesNote));

        CreateAnchoredText(panel.transform, "Trail duration, point sources, task list, premium unlock, grand reward, and cosmetic rewards remain open decisions.", 15, FontStyle.Bold, muted, 940, 34, 0f, -238f);
    }

    private void CreateTrailRewardNode(Transform parent, int step, float x, float y)
    {
        bool unlocked = step <= 2;
        bool claimed = prototypeClaimedTrailRewards.Contains(step);
        Color nodeColor = claimed ? new Color(0.18f, 0.42f, 0.16f) : unlocked ? new Color(0.35f, 0.12f, 0.62f) : new Color(0.22f, 0.2f, 0.24f);
        Color textColor = unlocked ? Color.white : new Color(0.72f, 0.68f, 0.78f);
        GameObject node = CreateAnchoredPanel(parent, $"TrailNode_{step}", nodeColor, 92, 110, x, y);
        Text accentText = CreateAnchoredText(node.transform, claimed ? "+" : unlocked ? "*" : "-", 26, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 52, 28, 0f, 30f);
        Text titleText = CreateAnchoredText(node.transform, $"Step {step}", 12, FontStyle.Bold, textColor, 76, 18, 0f, 8f);
        Text detailText = CreateAnchoredText(node.transform, "Reward", 10, FontStyle.Bold, textColor, 76, 16, 0f, -12f);
        UIBadgeCardView badgeView = node.AddComponent<UIBadgeCardView>();
        badgeView.ResetRuntimeBindings();
        badgeView.Initialize(accentText, titleText, detailText);
        badgeView.Apply(claimed ? "+" : unlocked ? "*" : "-", $"Step {step}", "Reward");
        Button collect = CreateAnchoredButton(node.transform, claimed ? "Claimed" : unlocked ? "Collect" : "Locked", 10, 74, 24, claimed ? new Color(0.12f, 0.34f, 0.12f) : unlocked ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 0f, -38f);
        collect.interactable = unlocked && !claimed;
        int rewardStep = step;
        collect.onClick.AddListener(() => ClaimPrototypeTrailReward(rewardStep));
    }

    private void ClaimPrototypeTrailReward(int step)
    {
        prototypeClaimedTrailRewards.Add(step);
        SavePrototypeTrailState();
        BuildEnchantedTrailUi();
    }

    private void LoadPrototypeTrailState()
    {
        prototypeClaimedTrailRewards.Clear();
        string saved = DurablePrefs.GetString(PrototypeTrailRewardsSaveKey, "");
        if (string.IsNullOrWhiteSpace(saved))
        {
            return;
        }

        string[] steps = saved.Split(PrototypeFriendSaveSeparator);
        for (int index = 0; index < steps.Length; index++)
        {
            if (int.TryParse(steps[index].Trim(), out int step))
            {
                prototypeClaimedTrailRewards.Add(step);
            }
        }
    }

    private void SavePrototypeTrailState()
    {
        List<int> steps = new List<int>(prototypeClaimedTrailRewards);
        steps.Sort();
        List<string> serialized = new List<string>();
        for (int index = 0; index < steps.Count; index++)
        {
            serialized.Add(steps[index].ToString());
        }

        DurablePrefs.SetString(PrototypeTrailRewardsSaveKey, string.Join(PrototypeFriendSaveSeparator.ToString(), serialized));
        DurablePrefs.Save();
    }

    private void ResetPrototypeTrailState()
    {
        prototypeClaimedTrailRewards.Clear();
        SavePrototypeTrailState();
    }

    private void CreateTrailTaskRow(Transform parent, string title, string progress, float ratio, float x, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"TrailTask_{title}", 270, 74, x, y);
        Text titleText = CreateAnchoredText(row.transform, title, 14, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 242, 20, 0f, 20f);
        CreateAnchoredPanel(row.transform, "TaskBarBack", new Color(0.34f, 0.28f, 0.2f), 220, 10, 0f, -4f);
        float fillWidth = Mathf.Clamp01(ratio) * 220f;
        if (fillWidth > 0f)
        {
            CreateAnchoredPanel(row.transform, "TaskBarFill", new Color(0.35f, 0.12f, 0.62f), fillWidth, 10, -110f + fillWidth * 0.5f, -4f);
        }
        Text progressText = CreateAnchoredText(row.transform, progress, 12, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 220, 18, 0f, -24f);
        TrailTaskRowView rowView = row.GetComponent<TrailTaskRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<TrailTaskRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(titleText, progressText);
        rowView.Apply(new TrailTaskRowDisplayModel(title, progress));
    }

    private void RemoveEnchantedTrailModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("EnchantedTrailModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }
}
