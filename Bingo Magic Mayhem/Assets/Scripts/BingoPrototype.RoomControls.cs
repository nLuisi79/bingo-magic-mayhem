using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Prototype Settings > Room Controls: per-realm toggles that set each room Locked / Open / Complete so testers
// can move through the realms without replaying them. Test save only; rules live in RoomAccessRules.
public partial class BingoPrototype
{
    private int roomControlsRealmIndex = -1;

    private void BuildRoomControlsUi(bool returnToMap)
    {
        RemoveDevSettingsModal();
        IReadOnlyList<RealmDefinition> realms = RealmContentCatalog.AllRealms;
        if (roomControlsRealmIndex < 0 || roomControlsRealmIndex >= realms.Count)
        {
            roomControlsRealmIndex = RealmContentCatalog.ActivePrototypeRealmIndex;
        }

        roomControlsRealmIndex = Mathf.Clamp(roomControlsRealmIndex, 0, realms.Count - 1);
        int realmIndex = roomControlsRealmIndex;
        RealmDefinition realm = realms[realmIndex];

        // If the room the lobby would reopen is now locked, leave settings to the world map instead.
        bool safeReturnToMap = returnToMap || (!devSettingsReturnToDen && !IsRoomUnlocked(RealmContentCatalog.ActivePrototypeRoomIndex));

        GameObject panel = CreateAnchoredPanel(contentRoot, "DevSettingsModal", new Color(0.1f, 0.04f, 0.16f, 0.98f), 780, 600, 0f, -18f);
        Color gold = new Color(1f, 0.9f, 0.32f);
        Color muted = new Color(0.85f, 0.82f, 0.95f);
        CreateAnchoredText(panel.transform, "ROOM CONTROLS", 32, FontStyle.Bold, gold, 700, 44, 0f, 254f);
        CreateAnchoredText(panel.transform, "Tap a room's state to cycle Locked > Open > Complete. Changes this device's test save only.", 15, FontStyle.Bold, muted, 700, 40, 0f, 214f);

        Button previousRealm = CreateAnchoredButton(panel.transform, "<", 22, 58, 46, new Color(0.35f, 0.12f, 0.62f), -330f, 162f);
        previousRealm.interactable = realmIndex > 0;
        previousRealm.onClick.AddListener(() => SelectRoomControlsRealm(realmIndex - 1, returnToMap));
        Button nextRealm = CreateAnchoredButton(panel.transform, ">", 22, 58, 46, new Color(0.35f, 0.12f, 0.62f), 330f, 162f);
        nextRealm.interactable = realmIndex < realms.Count - 1;
        nextRealm.onClick.AddListener(() => SelectRoomControlsRealm(realmIndex + 1, returnToMap));

        string realmState = !IsRealmUnlocked(realmIndex) ? "Locked" : IsRealmComplete(realmIndex) ? "Complete" : "Unlocked";
        CreateAnchoredText(panel.transform, $"Realm {realmIndex + 1}/{realms.Count}: {realm.Name}", 22, FontStyle.Bold, Color.white, 560, 30, 0f, 174f);
        CreateAnchoredText(panel.transform, realmState, 16, FontStyle.Bold, GetRoomControlsRealmStateColor(realmState), 560, 22, 0f, 146f);

        IReadOnlyList<RoomDefinition> rooms = realm.Rooms;
        const float firstRowY = 96f;
        const float lastRowY = -120f;
        float spacing = rooms.Count > 1 ? Mathf.Min(54f, (firstRowY - lastRowY) / (rooms.Count - 1)) : 0f;
        for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            float y = firstRowY - roomIndex * spacing;
            RoomDefinition room = rooms[roomIndex];
            RoomProgressState state = GetRoomProgressState(realmIndex, roomIndex);
            bool overridden = inventory.GetRoomStateOverride(room) != RoomStateOverride.None;

            Text label = CreateAnchoredText(panel.transform, $"{roomIndex + 1}. {room.Name}{(overridden ? "  *" : "")}", 18, FontStyle.Bold, Color.white, 430, 40, -110f, y);
            label.alignment = TextAnchor.MiddleLeft;

            Button stateButton = CreateAnchoredButton(panel.transform, state.ToString(), 18, 190, 42, GetRoomControlsStateColor(state), 240f, y);
            int capturedRoomIndex = roomIndex;
            stateButton.onClick.AddListener(() => CycleRoomControlsState(realmIndex, capturedRoomIndex, returnToMap));
        }

        Button completeAll = CreateAnchoredButton(panel.transform, "Complete Realm", 17, 220, 42, new Color(0.14f, 0.42f, 0.2f), -250f, -184f);
        completeAll.onClick.AddListener(() => SetRoomControlsRealmState(realmIndex, RoomProgressState.Complete, returnToMap));

        Button resetRealm = CreateAnchoredButton(panel.transform, "Reset Realm", 17, 220, 42, new Color(0.45f, 0.22f, 0.08f), 0f, -184f);
        resetRealm.onClick.AddListener(() =>
        {
            inventory.ClearRoomStatesForTesting(realm);
            BuildRoomControlsUi(returnToMap);
        });

        Button goToRealm = CreateAnchoredButton(panel.transform, "Go to Realm", 17, 220, 42, new Color(0.12f, 0.3f, 0.42f), 250f, -184f);
        goToRealm.interactable = IsRealmUnlocked(realmIndex);
        goToRealm.onClick.AddListener(() =>
        {
            RemoveDevSettingsModal();
            EnterRealm(realmIndex);
        });

        CreateAnchoredText(panel.transform, "* set here. Reset Realm clears these and the realm's restores.", 13, FontStyle.Bold, muted, 700, 20, 0f, -218f);

        Button back = CreateAnchoredButton(panel.transform, "Back to Settings", 20, 230, 46, new Color(0.35f, 0.12f, 0.62f), -130f, -256f);
        back.onClick.AddListener(() => BuildDevSettingsUi(safeReturnToMap, devSettingsReturnToDen));
        Button close = CreateAnchoredButton(panel.transform, "Close", 20, 170, 46, new Color(0.18f, 0.16f, 0.22f), 130f, -256f);
        close.onClick.AddListener(() => ReturnFromDevSettings(safeReturnToMap));
    }

    private void SelectRoomControlsRealm(int realmIndex, bool returnToMap)
    {
        roomControlsRealmIndex = Mathf.Clamp(realmIndex, 0, RealmContentCatalog.AllRealms.Count - 1);
        BuildRoomControlsUi(returnToMap);
    }

    private void CycleRoomControlsState(int realmIndex, int roomIndex, bool returnToMap)
    {
        RoomDefinition room = RealmContentCatalog.AllRealms[realmIndex].Rooms[roomIndex];
        RoomProgressState next = RoomAccessRules.Next(GetRoomProgressState(realmIndex, roomIndex));
        inventory.SetRoomStateForTesting(room, next);
        BuildRoomControlsUi(returnToMap);
    }

    private void SetRoomControlsRealmState(int realmIndex, RoomProgressState state, bool returnToMap)
    {
        IReadOnlyList<RoomDefinition> rooms = RealmContentCatalog.AllRealms[realmIndex].Rooms;
        for (int index = 0; index < rooms.Count; index++)
        {
            inventory.SetRoomStateForTesting(rooms[index], state);
        }

        BuildRoomControlsUi(returnToMap);
    }

    private static Color GetRoomControlsStateColor(RoomProgressState state)
    {
        switch (state)
        {
            case RoomProgressState.Open:
                return new Color(0.16f, 0.34f, 0.58f);
            case RoomProgressState.Complete:
                return new Color(0.14f, 0.42f, 0.2f);
            default:
                return new Color(0.3f, 0.28f, 0.34f);
        }
    }

    private static Color GetRoomControlsRealmStateColor(string realmState)
    {
        if (realmState == "Complete")
        {
            return new Color(0.55f, 0.95f, 0.6f);
        }

        return realmState == "Locked" ? new Color(0.75f, 0.72f, 0.8f) : new Color(0.6f, 0.82f, 1f);
    }
}
