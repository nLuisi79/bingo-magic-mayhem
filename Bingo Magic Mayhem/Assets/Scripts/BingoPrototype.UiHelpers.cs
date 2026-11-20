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

// Shared UGUI building blocks: modal chrome, stage preparation and safe-area scaling, text/panel/button/image factories.
// Split out of BingoPrototype.cs to keep that file manageable.
public partial class BingoPrototype
{
    private GameObject CreateModalShell(string name, float width, float height, Color color)
    {
        return CreateAnchoredPanel(contentRoot, name, color, width, height, 0f, -18f);
    }

    private Text CreateModalTitle(Transform parent, string text, Color color, float width = 820f, float y = 274f)
    {
        return CreateAnchoredText(parent, text, 44, FontStyle.Bold, color, width, 58, 0f, y);
    }

    private Text CreateModalSubtitle(Transform parent, string text, Color color, float width = 980f, float height = 34f, float y = 232f, int fontSize = 15)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, 0f, y);
    }

    private Text CreateModalSectionHeader(Transform parent, string text, Color color, float width, float x, float y, int fontSize = 23, float height = 32f)
    {
        return CreateAnchoredText(parent, text, fontSize, FontStyle.Bold, color, width, height, x, y);
    }

    private UIModalChromeView CreateModalChrome(
        GameObject panel,
        string title,
        string subtitle,
        string closeLabel,
        Color titleColor,
        Color subtitleColor,
        Color closeButtonColor,
        float titleWidth = 820f,
        float titleY = 274f,
        float subtitleWidth = 980f,
        float subtitleHeight = 34f,
        float subtitleY = 232f,
        int subtitleFontSize = 15,
        float closeWidth = 220f,
        float closeHeight = 48f,
        float closeX = 0f,
        float closeY = -286f,
        int closeFontSize = 20)
    {
        Text titleText = CreateModalTitle(panel.transform, title, titleColor, titleWidth, titleY);
        Text subtitleText = CreateModalSubtitle(panel.transform, subtitle, subtitleColor, subtitleWidth, subtitleHeight, subtitleY, subtitleFontSize);
        Button closeButton = CreateAnchoredButton(panel.transform, closeLabel, closeFontSize, closeWidth, closeHeight, closeButtonColor, closeX, closeY);
        Text closeButtonText = closeButton.GetComponentInChildren<Text>();

        UIModalController modalController = panel.GetComponent<UIModalController>();
        if (modalController == null)
        {
            modalController = panel.AddComponent<UIModalController>();
        }

        closeButton.onClick.RemoveListener(modalController.RequestClose);
        closeButton.onClick.AddListener(modalController.RequestClose);

        UIModalChromeView chromeView = panel.GetComponent<UIModalChromeView>();
        if (chromeView == null)
        {
            chromeView = panel.AddComponent<UIModalChromeView>();
        }

        chromeView.ResetRuntimeBindings();
        chromeView.Initialize(titleText, subtitleText, closeButton, closeButtonText, modalController);
        chromeView.Apply(title, subtitle, closeLabel);
        return chromeView;
    }

    private GameObject CreateModalSectionPanel(Transform parent, string name, float width, float height, float x, float y, Color? colorOverride = null)
    {
        return CreateAnchoredPanel(parent, name, colorOverride ?? new Color(0.1f, 0.045f, 0.14f), width, height, x, y);
    }

    private GameObject CreateModalInfoCard(Transform parent, string name, float width, float height, float x, float y, Color? colorOverride = null)
    {
        return CreateAnchoredPanel(parent, name, colorOverride ?? new Color(0.96f, 0.86f, 0.62f), width, height, x, y);
    }

    private VerticalLayoutGroup PrepareStage(float width, float height, Color stageColor, int padding, int spacing)
    {
        activeStageWidth = width;
        activeStageHeight = height;
        activeStageSafePadding = SafePadding;
        activeStageFillsViewport = false;
        lastRootSize = Vector2.zero;
        stageAmbientBackdrop = null;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (LayoutGroup group in GetComponents<LayoutGroup>())
        {
            group.enabled = false;
            Destroy(group);
        }

        Image background = GetComponent<Image>();
        if (background != null)
        {
            background.color = new Color(0.08f, 0.09f, 0.12f, 1f);
        }

        RectTransform rootRect = EnsureRootRectTransform();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject stage = new GameObject("Stage");
        stage.transform.SetParent(transform, false);

        stageRect = stage.AddComponent<RectTransform>();
        stageRect.anchorMin = new Vector2(0.5f, 0.5f);
        stageRect.anchorMax = new Vector2(0.5f, 0.5f);
        stageRect.pivot = new Vector2(0.5f, 0.5f);
        stageRect.anchoredPosition = Vector2.zero;
        stageRect.sizeDelta = new Vector2(width, height);

        Image stageImage = stage.AddComponent<Image>();
        stageImage.color = stageColor;

        contentRoot = stage.transform;

        VerticalLayoutGroup layout = stage.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(padding, padding, padding, padding);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        return layout;
    }

    private void ApplyStageScale(bool force = false)
    {
        if (stageRect == null)
        {
            return;
        }

        RectTransform rootRect = EnsureRootRectTransform();
        Vector2 rootSize = rootRect.rect.size;
        Rect safeArea = Screen.safeArea;
        if (!force && rootSize == lastRootSize && safeArea == lastSafeArea)
        {
            return;
        }

        lastRootSize = rootSize;
        lastSafeArea = safeArea;

        // Keep every screen inside the device safe area (notches, rounded corners, home indicator).
        // Full-bleed screens fit the whole stage so artwork-anchored UI (room medallions) stays visible and aligned;
        // the ambient backdrop covers any leftover bands. Other screens keep the original size limits.
        bool fillsViewport = activeStageFillsViewport || IsRealmHomeStage();
        StageFit.Result fit = StageFit.Compute(
            rootSize,
            new Vector2(Screen.width, Screen.height),
            safeArea,
            new Vector2(activeStageWidth, activeStageHeight),
            activeStageSafePadding,
            fillsViewport);

        stageRect.localScale = new Vector3(fit.Scale, fit.Scale, 1f);
        stageRect.anchoredPosition = fit.Offset;

        if (stageAmbientBackdrop != null)
        {
            float coverScale = Mathf.Max(rootSize.x / activeStageWidth, rootSize.y / activeStageHeight);
            stageAmbientBackdrop.sizeDelta = new Vector2(activeStageWidth, activeStageHeight) * coverScale;
        }
    }

    private bool IsRealmHomeStage()
    {
        if (stageRect == null)
        {
            return false;
        }

        for (int index = 0; index < stageRect.childCount; index++)
        {
            if (stageRect.GetChild(index).name.StartsWith(RealmHomeBackgroundObjectPrefix))
            {
                return true;
            }
        }

        return false;
    }

    private Text CreateText(string label, int size, FontStyle style, Color color, float height)
    {
        return CreateText(label, size, style, color, height, 560);
    }

    private Text CreateText(string label, int size, FontStyle style, Color color, float height, float width)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(contentRoot, false);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;

        return text;
    }

    private GameObject CreatePanel(string name, Color color, float height)
    {
        return CreatePanel(name, color, height, BoardWidth);
    }

    private GameObject CreatePanel(string name, Color color, float height, float width)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(contentRoot, false);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;
        layoutElement.flexibleWidth = 0;

        return panel;
    }

    private GameObject CreatePanelInParent(Transform parent, string name, Color color, float width, float height)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = panel.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;
        layoutElement.flexibleWidth = 0;

        return panel;
    }

    private GameObject CreateAnchoredLayer(Transform parent, string name, float width, float height)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent, false);

        RectTransform rect = layer.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = Vector2.zero;

        return layer;
    }

    private GameObject CreateAnchoredPanel(Transform parent, string name, Color color, float width, float height, float x, float y)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = panel.AddComponent<Image>();
        image.color = color;

        return panel;
    }

    private Text CreateAnchoredText(Transform parent, string label, int size, FontStyle style, Color color, float width, float height, float x, float y)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        return text;
    }

    private InputField CreateAnchoredInputField(Transform parent, string name, string placeholder, float width, float height, float x, float y)
    {
        GameObject inputObject = CreateAnchoredPanel(parent, name, new Color(0.96f, 0.86f, 0.62f), width, height, x, y);
        InputField input = inputObject.AddComponent<InputField>();
        input.characterLimit = 160;
        input.lineType = InputField.LineType.MultiLineSubmit;
        input.caretColor = new Color(0.18f, 0.08f, 0.32f);
        input.selectionColor = new Color(0.35f, 0.12f, 0.62f, 0.35f);

        Text messageText = CreateAnchoredText(inputObject.transform, "", 17, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), width - 28f, height - 22f, 0f, 0f);
        messageText.alignment = TextAnchor.UpperLeft;
        messageText.verticalOverflow = VerticalWrapMode.Truncate;

        Text placeholderText = CreateAnchoredText(inputObject.transform, placeholder, 16, FontStyle.Italic, new Color(0.36f, 0.28f, 0.42f, 0.72f), width - 28f, height - 22f, 0f, 0f);
        placeholderText.alignment = TextAnchor.UpperLeft;
        placeholderText.verticalOverflow = VerticalWrapMode.Truncate;

        input.textComponent = messageText;
        input.placeholder = placeholderText;
        input.targetGraphic = inputObject.GetComponent<Image>();

        return input;
    }

    private Image CreateAnchoredImage(Transform parent, string resourceName, float width, float height, float x, float y)
    {
        GameObject imageObject = new GameObject(resourceName);
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x, y);

        Image image = imageObject.AddComponent<Image>();
        Texture2D texture = Resources.Load<Texture2D>(resourceName);
        if (texture != null)
        {
            image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.preserveAspect = true;
            image.color = Color.white;
        }
        else
        {
            image.color = new Color(0.15f, 0.3f, 0.8f);
        }

        return image;
    }

    private void CreateInfoTile(Transform parent, string title, string detail, float width, Color color)
    {
        GameObject tile = new GameObject(title);
        tile.transform.SetParent(parent, false);

        Image image = tile.AddComponent<Image>();
        image.color = color;

        LayoutElement layoutElement = tile.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = 132;
        layoutElement.flexibleWidth = 0;

        VerticalLayoutGroup layout = tile.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 6;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(tile.transform, title, 20, FontStyle.Bold, Color.white, width - 24, 36);
        CreateTextInParent(tile.transform, detail, 19, FontStyle.Bold, Color.white, width - 24, 70);
    }

    private void CreateCardOption(Transform parent, int cardCount, string ingredientText, string chanceText)
    {
        int entryCost = cardCount * manaBetPerCard;
        bool canAfford = inventory.CanAffordMana(entryCost);
        Color fill = !canAfford
            ? new Color(0.5f, 0.48f, 0.44f)
            : cardCount == selectedCardCount ? new Color(1f, 0.86f, 0.46f) : new Color(0.96f, 0.88f, 0.68f);
        GameObject tile = new GameObject($"{cardCount} Card Option");
        tile.transform.SetParent(parent, false);

        Image image = tile.AddComponent<Image>();
        image.color = fill;

        LayoutElement layoutElement = tile.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 250;
        layoutElement.preferredHeight = 205;
        layoutElement.flexibleWidth = 0;

        VerticalLayoutGroup layout = tile.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 10, 10);
        layout.spacing = 7;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateTextInParent(tile.transform, $"{cardCount} {(cardCount == 1 ? "Card" : "Cards")}", 25, FontStyle.Bold, new Color(0.18f, 0.08f, 0.32f), 220, 30);
        CreateTextInParent(tile.transform, ingredientText, 18, FontStyle.Bold, new Color(0.14f, 0.08f, 0.24f), 220, 30);
        CreateTextInParent(tile.transform, chanceText, 15, FontStyle.Bold, new Color(0.14f, 0.08f, 0.24f), 220, 38);

        Button play = CreateButton(tile.transform, canAfford ? $"Play {entryCost} Mana" : $"Need {entryCost} Mana", 20, 48, 210, canAfford ? new Color(0.12f, 0.55f, 0.08f) : new Color(0.36f, 0.36f, 0.36f));
        play.onClick.AddListener(() => BeginRoundFromLobby(cardCount));
    }

    private Text CreateTextInParent(Transform parent, string label, int size, FontStyle style, Color color, float width, float height)
    {
        GameObject textObject = new GameObject(label);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;
        layoutElement.flexibleWidth = 0;

        return text;
    }

    private void CreateHeaderCell(Transform parent, string letter)
    {
        GameObject headerObject = new GameObject($"Header_{letter}");
        headerObject.transform.SetParent(parent, false);
        headerObject.AddComponent<RectTransform>();

        Text text = CreateChildText(headerObject.transform);
        text.text = letter;
        text.fontSize = 28;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(1f, 0.87f, 0.32f);
    }

    private Button CreateCell(Transform parent, int row, int column)
    {
        GameObject cellObject = new GameObject($"Cell_{row}_{column}");
        cellObject.transform.SetParent(parent, false);

        Image image = cellObject.AddComponent<Image>();
        image.color = marked[row, column] ? new Color(0.19f, 0.78f, 0.45f) : new Color(0.95f, 0.97f, 1f);

        Button button = cellObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => ToggleCell(row, column));

        Text label = CreateChildText(cellObject.transform);
        label.text = row == 2 && column == 2 ? "FREE" : numbers[row, column].ToString();
        label.color = marked[row, column] ? Color.white : new Color(0.08f, 0.1f, 0.16f);
        label.fontSize = row == 2 && column == 2 ? 18 : 26;
        label.fontStyle = FontStyle.Bold;

        return button;
    }

    private Text CreateChildText(Transform parent)
    {
        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text text = textObject.AddComponent<Text>();
        text.font = uiFont;
        text.alignment = TextAnchor.MiddleCenter;

        return text;
    }

    private Button CreateButton(Transform parent, string label, int fontSize, float height, float width, Color color)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.preferredWidth = width;
        layoutElement.flexibleWidth = 0;

        if (!string.IsNullOrEmpty(label))
        {
            Text text = CreateChildText(buttonObject.transform);
            text.text = label;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }

        return button;
    }
}
