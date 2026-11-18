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

// Social Freebies screen, freebie deep links and code redemption.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildSocialFreebiesUi()
    {
        RemoveInboxModal();
        RemoveDailyBonusModal();
        RemoveDailySpinModal();
        RemoveSocialFreebiesModal();
        GameObject panel = CreateModalShell("SocialFreebiesModal", 860, 430, new Color(0.07f, 0.035f, 0.12f, 0.985f));
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        Color violet = new Color(0.35f, 0.12f, 0.62f);

        UIModalChromeView chromeView = CreateModalChrome(
            panel,
            "SOCIAL FREEBIES",
            "Follow our social feeds. Freebie posts link back into the game and redeem rewards automatically.",
            "Back to Den",
            gold,
            muted,
            violet,
            720f,
            156f,
            720f,
            48f,
            106f,
            16,
            220f,
            48f,
            0f,
            -158f,
            20);
        chromeView.CloseRequested += BuildPlayerDenUi;

        Button instagram = CreateAnchoredButton(panel.transform, "Instagram", 18, 190, 48, new Color(0.55f, 0.18f, 0.78f), -230f, 28f);
        instagram.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeInstagramUrl, "Instagram"));
        Button facebook = CreateAnchoredButton(panel.transform, "Facebook", 18, 190, 48, new Color(0.14f, 0.25f, 0.48f), 0f, 28f);
        facebook.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeFacebookUrl, "Facebook"));
        Button x = CreateAnchoredButton(panel.transform, "X", 18, 190, 48, new Color(0.1f, 0.1f, 0.12f), 230f, 28f);
        x.onClick.AddListener(() => OpenSocialFreebiesUrl(PrototypeXUrl, "X"));

        Button simulate = CreateAnchoredButton(panel.transform, "Simulate Freebie Link", 18, 260, 42, new Color(0.12f, 0.55f, 0.08f), 0f, -58f);
        simulate.onClick.AddListener(() => HandlePrototypeDeepLink(BuildPrototypeSocialFreebieAppUrl()));
        SocialFreebiesActionsView actionsView = panel.GetComponent<SocialFreebiesActionsView>();
        if (actionsView == null)
        {
            actionsView = panel.AddComponent<SocialFreebiesActionsView>();
        }

        actionsView.ResetRuntimeBindings();
        actionsView.Initialize(instagram, facebook, x, simulate);
        actionsView.Apply(new SocialFreebiesActionsDisplayModel("Instagram", "Facebook", "X", "Simulate Freebie Link"));

        const string socialFreebiesNote = "Prototype social URLs are placeholders. Freebie reward values are placeholders, not final economy tuning.";
        Text socialFreebiesNoteText = CreateAnchoredText(panel.transform, socialFreebiesNote, 14, FontStyle.Bold, muted, 700, 44, 0f, -112f);
        SocialFreebiesFooterNoteView footerNoteView = panel.GetComponent<SocialFreebiesFooterNoteView>();
        if (footerNoteView == null)
        {
            footerNoteView = panel.AddComponent<SocialFreebiesFooterNoteView>();
        }

        footerNoteView.ResetRuntimeBindings();
        footerNoteView.Initialize(socialFreebiesNoteText);
        footerNoteView.Apply(new SocialFreebiesFooterNoteDisplayModel(socialFreebiesNote));
    }

    private void OpenSocialFreebiesUrl(string url, string label)
    {
        Application.OpenURL(url);
        coven.AddPrototypeChatLine($"Opened {label} freebies feed.");
    }

    private void HandlePrototypeDeepLink(string url)
    {
        if (!TryExtractPrototypeFreebieCode(url, out string code, out System.DateTime expiresAtUtc))
        {
            return;
        }

        RedeemPrototypeSocialFreebieCode(code, expiresAtUtc);
    }

    private bool TryExtractPrototypeFreebieCode(string url, out string code, out System.DateTime expiresAtUtc)
    {
        code = "";
        expiresAtUtc = System.DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        string lower = url.ToLowerInvariant();
        if (!lower.Contains("freebie") || !lower.Contains("code="))
        {
            return false;
        }

        int codeStart = lower.IndexOf("code=", System.StringComparison.Ordinal) + 5;
        if (codeStart < 5 || codeStart >= url.Length)
        {
            return false;
        }

        int codeEnd = url.IndexOf('&', codeStart);
        code = codeEnd >= 0 ? url.Substring(codeStart, codeEnd - codeStart) : url.Substring(codeStart);
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        string expirationText = ExtractQueryValue(url, "exp");
        if (string.IsNullOrWhiteSpace(expirationText))
        {
            expirationText = ExtractQueryValue(url, "expires");
        }

        if (!TryParseSocialFreebieExpiration(expirationText, out expiresAtUtc))
        {
            expiresAtUtc = System.DateTime.MinValue;
        }

        return true;
    }

    private void RedeemPrototypeSocialFreebieCode(string code, System.DateTime expiresAtUtc)
    {
        PrototypeRewardGrant reward = BuildPrototypeSocialFreebieReward();
        bool redeemed = inventory.TryRedeemSocialFreebieCode(
            code,
            reward,
            expiresAtUtc,
            out string message);
        if (redeemed)
        {
            TrackPrototypeAnalytics(
                PrototypeAnalyticsEvents.SocialFreebieRedeemed,
                PrototypeAnalyticsPayloadFactory.CreateSocialFreebieRedeemedJson(reward, code));
        }
        coven.AddPrototypeChatLine(message);
        RefreshPowerUpDisplays();

        if (contentRoot != null)
        {
            if (redeemed)
            {
                BuildPlayerDenUi();
            }
            else
            {
                BuildSocialFreebiesUi();
            }
        }
    }

    private PrototypeRewardGrant BuildPrototypeSocialFreebieReward()
    {
        return new PrototypeRewardGrant(
            "Social Freebie",
            250,
            5,
            0,
            0,
            new List<PrototypeRewardItem> { new PrototypeRewardItem("Single Sigil", 1) });
    }

    private string BuildPrototypeSocialFreebieAppUrl()
    {
        string expiration = System.DateTime.UtcNow.Date.AddDays(SocialFreebieExpirationDays).ToString("yyyyMMdd");
        return $"bingomagicmayhem://freebie?code={PrototypeSocialFreebieCode}&exp={expiration}";
    }

    private static string ExtractQueryValue(string url, string key)
    {
        string search = key + "=";
        int start = url.ToLowerInvariant().IndexOf(search, System.StringComparison.Ordinal);
        if (start < 0)
        {
            return "";
        }

        start += search.Length;
        if (start >= url.Length)
        {
            return "";
        }

        int end = url.IndexOf('&', start);
        return end >= 0 ? url.Substring(start, end - start) : url.Substring(start);
    }

    private static bool TryParseSocialFreebieExpiration(string value, out System.DateTime expiresAtUtc)
    {
        expiresAtUtc = System.DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (System.DateTime.TryParseExact(
            value.Trim(),
            "yyyyMMdd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
            out System.DateTime parsed))
        {
            expiresAtUtc = parsed.Date.AddDays(1);
            return true;
        }

        return false;
    }

    private void RemoveSocialFreebiesModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("SocialFreebiesModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }
    }
}
