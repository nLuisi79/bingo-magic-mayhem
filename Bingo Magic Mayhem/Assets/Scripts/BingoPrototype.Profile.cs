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

// Player profile modal: summary, avatar/cosmetics and settings tabs, and profile persistence.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private void BuildPlayerProfileUi(string tab)
    {
        playerProfileActiveTab = string.IsNullOrWhiteSpace(tab) ? PlayerProfileTabIds.Profile : tab;
        RemovePlayerProfileModal();

        PlayerProfileState profile = PlayerProfileState.FromPrototype(inventory, rewards);
        GameObject panel = CreateAnchoredPanel(contentRoot, "PlayerProfileModal", new Color(0.055f, 0.025f, 0.095f, 0.985f), 1080, 660, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        PlayerProfileModalDisplayModel modalDisplayModel = BuildPlayerProfileModalDisplayModel(profile);
        UIModalChromeView modalChromeView = CreateModalChrome(
            panel,
            modalDisplayModel.Title,
            modalDisplayModel.Subtitle,
            modalDisplayModel.CloseButtonText,
            gold,
            muted,
            new Color(0.35f, 0.12f, 0.62f),
            820f,
            274f,
            820f,
            28f,
            232f,
            18,
            220f,
            48f,
            0f,
            -286f,
            20);

        PlayerProfileTabButtonView profileTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Profile, -220f, 184f);
        PlayerProfileTabButtonView avatarsTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Avatars, 0f, 184f);
        PlayerProfileTabButtonView settingsTabView = CreatePlayerProfileTabButton(panel.transform, PlayerProfileTabIds.Settings, 220f, 184f);

        BuildPlayerProfileTabContent(panel.transform, profile);

        playerProfileModalView = panel.AddComponent<PlayerProfileModalView>();
        playerProfileModalView.ResetRuntimeBindings();
        playerProfileModalView.Initialize(modalChromeView);
        playerProfileModalView.RegisterTab(profileTabView);
        playerProfileModalView.RegisterTab(avatarsTabView);
        playerProfileModalView.RegisterTab(settingsTabView);
        playerProfileModalView.SetActiveTab(playerProfileActiveTab);
        playerProfileModalView.Apply(modalDisplayModel);
        playerProfileModalView.CloseRequested += RemovePlayerProfileModal;
        playerProfileModalView.TabRequested += BuildPlayerProfileUi;
    }

    private PlayerProfileTabButtonView CreatePlayerProfileTabButton(Transform parent, string tab, float x, float y)
    {
        bool active = playerProfileActiveTab == tab;
        Button button = CreateAnchoredButton(parent, tab, 18, 190, 42, active ? new Color(0.55f, 0.18f, 0.78f) : new Color(0.18f, 0.16f, 0.28f), x, y);
        UITabButtonBinder binder = button.gameObject.AddComponent<UITabButtonBinder>();
        binder.SetSelected(active);
        PlayerProfileTabButtonView tabView = button.gameObject.AddComponent<PlayerProfileTabButtonView>();
        tabView.ResetRuntimeBindings();
        tabView.Initialize(binder, tab);
        return tabView;
    }

    private void BuildPlayerProfileTabContent(Transform parent, PlayerProfileState profile)
    {
        if (playerProfileActiveTab == PlayerProfileTabIds.Avatars)
        {
            BuildPlayerAvatarTab(parent, profile);
            return;
        }

        if (playerProfileActiveTab == PlayerProfileTabIds.Settings)
        {
            BuildPlayerSettingsTab(parent);
            return;
        }

        BuildPlayerProfileSummaryTab(parent, profile);
    }

    private void BuildPlayerProfileSummaryTab(Transform parent, PlayerProfileState profile)
    {
        Color cream = new Color(0.96f, 0.86f, 0.62f);
        Color purple = new Color(0.18f, 0.08f, 0.32f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);

        GameObject identity = CreateProfileSectionPanel(parent, "ProfileIdentity", cream, 320, 334, -330f, -26f);
        GameObject summaryAvatar = CreateAnchoredPanel(identity.transform, "AvatarFrame", GetPrototypeFrameColor(), 150, 150, 0f, 70f);
        if (!TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 126f, 126f))
        {
            CreateAnchoredText(summaryAvatar.transform, GetPrototypeAvatarIcon(), 46, FontStyle.Bold, Color.white, 130, 80, 0f, 14f);
        }
        TryCreateCosmeticSprite(summaryAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 150f, 150f);
        CreateAnchoredText(identity.transform, "Player Name", 16, FontStyle.Bold, purple, 260, 24, 0f, -8f);
        Text displayNameText = CreateAnchoredText(identity.transform, profileSettingsState.DisplayName, 30, FontStyle.Bold, purple, 280, 40, 0f, -42f);
        Text levelAndRankText = CreateAnchoredText(identity.transform, $"Level {profile.Level}\nRank: Aura TBD", 18, FontStyle.Bold, new Color(0.04f, 0.32f, 0.1f), 260, 58, 0f, -102f);

        GameObject progress = CreateProfileSectionPanel(parent, "ProfileProgress", new Color(0.12f, 0.06f, 0.18f), 640, 334, 190f, -26f);
        CreateProfileSectionTitle(progress.transform, "ACCOUNT SUMMARY", 24, gold, 560f, 132f);
        Text manaValueText = CreateProfileSummaryRow(progress.transform, "Mana", profile.Mana.ToString("N0"), -188f, 82f);
        Text crystalsValueText = CreateProfileSummaryRow(progress.transform, "Crystals", profile.Crystals.ToString("N0"), 188f, 82f);
        Text albumValueText = CreateProfileSummaryRow(progress.transform, "Album", $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} Grimoire", -188f, 18f);
        Text bookValueText = CreateProfileSummaryRow(progress.transform, "Book", inventory.BookOfShadowsPurchased ? $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked", 188f, 18f);
        Text roomsValueText = CreateProfileSummaryRow(progress.transform, "Rooms", $"{inventory.GetRestoredRoomCount()} restored", -188f, -46f);
        Text auraValueText = CreateProfileSummaryRow(progress.transform, "Aura", "Strength formula TBD", 188f, -46f);
        Text auraRankNoteText = CreateAnchoredText(progress.transform, profile.AuraRankNoteText, 15, FontStyle.Bold, muted, 560, 42, 0f, -116f);

        Button rank = CreateAnchoredButton(parent, "Aura Ranks", 18, 210, 42, new Color(0.35f, 0.12f, 0.62f), -330f, -218f);
        PlayerProfileSummaryTabView summaryView = parent.gameObject.AddComponent<PlayerProfileSummaryTabView>();
        summaryView.ResetRuntimeBindings();
        summaryView.Initialize(
            displayNameText,
            levelAndRankText,
            auraRankNoteText,
            manaValueText,
            crystalsValueText,
            albumValueText,
            bookValueText,
            roomsValueText,
            auraValueText,
            rank);
        summaryView.AuraRanksRequested += BuildRankInfoUi;
        summaryView.Apply(BuildPlayerProfileSummaryDisplayModel(profile));
    }

    private Text CreateProfileSummaryRow(Transform parent, string label, string value, float x, float y)
    {
        GameObject row = CreateModalInfoCard(parent, $"ProfileSummary_{label}", 286, 48, x, y);
        Text labelText = CreateAnchoredText(row.transform, label, 14, FontStyle.Bold, new Color(0.35f, 0.12f, 0.62f), 250, 18, 0f, 10f);
        Text valueText = CreateAnchoredText(row.transform, value, 16, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 250, 22, 0f, -10f);
        PlayerProfileSummaryRowView rowView = row.GetComponent<PlayerProfileSummaryRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<PlayerProfileSummaryRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, valueText);
        rowView.Apply(label, value);
        return rowView.ValueText;
    }

    private void BuildPlayerAvatarTab(Transform parent, PlayerProfileState profile)
    {
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        GameObject preview = CreateProfileSectionPanel(parent, "AvatarPreview", new Color(0.12f, 0.06f, 0.18f), 360, 360, -300f, -30f);
        GameObject previewAvatar = CreateAnchoredPanel(preview.transform, "AvatarFramePreview", GetPrototypeFrameColor(), 190, 190, 0f, 58f);
        if (!TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].PrimaryAssetKey, "AvatarSprite", 160f, 160f))
        {
            CreateAnchoredText(previewAvatar.transform, GetPrototypeAvatarIcon(), 58, FontStyle.Bold, Color.white, 160, 100, 0f, 16f);
        }
        TryCreateCosmeticSprite(previewAvatar.transform, PrototypeAvatarFrames[selectedPrototypeFrameIndex].PrimaryAssetKey, "FrameSprite", 190f, 190f);
        Text avatarNameText = CreateAnchoredText(preview.transform, PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, 25, FontStyle.Bold, gold, 300, 34, 0f, -54f);
        Text cosmeticsSummaryText = CreateAnchoredText(preview.transform, $"{PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName}\n{PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName} dauber", 17, FontStyle.Bold, muted, 300, 58, 0f, -106f);

        GameObject controls = CreateProfileSectionPanel(parent, "AvatarControls", new Color(0.96f, 0.86f, 0.62f), 560, 360, 190f, -30f);
        CreateProfileSectionTitle(controls.transform, "COSMETICS", 26, new Color(0.18f, 0.08f, 0.32f), 480f, 132f);
        (Text avatarValueText, Button avatarNextButton) = CreateCosmeticSelectorRow(controls.transform, "Avatar", PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName, -6f, 70f, CyclePrototypeAvatar);
        (Text frameValueText, Button frameNextButton) = CreateCosmeticSelectorRow(controls.transform, "Frame", PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName, -6f, 6f, CyclePrototypeFrame);
        (Text dauberValueText, Button dauberNextButton) = CreateCosmeticSelectorRow(controls.transform, "Dauber", PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName, -6f, -58f, CyclePrototypeDauber);
        Text footnoteText = CreateAnchoredText(controls.transform, "Avatar unlock rules, rank cosmetics, and custom avatar behavior are placeholders only.", 14, FontStyle.Bold, new Color(0.34f, 0.26f, 0.34f), 480, 42, 0f, -128f);

        PlayerAvatarTabView avatarTabView = parent.gameObject.AddComponent<PlayerAvatarTabView>();
        avatarTabView.ResetRuntimeBindings();
        avatarTabView.Initialize(
            avatarNameText,
            cosmeticsSummaryText,
            avatarValueText,
            frameValueText,
            dauberValueText,
            footnoteText,
            avatarNextButton,
            frameNextButton,
            dauberNextButton);
        avatarTabView.AvatarNextRequested += CyclePrototypeAvatar;
        avatarTabView.FrameNextRequested += CyclePrototypeFrame;
        avatarTabView.DauberNextRequested += CyclePrototypeDauber;
        avatarTabView.Apply(BuildPlayerAvatarTabDisplayModel());
    }

    private (Text valueText, Button nextButton) CreateCosmeticSelectorRow(Transform parent, string label, string value, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateProfileOptionRowPanel(parent, $"Cosmetic_{label}", 480f, 48f, x, y);
        Text labelText = CreateProfileOptionRowLabel(row.transform, label, 16, new Color(1f, 0.9f, 0.32f), 118, 22, -164f);
        Text valueText = CreateProfileOptionRowValue(row.transform, value, 16, Color.white, 210, 24, -18f);
        Button next = CreateAnchoredButton(row.transform, "Next", 13, 82, 28, new Color(0.35f, 0.12f, 0.62f), 184f, 0f);
        ProfileSelectorRowView rowView = row.GetComponent<ProfileSelectorRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<ProfileSelectorRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, valueText, next);
        rowView.Apply(new ProfileSelectorRowDisplayModel
        {
            LabelText = label,
            ValueText = value,
            ActionButtonText = "Next"
        });
        rowView.ActionRequested += () => action?.Invoke();
        return (valueText, next);
    }

    private bool TryCreateCosmeticSprite(Transform parent, string assetKey, string objectName, float width, float height)
    {
        Sprite sprite = CosmeticSpriteResolver.Load(assetKey);
        if (sprite == null)
        {
            return false;
        }

        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;

        Image image = imageObject.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return true;
    }

    private void BuildPlayerSettingsTab(Transform parent)
    {
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.84f, 0.82f, 0.94f);
        GameObject account = CreateProfileSectionPanel(parent, "ProfileAccountSettings", new Color(0.12f, 0.06f, 0.18f), 460, 360, -250f, -30f);
        CreateProfileSectionHeader(account.transform, "ACCOUNT", 25, gold, 380f, 132f);
        profileDisplayNameInput = CreateAnchoredInputField(account.transform, "ProfileDisplayNameInput", "Display name", 360, 44, 0f, 84f);
        profileDisplayNameInput.text = profileSettingsState.DisplayName;
        profileDisplayNameInput.characterLimit = ProfileDisplayNameValidator.BetaMaximumLength;
        profileDisplayNameInput.lineType = InputField.LineType.SingleLine;
        profileDisplayNameInput.textComponent.alignment = TextAnchor.MiddleLeft;
        Button saveName = CreateAnchoredButton(account.transform, "Save Display Name", 16, 220, 38, new Color(0.12f, 0.55f, 0.08f), 0f, 38f);
        profileDisplayNameStatusText = CreateAnchoredText(account.transform, lastProfileDisplayNameStatus, 12, FontStyle.Bold, muted, 380, 34, 0f, -2f);
        CreateProfileSectionHeader(account.transform, "LOGIN PLACEHOLDERS", 14, gold, 380f, -42f, 22f);
        CreateCompactLoginOptionButton(account.transform, "Google", -125f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Facebook", 0f, -78f);
        CreateCompactLoginOptionButton(account.transform, "Apple", 125f, -78f);
        CreateProfileSectionNote(account.transform, "Name validation is local Beta/test only. Login, uniqueness, and moderation require backend approval.", 12, muted, 390, 48, 0f, -132f);

        GameObject preferences = CreateProfileSectionPanel(parent, "ProfilePreferences", new Color(0.96f, 0.86f, 0.62f), 460, 360, 250f, -30f);
        CreateProfileSectionHeader(preferences.transform, "PREFERENCES", 25, new Color(0.18f, 0.08f, 0.32f), 380f, 132f);
        (Button soundToggleButton, Text soundToggleText) = CreatePreferenceToggle(preferences.transform, "Sound", prototypeSoundEnabled, 58f, TogglePrototypeSound);
        (Button notificationsToggleButton, Text notificationsToggleText) = CreatePreferenceToggle(preferences.transform, "Notifications", prototypeNotificationsEnabled, -8f, TogglePrototypeNotifications);
        CreateProfileSectionNote(preferences.transform, "Cosmetics and preferences save locally. Name editing and platform permissions wire up later.", 14, new Color(0.34f, 0.26f, 0.34f), 380, 58, 0f, -112f);

        PlayerSettingsTabView settingsTabView = parent.gameObject.AddComponent<PlayerSettingsTabView>();
        settingsTabView.ResetRuntimeBindings();
        settingsTabView.Initialize(
            profileDisplayNameInput,
            profileDisplayNameStatusText,
            saveName,
            soundToggleButton,
            soundToggleText,
            notificationsToggleButton,
            notificationsToggleText);
        settingsTabView.SaveNameRequested += SavePrototypeDisplayName;
        settingsTabView.SoundToggleRequested += TogglePrototypeSound;
        settingsTabView.NotificationsToggleRequested += TogglePrototypeNotifications;
        settingsTabView.Apply(BuildPlayerSettingsTabDisplayModel());
    }

    private void CreateCompactLoginOptionButton(Transform parent, string provider, float x, float y)
    {
        Button button = CreateAnchoredButton(parent, provider, 13, 110, 32, new Color(0.18f, 0.16f, 0.28f), x, y);
        ProfileLoginOptionView view = button.gameObject.GetComponent<ProfileLoginOptionView>();
        if (view == null)
        {
            view = button.gameObject.AddComponent<ProfileLoginOptionView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(button);
        view.Apply(new ProfileLoginOptionDisplayModel
        {
            ProviderName = provider,
            IsAvailable = false
        });
    }

    private GameObject CreateProfileSectionPanel(Transform parent, string name, Color color, float width, float height, float x, float y)
    {
        return CreateAnchoredPanel(parent, name, color, width, height, x, y);
    }

    private Text CreateProfileSectionTitle(Transform parent, string text, int fontSize, Color color, float width, float y, float height = 34f)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, (int)width, (int)height, 0f, y);
    }

    private Text CreateProfileSectionHeader(Transform parent, string text, int fontSize, Color color, float width, float y, float height = 34f)
    {
        Text title = CreateProfileSectionTitle(parent, string.Empty, fontSize, color, width, y, height);
        ProfileSectionHeaderView view = title.gameObject.GetComponent<ProfileSectionHeaderView>();
        if (view == null)
        {
            view = title.gameObject.AddComponent<ProfileSectionHeaderView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(title);
        view.Apply(new ProfileSectionHeaderDisplayModel
        {
            TitleText = text
        });
        return title;
    }

    private Text CreateProfileSectionNote(Transform parent, string text, int fontSize, Color color, int width, int height, float x, float y)
    {
        Text note = CreateAnchoredText(parent, string.Empty, fontSize, FontStyle.Bold, color, width, height, x, y);
        ProfileSectionNoteView view = note.gameObject.GetComponent<ProfileSectionNoteView>();
        if (view == null)
        {
            view = note.gameObject.AddComponent<ProfileSectionNoteView>();
        }

        view.ResetRuntimeBindings();
        view.Initialize(note);
        view.Apply(new ProfileSectionNoteDisplayModel
        {
            NoteText = text
        });
        return note;
    }

    private void SavePrototypeDisplayName()
    {
        DisplayNameValidationResult result = ProfileDisplayNameValidator.ValidateBeta(profileDisplayNameInput?.text);
        lastProfileDisplayNameStatus = result.Message;
        if (profileDisplayNameStatusText != null)
        {
            profileDisplayNameStatusText.text = result.Message;
            profileDisplayNameStatusText.color = result.IsValid
                ? new Color(0.55f, 1f, 0.55f)
                : new Color(1f, 0.58f, 0.58f);
        }

        if (!result.IsValid)
        {
            return;
        }

        profileSettingsState.DisplayName = result.NormalizedName;
        if (profileDisplayNameInput != null)
        {
            profileDisplayNameInput.text = result.NormalizedName;
        }

        PersistProfileSettings("display_name_changed");
    }

    private (Button button, Text valueText) CreatePreferenceToggle(Transform parent, string label, bool enabled, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject row = CreateProfileOptionRowPanel(parent, $"Preference_{label}", 360f, 50f, 0f, y);
        Text labelText = CreateProfileOptionRowLabel(row.transform, label, 18, Color.white, 170, 24, -72f);
        Button toggle = CreateAnchoredButton(row.transform, enabled ? "ON" : "OFF", 16, 92, 30, enabled ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.42f, 0.12f, 0.18f), 112f, 0f);
        ProfileToggleRowView rowView = row.GetComponent<ProfileToggleRowView>();
        if (rowView == null)
        {
            rowView = row.AddComponent<ProfileToggleRowView>();
        }

        rowView.ResetRuntimeBindings();
        rowView.Initialize(labelText, toggle);
        rowView.Apply(new ProfileToggleRowDisplayModel
        {
            LabelText = label,
            ToggleText = enabled ? "ON" : "OFF"
        });
        rowView.ToggleRequested += () => action?.Invoke();
        Text valueText = toggle.GetComponentInChildren<Text>();
        return (toggle, valueText);
    }

    private GameObject CreateProfileOptionRowPanel(Transform parent, string name, float width, float height, float x, float y)
    {
        return CreateAnchoredPanel(parent, name, new Color(0.18f, 0.13f, 0.2f), width, height, x, y);
    }

    private Text CreateProfileOptionRowLabel(Transform parent, string text, int fontSize, Color color, int width, int height, float x)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, 0f);
    }

    private Text CreateProfileOptionRowValue(Transform parent, string text, int fontSize, Color color, int width, int height, float x)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, 0f);
    }

    private Button CreateDenReturnFooter(Transform parent, float x, float y, UnityEngine.Events.UnityAction action)
    {
        Button close = CreateAnchoredButton(parent, "Back to Den", 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), x, y);
        UIFooterActionView footerView = close.gameObject.GetComponent<UIFooterActionView>();
        if (footerView == null)
        {
            footerView = close.gameObject.AddComponent<UIFooterActionView>();
        }

        footerView.ResetRuntimeBindings();
        footerView.Initialize(close);
        if (action != null)
        {
            footerView.Requested += () => action();
        }

        footerView.Apply(new UIFooterActionDisplayModel
        {
            ButtonText = "Back to Den",
            Interactable = close.interactable
        });

        return close;
    }

    private void CreateDualFooterNavigation(
        Transform parent,
        string backLabel,
        float y,
        UnityEngine.Events.UnityAction backAction,
        UnityEngine.Events.UnityAction closeAction,
        string closeLabel = "Back to Den")
    {
        Button back = CreateAnchoredButton(parent, backLabel, 20, 220, 48, new Color(0.35f, 0.12f, 0.62f), -130f, y);
        Button close = CreateAnchoredButton(parent, closeLabel, 20, 220, 48, new Color(0.18f, 0.16f, 0.22f), 130f, y);
        UIDualFooterNavigationView footerView = parent.gameObject.GetComponent<UIDualFooterNavigationView>();
        if (footerView == null)
        {
            footerView = parent.gameObject.AddComponent<UIDualFooterNavigationView>();
        }

        footerView.ResetRuntimeBindings();
        footerView.Initialize(back, close);
        if (backAction != null)
        {
            footerView.BackRequested += () => backAction();
        }

        if (closeAction != null)
        {
            footerView.CloseRequested += () => closeAction();
        }

        footerView.Apply(new UIDualFooterNavigationDisplayModel
        {
            BackButtonText = backLabel,
            BackButtonInteractable = back.interactable,
            CloseButtonText = closeLabel,
            CloseButtonInteractable = close.interactable
        });
    }

    private PlayerProfileSummaryDisplayModel BuildPlayerProfileSummaryDisplayModel(PlayerProfileState profile)
    {
        return new PlayerProfileSummaryDisplayModel(
            profileSettingsState.DisplayName,
            $"Level {profile.Level}\nRank: Aura TBD",
            profile.AuraRankNoteText,
            profile.Mana.ToString("N0"),
            profile.Crystals.ToString("N0"),
            $"{inventory.GetOwnedGrimoireCardCount()}/{CardAlbumCatalog.TotalCards} Grimoire",
            inventory.BookOfShadowsPurchased ? $"{inventory.GetOwnedBookOfShadowsCardCount()}/{CardAlbumCatalog.BookOfShadowsTotalCards}" : "Locked",
            $"{inventory.GetRestoredRoomCount()} restored",
            "Strength formula TBD");
    }

    private PlayerProfileModalDisplayModel BuildPlayerProfileModalDisplayModel(PlayerProfileState profile)
    {
        return new PlayerProfileModalDisplayModel(
            "PLAYER PROFILE",
            $"Level {profile.Level}  |  {profile.LevelProgressText}  |  {profile.AuraStrengthText}",
            "Back to Den");
    }

    private PlayerAvatarTabDisplayModel BuildPlayerAvatarTabDisplayModel()
    {
        return new PlayerAvatarTabDisplayModel(
            PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName,
            $"{PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName}\n{PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName} dauber",
            PrototypeAvatars[selectedPrototypeAvatarIndex].DisplayName,
            PrototypeAvatarFrames[selectedPrototypeFrameIndex].DisplayName,
            PrototypeDaubers[selectedPrototypeDauberIndex].DisplayName,
            "Avatar unlock rules, rank cosmetics, and custom avatar behavior are placeholders only.");
    }

    private PlayerSettingsTabDisplayModel BuildPlayerSettingsTabDisplayModel()
    {
        return new PlayerSettingsTabDisplayModel(
            profileSettingsState.DisplayName,
            lastProfileDisplayNameStatus,
            prototypeSoundEnabled,
            prototypeNotificationsEnabled);
    }

    private string GetPrototypeAvatarIcon()
    {
        if (selectedPrototypeAvatarIndex == 1) return "SUN";
        if (selectedPrototypeAvatarIndex == 2) return "SEER";
        if (selectedPrototypeAvatarIndex == 3) return "STAR";
        return "MOON";
    }

    private Color GetPrototypeFrameColor()
    {
        if (selectedPrototypeFrameIndex == 1) return new Color(0.35f, 0.12f, 0.62f);
        if (selectedPrototypeFrameIndex == 2) return new Color(0.12f, 0.42f, 0.18f);
        if (selectedPrototypeFrameIndex == 3) return new Color(0.12f, 0.25f, 0.52f);
        return new Color(0.78f, 0.48f, 0.06f);
    }

    private void CyclePrototypeAvatar()
    {
        selectedPrototypeAvatarIndex = (selectedPrototypeAvatarIndex + 1) % PrototypeAvatars.Count;
        PersistProfileSettings("avatar_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void CyclePrototypeFrame()
    {
        selectedPrototypeFrameIndex = (selectedPrototypeFrameIndex + 1) % PrototypeAvatarFrames.Count;
        PersistProfileSettings("frame_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void CyclePrototypeDauber()
    {
        selectedPrototypeDauberIndex = (selectedPrototypeDauberIndex + 1) % PrototypeDaubers.Count;
        PersistProfileSettings("dauber_selected");
        BuildPlayerProfileUi(PlayerProfileTabIds.Avatars);
    }

    private void TogglePrototypeSound()
    {
        prototypeSoundEnabled = !prototypeSoundEnabled;
        PersistProfileSettings("sound_preference_changed");
        BuildPlayerProfileUi(PlayerProfileTabIds.Settings);
    }

    private void TogglePrototypeNotifications()
    {
        prototypeNotificationsEnabled = !prototypeNotificationsEnabled;
        PersistProfileSettings("notification_preference_changed");
        BuildPlayerProfileUi(PlayerProfileTabIds.Settings);
    }

    private void ApplyProfileSettings(ProfileSettingsState state)
    {
        profileSettingsState = state ?? ProfileSettingsState.CreateDefault();
        selectedPrototypeAvatarIndex = GetCosmeticIndex(profileSettingsState.AvatarId, PrototypeAvatars);
        selectedPrototypeFrameIndex = GetCosmeticIndex(profileSettingsState.FrameId, PrototypeAvatarFrames);
        selectedPrototypeDauberIndex = GetCosmeticIndex(profileSettingsState.DauberId, PrototypeDaubers);
        prototypeSoundEnabled = profileSettingsState.SoundEnabled;
        prototypeNotificationsEnabled = profileSettingsState.NotificationsEnabled;
    }

    private void PersistProfileSettings(string actionType)
    {
        profileSettingsState = new ProfileSettingsState
        {
            DisplayName = profileSettingsState.DisplayName,
            AvatarId = PrototypeAvatars[Mathf.Clamp(selectedPrototypeAvatarIndex, 0, PrototypeAvatars.Count - 1)].Id,
            FrameId = PrototypeAvatarFrames[Mathf.Clamp(selectedPrototypeFrameIndex, 0, PrototypeAvatarFrames.Count - 1)].Id,
            DauberId = PrototypeDaubers[Mathf.Clamp(selectedPrototypeDauberIndex, 0, PrototypeDaubers.Count - 1)].Id,
            SoundEnabled = prototypeSoundEnabled,
            NotificationsEnabled = prototypeNotificationsEnabled
        };

        if (profileSettingsPersistence != null)
        {
            profileSettingsPersistence.Save(profileSettingsState, actionType);
        }
    }

    private static int GetCosmeticIndex(string id, IReadOnlyList<CosmeticDefinition> definitions)
    {
        for (int index = 0; index < definitions.Count; index++)
        {
            if (string.Equals(definitions[index].Id, id, System.StringComparison.Ordinal))
            {
                return index;
            }
        }

        return 0;
    }

    private void RemovePlayerProfileModal()
    {
        if (contentRoot == null)
        {
            return;
        }

        Transform existing = contentRoot.Find("PlayerProfileModal");
        if (existing != null)
        {
            Destroy(existing.gameObject);
        }

        playerProfileModalView = null;
    }
}
