// Room and realm access rules, including the prototype-only Room Controls overrides from Prototype Settings.
// Normal play: room 1 of an unlocked realm is open, later rooms open once the previous room is restored,
// and a realm unlocks once every room in the previous realm is restored.
// Room Controls can force a room Locked or Open (or Complete, which also writes the restore flag) so testers
// can jump around the realms. These overrides are dev/QA tools only and are not a player-facing mechanic.

public enum RoomProgressState
{
    Locked,
    Open,
    Complete
}

public enum RoomStateOverride
{
    None = 0,
    Locked = 1,
    Open = 2,
    Complete = 3
}

public static class RoomAccessRules
{
    public static RoomProgressState ResolveRoom(
        bool restored,
        RoomStateOverride roomOverride,
        bool realmUnlocked,
        bool isFirstRoom,
        bool previousRoomRestored)
    {
        if (restored)
        {
            return RoomProgressState.Complete;
        }

        if (roomOverride == RoomStateOverride.Locked)
        {
            return RoomProgressState.Locked;
        }

        if (roomOverride == RoomStateOverride.Open)
        {
            return RoomProgressState.Open;
        }

        if (!realmUnlocked)
        {
            return RoomProgressState.Locked;
        }

        return isFirstRoom || previousRoomRestored ? RoomProgressState.Open : RoomProgressState.Locked;
    }

    // A realm is reachable when it is the first realm, the previous realm is fully restored, or Room Controls
    // opened/completed one of its rooms (so testers can enter a realm without finishing the one before it).
    public static bool IsRealmUnlocked(bool isFirstRealm, bool previousRealmComplete, bool anyRoomForcedAccessible)
    {
        return isFirstRealm || previousRealmComplete || anyRoomForcedAccessible;
    }

    public static RoomProgressState Next(RoomProgressState state)
    {
        switch (state)
        {
            case RoomProgressState.Locked:
                return RoomProgressState.Open;
            case RoomProgressState.Open:
                return RoomProgressState.Complete;
            default:
                return RoomProgressState.Locked;
        }
    }

    public static RoomStateOverride ToOverride(RoomProgressState state)
    {
        switch (state)
        {
            case RoomProgressState.Locked:
                return RoomStateOverride.Locked;
            case RoomProgressState.Open:
                return RoomStateOverride.Open;
            default:
                return RoomStateOverride.Complete;
        }
    }
}
