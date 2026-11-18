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

// Mana Cauldron screen and Den button/status text. Cauldron amounts and refill timing live in PlayerInventoryState;
// this part of BingoPrototype only builds the UI. Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildManaCauldronUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        StopRoundAndClearCalledHistory();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.055f, 0.025f, 0.08f, 0.98f), 0, 0);
        layout.enabled = false;

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color cream = new Color(0.97f, 0.89f, 0.68f);

        CreateAnchoredPanel(contentRoot, "CauldronDimDen", new Color(0.09f, 0.05f, 0.12f), 1500, 820, 0f, -12f);
        CreateAnchoredText(contentRoot, "PLAYER'S DEN", 26, FontStyle.Bold, new Color(0.72f, 0.64f, 0.82f), 400, 40, 0f, 386f);
        CreateLobbyResourceTile(contentRoot, "Mana", profile.Mana.ToString("N0"), "*", 230, 438f, 382f);
        CreateLobbyResourceTile(contentRoot, "Crystals", profile.Crystals.ToString("N0"), "<>", 230, 690f, 382f);

        GameObject modal = CreateAnchoredPanel(contentRoot, "ManaCauldronModal", new Color(0.12f, 0.04f, 0.18f, 0.98f), 1040, 570, 0f, -12f);
        Text headerText = CreateAnchoredText(modal.transform, string.Empty, 54, FontStyle.Bold, new Color(1f, 0.94f, 0.72f), 940, 70, 0f, 226f);
        CreateAnchoredPanel(modal.transform, "CauldronBowl", new Color(0.22f, 0.08f, 0.28f), 350, 230, -250f, 38f);
        // Three lines at size 54 measure 184 units in the built-in font; a shorter box silently drops the last line.
        CreateAnchoredText(modal.transform, "*\n*\n*", 54, FontStyle.Bold, gold, 240, 190, -250f, 38f);
        cauldronAmountText = CreateAnchoredText(modal.transform, string.Empty, 72, FontStyle.Bold, Color.white, 370, 90, 248f, 72f);
        Text resourceLabelText = CreateAnchoredText(modal.transform, string.Empty, 38, FontStyle.Bold, gold, 360, 54, 248f, 8f);
        cauldronStatusText = CreateAnchoredText(modal.transform, string.Empty, 25, FontStyle.Bold, cream, 360, 96, 248f, -76f);
        Text refillNoteText = CreateAnchoredText(modal.transform, string.Empty, 18, FontStyle.Bold, cream, 780, 42, 0f, -228f);

        cauldronCollectButton = CreateAnchoredButton(modal.transform, string.Empty, 28, 330, 58, profile.ManaCauldronAmount > 0 ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.34f, 0.32f, 0.38f), 218f, -154f);
        cauldronCollectButton.interactable = profile.ManaCauldronAmount > 0;

        Button back = CreateAnchoredButton(contentRoot, "Back to Den", 20, 220, 50, new Color(0.18f, 0.16f, 0.22f), -620f, -382f);
        ManaCauldronModalView view = modal.GetComponent<ManaCauldronModalView>();
        if (view == null)
        {
            view = modal.AddComponent<ManaCauldronModalView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(headerText, cauldronAmountText, resourceLabelText, cauldronStatusText, refillNoteText, cauldronCollectButton, back);
        view.CollectRequested += CollectManaCauldronFromDen;
        view.BackRequested += BuildPlayerDenUi;
        view.Apply(BuildManaCauldronDisplayModel(profile));

        ApplyStageScale(true);
    }

    private ManaCauldronModalDisplayModel BuildManaCauldronDisplayModel(PlayerProfileState profile)
    {
        return new ManaCauldronModalDisplayModel
        {
            HeaderText = "MANA CAULDRON",
            AmountText = $"{profile.ManaCauldronAmount} / {profile.ManaCauldronCapacity}",
            ResourceLabelText = "Mana Coins",
            StatusText = GetManaCauldronDetailStatusText(),
            RefillNoteText = $"Room restorations increase the hourly refill. Current refill: +{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour.",
            CollectButtonText = profile.ManaCauldronAmount > 0 ? $"Collect {profile.ManaCauldronAmount}" : "Empty",
            CollectButtonInteractable = profile.ManaCauldronAmount > 0,
            BackButtonText = "Back to Den"
        };
    }

    private string GetDenManaCauldronButtonText()
    {
        return $"MANA CAULDRON\n{inventory.ManaCauldronAmount} / {inventory.ManaCauldronCapacity}\n+{inventory.GetManaCauldronHourlyRefillAmount()}/hr | {GetManaCauldronShortTimerText()}";
    }

    private string GetManaCauldronDetailStatusText()
    {
        if (inventory.ManaCauldronAmount >= inventory.ManaCauldronCapacity)
        {
            return $"Full - ready to collect\n+{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour";
        }

        string collectLine = inventory.ManaCauldronAmount > 0 ? "Collect available now" : "Still brewing...";
        return $"{collectLine}\n+{inventory.GetManaCauldronHourlyRefillAmount()} mana/hour\n{inventory.GetManaCauldronCountdownText()}";
    }

    private string GetManaCauldronShortTimerText()
    {
        return inventory.ManaCauldronAmount >= inventory.ManaCauldronCapacity
            ? "Full"
            : $"Next {inventory.GetManaCauldronRemainingTimeText()}";
    }

    private void CollectManaCauldronFromDen()
    {
        inventory.CollectManaCauldron();
        BuildPlayerDenUi();
    }
}
