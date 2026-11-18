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

// Jackpot wheel spin screen, its panels and result text. Wheel weights come from the economy tuning config.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildJackpotSpinUi()
    {
        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        roundFlow.StopRound();

        VerticalLayoutGroup layout = PrepareStage(StageWidth, StageHeight, new Color(0.08f, 0.02f, 0.14f, 0.98f), 0, 0);
        layout.enabled = false;

        CreateAnchoredPanel(contentRoot, "JackpotHeader", new Color(0.09f, 0.04f, 0.15f), 1440, 84, 0f, 370f);
        Text titleText = CreateAnchoredText(contentRoot, string.Empty, 46, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 900, 62, 110f, 382f);
        Text subtitleText = CreateAnchoredText(contentRoot, string.Empty, 20, FontStyle.Bold, Color.white, 900, 30, 110f, 338f);
        CreateLobbyResourceTile(contentRoot, "Mana", inventory.GetManaText(), "*", 220, -570f, 370f);

        CreateAnchoredText(contentRoot, "v", 46, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 80, 56, -390f, 298f);
        GameObject wheel = CreateAnchoredPanel(contentRoot, "JackpotWheel", new Color(0.2f, 0.05f, 0.32f), 560, 560, -390f, 38f);
        BuildJackpotWheelSegments(wheel.transform);
        bool hasStackToCollect = jackpotWheelFlow.HasStackToCollect;
        bool canSpinWheel = jackpotWheelFlow.CanSpin(inventory.PendingJackpotSpins);
        bool canBigCollect = jackpotWheelFlow.CanCollect(inventory.PendingJackpotSpins);
        bool canUseCenterButton = canSpinWheel || canBigCollect;
        string centerButtonLabel = jackpotWheelFlow.IsAnimating ? "SPINNING" : canSpinWheel ? "SPIN" : "COLLECT";
        Color centerButtonColor = canBigCollect
            ? new Color(0.86f, 0.55f, 0.06f)
            : canSpinWheel ? new Color(0.08f, 0.48f, 0.12f) : new Color(0.35f, 0.35f, 0.35f);
        Button centerSpin = CreateAnchoredButton(wheel.transform, centerButtonLabel, jackpotWheelFlow.IsAnimating ? 22 : 34, 178, 178, centerButtonColor, 0f, 0f);
        centerSpin.interactable = canUseCenterButton;

        BuildJackpotPotPanel();
        Text roomNameText = CreateAnchoredText(contentRoot, string.Empty, 28, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 620, 42, 350f, 274f);
        string spinStatus = jackpotWheelFlow.IsAnimating ? "Wheel spinning..." : canBigCollect ? "Ready to collect" : $"Spins Left: {inventory.PendingJackpotSpins}";
        Text spinStatusText = CreateAnchoredText(contentRoot, string.Empty, 24, FontStyle.Bold, canBigCollect ? new Color(1f, 0.9f, 0.32f) : new Color(0.8f, 1f, 0.55f), 420, 38, -390f, -238f);
        BuildJackpotCollectedStackPanel();
        BuildJackpotCollectionConfirmationPanel();

        string result = string.IsNullOrEmpty(jackpotSpinResultText)
            ? "Spin each wheelspin, stack the rewards, then collect once."
            : jackpotSpinResultText;
        Text resultText = CreateAnchoredText(contentRoot, string.Empty, 26, FontStyle.Bold, Color.white, 720, 58, 350f, -184f);
        BuildJackpotResultPanel();

        string collectLabel = canBigCollect ? "Collect" : canSpinWheel && hasStackToCollect ? "Keep Spinning" : "Return Home";
        Button collect = CreateAnchoredButton(contentRoot, collectLabel, 22, 220, 56, canBigCollect ? new Color(0.86f, 0.55f, 0.06f) : new Color(0.35f, 0.12f, 0.62f), 350f, -332f);
        collect.interactable = !jackpotWheelFlow.IsAnimating && (!hasStackToCollect || canBigCollect);
        JackpotSpinScreenView screenView = contentRoot.gameObject.GetComponent<JackpotSpinScreenView>();
        if (screenView == null)
        {
            screenView = contentRoot.gameObject.AddComponent<JackpotSpinScreenView>();
        }

        screenView.ResetRuntimeBindings();
        screenView.Initialize(titleText, subtitleText, roomNameText, spinStatusText, resultText, centerSpin, collect);
        screenView.CenterRequested += () =>
        {
            if (inventory.PendingJackpotSpins <= 0 && jackpotWheelFlow.HasStackToCollect)
            {
                BigCollectJackpotWheelStack();
                return;
            }

            SpinJackpotWheel();
        };
        screenView.CollectRequested += () =>
        {
            if (inventory.PendingJackpotSpins <= 0 && jackpotWheelFlow.HasStackToCollect)
            {
                BigCollectJackpotWheelStack();
                return;
            }

            ReturnFromJackpotWheel();
        };
        screenView.Apply(new JackpotSpinScreenDisplayModel
        {
            TitleText = "Jackpot Wheelspin Earned!",
            SubtitleText = "Your bingo win unlocked a chance at the Grand Jackpot.",
            RoomNameText = RealmContentCatalog.ActivePrototypeRoom.Name,
            SpinStatusText = spinStatus,
            ResultText = result,
            CenterButtonText = centerButtonLabel,
            CollectButtonText = collectLabel,
            CanUseCenterButton = canUseCenterButton,
            CanUseCollectButton = !jackpotWheelFlow.IsAnimating && (!hasStackToCollect || canBigCollect)
        });

        ApplyStageScale(true);
    }

    private void SpinJackpotWheel()
    {
        if (jackpotWheelFlow.IsAnimating)
        {
            return;
        }

        if (inventory.PendingJackpotSpins <= 0)
        {
            jackpotSpinResultText = "No jackpot spins are available.";
            BuildJackpotSpinUi();
            return;
        }

        jackpotWheelFlow.ClearLatestSpinResult();

        if (!inventory.TryConsumeJackpotSpin())
        {
            jackpotSpinResultText = "No jackpot spins are available.";
            BuildJackpotSpinUi();
            return;
        }

        int targetSegment = JackpotWheelRules.GetSegmentForRoll(Random.Range(0, 100));
        JackpotWheelSpinResult pendingResult = ResolveJackpotSpinReward(targetSegment);
        if (!jackpotWheelFlow.TryBeginSpin(inventory.PendingJackpotSpins + 1, targetSegment, pendingResult))
        {
            jackpotSpinResultText = "No jackpot spins are available.";
            BuildJackpotSpinUi();
            return;
        }

        int animationSequence = jackpotWheelFlow.AnimationSequence;
        jackpotSpinResultText = "Wheel spinning...";
        BuildJackpotSpinUi();
        StartCoroutine(AnimateJackpotWheelSpin(animationSequence));
    }

    private IEnumerator AnimateJackpotWheelSpin(int animationSequence)
    {
        float duration = 1.7f;
        float elapsed = 0f;
        float startRotation = jackpotWheelFlow.Rotation;
        float targetRotation = startRotation + 1080f + JackpotWheelRules.GetDeltaToTarget(startRotation, jackpotWheelFlow.TargetSegment);

        while (elapsed < duration)
        {
            elapsed += 0.035f;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            jackpotWheelFlow.Rotation = Mathf.Lerp(startRotation, targetRotation, eased);
            UpdateJackpotWheelSegmentTransforms();
            yield return new WaitForSeconds(0.035f);
        }

        if (animationSequence != jackpotWheelFlow.AnimationSequence)
        {
            yield break;
        }

        jackpotWheelFlow.Rotation = targetRotation;
        jackpotWheelFlow.CompleteSpin();
        JackpotWheelSpinResult lastResult = jackpotWheelFlow.LastSpinResult;
        if (lastResult != null)
        {
            jackpotSpinResultText = inventory.PendingJackpotSpins > 0
                ? $"{lastResult.Label} stacked +{lastResult.ManaAward:N0}. Spin {inventory.PendingJackpotSpins} more, then collect."
                : $"All wheelspins stacked. Collect +{jackpotWheelFlow.CollectedMana:N0}.";
        }

        BuildJackpotSpinUi();
    }

    private JackpotWheelSpinResult ResolveJackpotSpinReward(int segmentIndex)
    {
        return JackpotWheelRules.ResolveSpinResult(
            segmentIndex,
            inventory.GetWheelStandardValue,
            inventory.GetJackpotValue(),
            inventory.GetEpicValue(),
            inventory.GetLegendaryValue());
    }

    private void BigCollectJackpotWheelStack()
    {
        JackpotWheelCollectResult collectResult = jackpotWheelFlow.Collect();
        if (collectResult == null)
        {
            ReturnFromJackpotWheel();
            return;
        }

        if (collectResult.ResetPot)
        {
            inventory.GrantJackpotSpecialMana(collectResult.CollectedMana);
        }
        else
        {
            inventory.GrantJackpotMana(collectResult.CollectedMana);
        }

        TrackPrototypeAnalytics(
            PrototypeAnalyticsEvents.JackpotCollected,
            PrototypeAnalyticsPayloadFactory.CreateJackpotCollectedJson(
                collectResult.CollectedMana,
                collectResult.SpinResultCount,
                collectResult.ResetPot,
                inventory.PendingJackpotSpins));
        jackpotSpinResultText = $"Collected +{collectResult.CollectedMana:N0} to profile mana.";
        BuildJackpotSpinUi();
    }

    private void ReturnFromJackpotWheel()
    {
        jackpotSpinResultText = "";
        jackpotWheelFlow.ResetForReturnHome();
        BuildLobbyUi();
    }

    private void BuildJackpotWheelSegments(Transform wheel)
    {
        jackpotWheelSegmentRects.Clear();
        Color[] colors =
        {
            new Color(0.07f, 0.2f, 0.72f),
            new Color(0.78f, 0.15f, 0.58f),
            new Color(0.94f, 0.52f, 0.05f),
            new Color(0.06f, 0.58f, 0.18f),
            new Color(0.08f, 0.55f, 0.8f),
            new Color(0.96f, 0.8f, 0.08f)
        };

        int segmentCount = JackpotWheelRules.GetSegmentCount();
        float angleStep = 360f / segmentCount;
        for (int index = 0; index < segmentCount; index++)
        {
            float angle = jackpotWheelFlow.Rotation + index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians) * 190f;
            float y = Mathf.Cos(radians) * 190f;
            bool special = index >= 8;
            GameObject segment = CreateAnchoredPanel(wheel, $"WheelSegment_{index}", special ? GetJackpotSpecialColor(index) : colors[index % colors.Length], special ? 150 : 136, 82, x, y);
            jackpotWheelSegmentRects.Add(segment.GetComponent<RectTransform>());
            segment.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
            string label = GetJackpotWheelSegmentLabel(index);
            CreateAnchoredText(segment.transform, label, special ? 21 : 26, FontStyle.Bold, Color.white, special ? 138 : 118, 46, 0f, special ? 0f : 8f);
            if (!special)
            {
                CreateAnchoredText(segment.transform, "*", 18, FontStyle.Bold, new Color(1f, 0.86f, 0.16f), 44, 26, 0f, -22f);
            }
        }
    }

    private void UpdateJackpotWheelSegmentTransforms()
    {
        int segmentCount = Mathf.Min(JackpotWheelRules.GetSegmentCount(), jackpotWheelSegmentRects.Count);
        if (segmentCount == 0)
        {
            return;
        }

        float angleStep = 360f / JackpotWheelRules.GetSegmentCount();
        for (int index = 0; index < segmentCount; index++)
        {
            RectTransform segment = jackpotWheelSegmentRects[index];
            if (segment == null)
            {
                continue;
            }

            float angle = jackpotWheelFlow.Rotation + index * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            segment.anchoredPosition = new Vector2(Mathf.Sin(radians) * 190f, Mathf.Cos(radians) * 190f);
            segment.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }

    private void BuildJackpotPotPanel()
    {
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotPotPanel", new Color(0.15f, 0.06f, 0.22f), 520, 390, 350f, 38f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 25, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 460, 40, 0f, 142f);
        Text potValueText = CreateAnchoredText(panel.transform, string.Empty, 70, FontStyle.Bold, Color.white, 340, 82, -26f, 82f);
        Text accentText = CreateAnchoredText(panel.transform, string.Empty, 52, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 70, 70, 168f, 82f);
        JackpotPotPanelView view = panel.GetComponent<JackpotPotPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<JackpotPotPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, potValueText, accentText);
        view.Apply(new JackpotPotPanelDisplayModel
        {
            TitleText = "CURRENT POT",
            PotValueText = inventory.GetCurrentJackpotPotText(),
            AccentText = "*"
        });
        CreateJackpotTierRow(panel.transform, "JACKPOT", inventory.GetJackpotValue(), -2f, new Color(0.58f, 0.18f, 0.85f));
        CreateJackpotTierRow(panel.transform, "EPIC", inventory.GetEpicValue(), -84f, new Color(0.72f, 0.12f, 0.78f));
        CreateJackpotTierRow(panel.transform, "LEGENDARY", inventory.GetLegendaryValue(), -166f, new Color(0.92f, 0.62f, 0.06f));
    }

    private void BuildJackpotResultPanel()
    {
        JackpotWheelSpinResult lastResult = jackpotWheelFlow.LastSpinResult;
        if (lastResult == null)
        {
            return;
        }

        Color accent = GetJackpotResultAccent(lastResult);
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotResultPanel", new Color(0.1f, 0.04f, 0.16f), 520, 132, 350f, -252f);
        Text eyebrowText = CreateAnchoredText(panel.transform, string.Empty, 15, FontStyle.Bold, new Color(0.82f, 0.9f, 1f), 460, 24, 0f, 48f);
        Text labelText = CreateAnchoredText(panel.transform, string.Empty, 30, FontStyle.Bold, accent, 240, 40, -116f, 14f);
        Text amountText = CreateAnchoredText(panel.transform, string.Empty, 38, FontStyle.Bold, Color.white, 170, 46, 96f, 14f);
        Text accentText = CreateAnchoredText(panel.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 190f, 14f);
        Text descriptionText = CreateAnchoredText(panel.transform, string.Empty, 17, FontStyle.Bold, new Color(0.82f, 0.9f, 1f), 460, 30, 0f, -40f);
        JackpotResultPanelView view = panel.GetComponent<JackpotResultPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<JackpotResultPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(eyebrowText, labelText, amountText, accentText, descriptionText);
        view.Apply(new JackpotResultPanelDisplayModel
        {
            EyebrowText = GetJackpotResultEyebrow(lastResult),
            LabelText = lastResult.Label,
            AmountText = $"+{lastResult.ManaAward:N0}",
            AccentText = "*",
            DescriptionText = GetJackpotResultDescription(lastResult)
        });
    }

    private void BuildJackpotCollectionConfirmationPanel()
    {
        if (!jackpotWheelFlow.CollectionConfirmed || jackpotWheelFlow.LastCollectedMana <= 0)
        {
            return;
        }

        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotCollectConfirmation", new Color(0.08f, 0.24f, 0.1f), 520, 122, 350f, -252f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 18, FontStyle.Bold, new Color(0.8f, 1f, 0.55f), 460, 28, 0f, 38f);
        Text amountText = CreateAnchoredText(panel.transform, string.Empty, 42, FontStyle.Bold, Color.white, 180, 52, -68f, 2f);
        Text accentText = CreateAnchoredText(panel.transform, string.Empty, 32, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 44f, 2f);
        Text descriptionText = CreateAnchoredText(panel.transform, string.Empty, 17, FontStyle.Bold, new Color(0.84f, 0.92f, 1f), 470, 30, 0f, -42f);
        JackpotCollectionConfirmationPanelView view = panel.GetComponent<JackpotCollectionConfirmationPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<JackpotCollectionConfirmationPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, amountText, accentText, descriptionText);
        view.Apply(new JackpotCollectionConfirmationPanelDisplayModel
        {
            TitleText = "COLLECTED",
            AmountText = $"+{jackpotWheelFlow.LastCollectedMana:N0}",
            AccentText = "*",
            DescriptionText = jackpotWheelFlow.LastCollectResetPot
                ? "Special tier collected. Pot reset to room minimum."
                : "Standard rewards collected. Pot carries forward."
        });
    }

    private void BuildJackpotCollectedStackPanel()
    {
        bool readyToCollect = jackpotWheelFlow.CanCollect(inventory.PendingJackpotSpins);
        GameObject panel = CreateAnchoredPanel(contentRoot, "JackpotCollectedStack", readyToCollect ? new Color(0.2f, 0.1f, 0.03f) : new Color(0.12f, 0.04f, 0.18f), 500, 136, -390f, -360f);
        Text titleText = CreateAnchoredText(panel.transform, string.Empty, 18, FontStyle.Bold, new Color(1f, 0.9f, 0.32f), 460, 26, 0f, 50f);
        Text amountText = CreateAnchoredText(panel.transform, string.Empty, 38, FontStyle.Bold, Color.white, 180, 48, -142f, 12f);
        Text accentText = CreateAnchoredText(panel.transform, string.Empty, 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 40, 40, -36f, 12f);
        Text stackText = CreateAnchoredText(panel.transform, string.Empty, 15, FontStyle.Bold, new Color(0.86f, 0.9f, 1f), 240, 86, 116f, -8f);
        JackpotCollectedStackPanelView view = panel.GetComponent<JackpotCollectedStackPanelView>();
        if (view == null)
        {
            view = panel.AddComponent<JackpotCollectedStackPanelView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(titleText, amountText, accentText, stackText);
        view.Apply(new JackpotCollectedStackPanelDisplayModel
        {
            TitleText = readyToCollect ? "REWARD STACK READY" : "WHEEL REWARD STACK",
            AmountText = $"+{jackpotWheelFlow.CollectedMana:N0}",
            AccentText = "*",
            StackText = GetJackpotCollectedStackText()
        });
    }

    private string GetJackpotCollectedStackText()
    {
        IReadOnlyList<JackpotWheelSpinResult> collectedResults = jackpotWheelFlow.CollectedResults;
        if (collectedResults.Count == 0)
        {
            return "Spin to build the reward stack.";
        }

        int firstVisible = Mathf.Max(0, collectedResults.Count - 5);
        string text = "";
        if (firstVisible > 0)
        {
            text = $"+{firstVisible} earlier";
        }

        for (int index = firstVisible; index < collectedResults.Count; index++)
        {
            JackpotWheelSpinResult result = collectedResults[index];
            if (text.Length > 0)
            {
                text += "\n";
            }

            text += $"{index + 1}. {result.Label} +{result.ManaAward:N0}";
        }

        return text;
    }

    private string GetJackpotResultEyebrow(JackpotWheelSpinResult result)
    {
        if (result == null)
        {
            return "LATEST SPIN";
        }

        if (result.Label == "LEGENDARY")
        {
            return "LEGENDARY HIT";
        }

        if (result.Label == "EPIC")
        {
            return "EPIC HIT";
        }

        if (result.Label == "JACKPOT")
        {
            return "JACKPOT HIT";
        }

        return "LATEST SPIN";
    }

    private string GetJackpotResultDescription(JackpotWheelSpinResult result)
    {
        if (result == null)
        {
            return "";
        }

        if (result.Label == "LEGENDARY")
        {
            return "Legendary tier: 3x pot and pot resets on collect";
        }

        if (result.Label == "EPIC")
        {
            return "Epic tier: 2x pot and pot resets on collect";
        }

        if (result.Label == "JACKPOT")
        {
            return "Jackpot tier: 1x pot and pot resets on collect";
        }

        return "Standard tier: pot carries forward";
    }

    private Color GetJackpotResultAccent(JackpotWheelSpinResult result)
    {
        if (result == null)
        {
            return Color.white;
        }

        if (result.Label == "LEGENDARY")
        {
            return new Color(1f, 0.74f, 0.12f);
        }

        if (result.Label == "EPIC")
        {
            return new Color(0.94f, 0.28f, 1f);
        }

        if (result.Label == "JACKPOT")
        {
            return new Color(1f, 0.9f, 0.32f);
        }

        return Color.white;
    }

    private void CreateJackpotTierRow(Transform parent, string label, int value, float y, Color color)
    {
        GameObject row = CreateAnchoredPanel(parent, label, new Color(color.r * 0.35f, color.g * 0.25f, color.b * 0.45f, 1f), 440, 58, 0f, y);
        CreateAnchoredText(row.transform, label, 24, FontStyle.Bold, color, 190, 42, -104f, 0f);
        CreateAnchoredText(row.transform, value.ToString("N0"), 33, FontStyle.Bold, Color.white, 150, 44, 82f, 0f);
        CreateAnchoredText(row.transform, "*", 30, FontStyle.Bold, new Color(1f, 0.82f, 0.18f), 42, 42, 180f, 0f);
    }

    private string GetJackpotWheelSegmentLabel(int segmentIndex)
    {
        return JackpotWheelRules.GetSegmentLabel(segmentIndex, inventory.GetWheelStandardValue);
    }

    private Color GetJackpotSpecialColor(int segmentIndex)
    {
        if (segmentIndex == 8)
        {
            return new Color(0.48f, 0.06f, 0.56f);
        }

        if (segmentIndex == 9)
        {
            return new Color(0.66f, 0.08f, 0.82f);
        }

        return new Color(0.96f, 0.62f, 0.04f);
    }
}
