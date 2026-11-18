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

// Leaderboard screen (prototype rows and tabs).
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildLeaderboardUi()
    {
        RemoveLeaderboardModal();
        RemoveFriendsModal();
        RemoveInboxModal();
        RemoveSocialFreebiesModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();

        GameObject panel = CreateModalShell("LeaderboardModal", 1040, 620, new Color(0.055f, 0.028f, 0.085f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "LEADERBOARDS",
            "Prototype gameplay standings only. Rewards, seasons, scoring formula, and anti-abuse rules are not active.",
            "Back to Den",
            gold,
            muted,
            violet,
            860f,
            258f,
            880f,
            34f,
            216f);
        chromeView.CloseRequested += RemoveLeaderboardModal;

        CreateLeaderboardTabButton(panel.transform, "Friends", -240f, 168f);
        CreateLeaderboardTabButton(panel.transform, "Team", 0f, 168f);
        CreateLeaderboardTabButton(panel.transform, "Global", 240f, 168f);

        GameObject board = CreateModalSectionPanel(panel.transform, "LeaderboardRows", 850, 312, 0f, -22f);
        Text boardTitleText = CreateModalSectionHeader(board.transform, GetLeaderboardTitle(), gold, 760, 0f, 124f, 24, 32f);
        Text rankHeaderText = CreateAnchoredText(board.transform, "Rank", 14, FontStyle.Bold, muted, 80, 22, -352f, 86f);
        Text playerHeaderText = CreateAnchoredText(board.transform, "Player", 14, FontStyle.Bold, muted, 250, 22, -200f, 86f);
        Text covenHeaderText = CreateAnchoredText(board.transform, "Coven", 14, FontStyle.Bold, muted, 190, 22, 60f, 86f);
        Text weeklyScoreHeaderText = CreateAnchoredText(board.transform, "Weekly Score", 14, FontStyle.Bold, muted, 160, 22, 298f, 86f);
        LeaderboardBoardView boardView = board.GetComponent<LeaderboardBoardView>();
        if (boardView == null)
        {
            boardView = board.AddComponent<LeaderboardBoardView>();
        }

        boardView.ResetRuntimeBindings();
        boardView.Initialize(boardTitleText, rankHeaderText, playerHeaderText, covenHeaderText, weeklyScoreHeaderText);
        boardView.Apply(new LeaderboardBoardDisplayModel(
            GetLeaderboardTitle(),
            "Rank",
            "Player",
            "Coven",
            "Weekly Score"));

        string[,] rows = GetPrototypeLeaderboardRows();
        for (int index = 0; index < rows.GetLength(0); index++)
        {
            CreateLeaderboardRow(board.transform, index + 1, rows[index, 0], rows[index, 1], rows[index, 2], 48f - index * 48f);
        }

        GameObject scope = CreateAnchoredPanel(panel.transform, "LeaderboardScope", new Color(0.13f, 0.08f, 0.15f), 850, 76, 0f, -220f);
        Text scopeNoteText = CreateAnchoredText(scope.transform, "Beta note: this is a UI shell for friends, Coven/team, and global views. Final metrics, reset cadence, rewards, ties, moderation, and backend syncing remain open.", 14, FontStyle.Bold, muted, 790, 54, 0f, 0f);
        LeaderboardScopeNoteView scopeView = scope.GetComponent<LeaderboardScopeNoteView>();
        if (scopeView == null)
        {
            scopeView = scope.AddComponent<LeaderboardScopeNoteView>();
        }

        scopeView.ResetRuntimeBindings();
        scopeView.Initialize(scopeNoteText);
        scopeView.Apply(new LeaderboardScopeNoteDisplayModel("Beta note: this is a UI shell for friends, Coven/team, and global views. Final metrics, reset cadence, rewards, ties, moderation, and backend syncing remain open."));
    }

    private void CreateLeaderboardTabButton(Transform parent, string tab, float x, float y)
    {
        bool active = prototypeLeaderboardTab == tab;
        Button button = CreateAnchoredButton(parent, tab, 18, 200, 42, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        UITabButtonBinder binder = button.GetComponent<UITabButtonBinder>();
        if (binder == null)
        {
            binder = button.gameObject.AddComponent<UITabButtonBinder>();
        }

        binder.SetSelected(active);
        LeaderboardTabButtonView tabView = button.GetComponent<LeaderboardTabButtonView>();
        if (tabView == null)
        {
            tabView = button.gameObject.AddComponent<LeaderboardTabButtonView>();
        }

        tabView.ResetRuntimeBindings();
        tabView.Initialize(binder, tab);
        tabView.SetSelected(active);
        tabView.TabSelected += HandleLeaderboardTabSelected;
    }

    private void HandleLeaderboardTabSelected(string tab)
    {
        prototypeLeaderboardTab = tab;
        BuildLeaderboardUi();
    }

    private string GetLeaderboardTitle()
    {
        if (prototypeLeaderboardTab == "Team")
        {
            return "Coven / Team Standings";
        }

        if (prototypeLeaderboardTab == "Global")
        {
            return "Global Standings";
        }

        return "Friends Standings";
    }

    private string[,] GetPrototypeLeaderboardRows()
    {
        if (prototypeLeaderboardTab == "Team")
        {
            return new string[,]
            {
                { "Moonpetal Circle", "Your Coven", "18,420" },
                { "Starlit Hearth", "Coven", "17,960" },
                { "Azalea Coven", "Coven", "16,780" },
                { "Silver Dew Circle", "Coven", "15,230" },
                { "Dawnroot Guild", "Coven", "14,910" }
            };
        }

        if (prototypeLeaderboardTab == "Global")
        {
            return new string[,]
            {
                { "Celeste", "Astral Bloom", "42,300" },
                { "Marisol", "Moonpetal Circle", "39,880" },
                { "You", "Moonpetal Circle", "36,240" },
                { "Rowan", "Starlit Hearth", "35,910" },
                { "Luna", "Moonpetal Circle", "34,700" }
            };
        }

        return new string[,]
        {
            { "You", "Moonpetal Circle", "36,240" },
            { "Luna", "Moonpetal Circle", "34,700" },
            { "Mira", "Moonpetal Circle", "31,980" },
            { "Eldric", "Moonpetal Circle", "29,450" },
            { "Juniper", "Pending", "24,120" }
        };
    }

    private void CreateLeaderboardRow(Transform parent, int rank, string playerName, string covenName, string score, float y)
    {
        bool isPlayer = playerName == "You";
        Color rowColor = isPlayer ? new Color(0.96f, 0.86f, 0.62f) : new Color(0.18f, 0.13f, 0.2f);
        Color textColor = isPlayer ? new Color(0.18f, 0.08f, 0.32f) : Color.white;
        GameObject row = CreateAnchoredPanel(parent, $"LeaderboardRow_{rank}", rowColor, 780, 40, 0f, y);
        Text rankText = CreateAnchoredText(row.transform, $"#{rank}", 16, FontStyle.Bold, textColor, 78, 22, -350f, 0f);
        Text playerNameText = CreateAnchoredText(row.transform, playerName, 16, FontStyle.Bold, textColor, 238, 22, -200f, 0f);
        Text covenNameText = CreateAnchoredText(row.transform, covenName, 14, FontStyle.Bold, textColor, 190, 22, 60f, 0f);
        Text scoreText = CreateAnchoredText(row.transform, score, 16, FontStyle.Bold, textColor, 150, 22, 298f, 0f);
        LeaderboardRowView rowView = row.GetComponent<LeaderboardRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<LeaderboardRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(rankText, playerNameText, covenNameText, scoreText);
        rowView.Apply(new LeaderboardRowDisplayModel($"#{rank}", playerName, covenName, score));
    }

    private void RemoveLeaderboardModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("LeaderboardModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }
}
