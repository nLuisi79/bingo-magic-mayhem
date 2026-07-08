import pathlib

import openpyxl


SOURCE = pathlib.Path(
    r"C:\Users\nLuis\OneDrive\Documents\Bingo Magic Mayhem\outputs\potions-tuning\BMM Potions Tuned v1.xlsx"
)
OUTPUT = pathlib.Path(
    r"C:\Users\nLuis\OneDrive\Documents\Bingo Magic Mayhem\Bingo Magic Mayhem\Assets\Scripts\RealmContentCatalog.cs"
)


def escape(value):
    return str(value).replace("\\", "\\\\").replace('"', '\\"')


def load_realms():
    workbook = openpyxl.load_workbook(SOURCE, data_only=True)
    sheet = workbook["Tuned Catalog"]
    realms = []
    current_realm = None
    current_room = None

    for row in sheet.iter_rows(min_row=2, values_only=True):
        realm_no, realm, room_no, room, room_type, potion, _, ingredient, _, tuned_count, rarity, _ = row[:12]
        if not realm:
            continue

        if current_realm is None or current_realm["name"] != realm:
            current_realm = {"index": int(realm_no), "name": realm, "rooms": []}
            realms.append(current_realm)
            current_room = None

        if current_room is None or current_room["name"] != room:
            current_room = {
                "index": int(room_no),
                "name": room,
                "type": room_type,
                "potion": potion,
                "ingredients": [],
            }
            current_realm["rooms"].append(current_room)

        rarity_name = {
            "Key": "Key Ingredient",
            "Rare": "Rare",
            "Common": "Common",
            "Uncommon": "Uncommon",
        }.get(str(rarity), str(rarity))

        current_room["ingredients"].append(
            {
                "name": ingredient,
                "count": int(tuned_count),
                "rarity": rarity_name,
            }
        )

    return realms


def build_catalog(realms):
    lines = [
        "using System.Collections.Generic;",
        "",
        "public sealed class RealmDefinition",
        "{",
        "    public RealmDefinition(string name, IReadOnlyList<RoomDefinition> rooms)",
        "    {",
        "        Name = name;",
        "        Rooms = rooms;",
        "    }",
        "",
        "    public string Name { get; private set; }",
        "    public IReadOnlyList<RoomDefinition> Rooms { get; private set; }",
        "}",
        "",
        "public sealed class RoomDefinition",
        "{",
        "    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients)",
        '        : this(name, potionName, ingredients, BingoRoomMode.Standard, "Standard", RoomProgressionProfile.LevelOne)',
        "    {",
        "    }",
        "",
        "    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, BingoRoomMode mode, string modeLabel)",
        "        : this(name, potionName, ingredients, mode, modeLabel, RoomProgressionProfile.LevelOne)",
        "    {",
        "    }",
        "",
        "    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, RoomProgressionProfile progression)",
        '        : this(name, potionName, ingredients, BingoRoomMode.Standard, "Standard", progression)',
        "    {",
        "    }",
        "",
        "    public RoomDefinition(string name, string potionName, IReadOnlyList<IngredientRequirement> ingredients, BingoRoomMode mode, string modeLabel, RoomProgressionProfile progression)",
        "    {",
        "        Name = name;",
        "        PotionName = potionName;",
        "        Ingredients = ingredients;",
        "        Mode = mode;",
        "        ModeLabel = modeLabel;",
        "        Progression = progression;",
        "    }",
        "",
        "    public string Name { get; private set; }",
        "    public string PotionName { get; private set; }",
        "    public IReadOnlyList<IngredientRequirement> Ingredients { get; private set; }",
        "    public BingoRoomMode Mode { get; private set; }",
        "    public string ModeLabel { get; private set; }",
        "    public RoomProgressionProfile Progression { get; private set; }",
        "    public bool IsSpecial => Mode == BingoRoomMode.Special;",
        "}",
        "",
        "public sealed class RoomProgressionProfile",
        "{",
        "    public static readonly RoomProgressionProfile LevelOne = Create(1, 1, 0);",
        "",
        "    public RoomProgressionProfile(",
        "        int level,",
        "        int minManaBet,",
        "        int maxManaBet,",
        "        int betStep,",
        "        int minimumJackpotPot,",
        "        float jackpotContributionRate,",
        "        float xpMultiplier,",
        "        float ingredientDropChance,",
        "        float cellRewardChance,",
        "        int restoreManaReward)",
        "    {",
        "        Level = level;",
        "        MinManaBet = minManaBet;",
        "        MaxManaBet = maxManaBet;",
        "        BetStep = betStep;",
        "        MinimumJackpotPot = minimumJackpotPot;",
        "        JackpotContributionRate = jackpotContributionRate;",
        "        XpMultiplier = xpMultiplier;",
        "        IngredientDropChance = ingredientDropChance;",
        "        CellRewardChance = cellRewardChance;",
        "        RestoreManaReward = restoreManaReward;",
        "    }",
        "",
        "    public int Level { get; private set; }",
        "    public int MinManaBet { get; private set; }",
        "    public int MaxManaBet { get; private set; }",
        "    public int BetStep { get; private set; }",
        "    public int MinimumJackpotPot { get; private set; }",
        "    public float JackpotContributionRate { get; private set; }",
        "    public float XpMultiplier { get; private set; }",
        "    public float IngredientDropChance { get; private set; }",
        "    public float CellRewardChance { get; private set; }",
        "    public int RestoreManaReward { get; private set; }",
        "",
        "    public static RoomProgressionProfile Create(int realmIndex, int roomIndex, int roomOffset)",
        "    {",
        "        int level = ((realmIndex - 1) * 4) + roomIndex;",
        "        bool special = roomIndex == 4;",
        "        int minBet = 25 + ((realmIndex - 1) * 25);",
        "        int maxBet = minBet * (special ? 8 : 6);",
        "        int betStep = realmIndex >= 4 ? 50 : 25;",
        "        int minimumPot = 250 + ((realmIndex - 1) * 250) + (roomOffset * 125);",
        "        float contribution = 0.10f + (realmIndex * 0.01f) + (special ? 0.03f : 0f);",
        "        float xp = 1f + ((level - 1) * 0.08f);",
        "        float ingredient = special ? 0.16f : 0.22f;",
        "        float cellReward = special ? 0.60f : 0.75f;",
        "        int restoreReward = 1000 + ((realmIndex - 1) * 500) + (roomOffset * 250);",
        "        return new RoomProgressionProfile(level, minBet, maxBet, betStep, minimumPot, contribution, xp, ingredient, cellReward, restoreReward);",
        "    }",
        "}",
        "",
        "public sealed class IngredientRequirement",
        "{",
        "    public IngredientRequirement(string name, int required, string rarity, string description)",
        "    {",
        "        Name = name;",
        "        Required = required;",
        "        Rarity = rarity;",
        "        Description = description;",
        "    }",
        "",
        "    public string Name { get; private set; }",
        "    public int Required { get; private set; }",
        "    public string Rarity { get; private set; }",
        "    public string Description { get; private set; }",
        "}",
        "",
        "public static class RealmContentCatalog",
        "{",
        "    public static int ActivePrototypeRealmIndex { get; private set; }",
        "    public static int ActivePrototypeRoomIndex { get; private set; }",
        "",
        "    public static readonly IReadOnlyList<RealmDefinition> AllRealms = new List<RealmDefinition>",
        "    {",
    ]

    for realm in realms:
        lines += [
            f'        new RealmDefinition("{escape(realm["name"])}",',
            "            new List<RoomDefinition>",
            "            {",
        ]
        for room in realm["rooms"]:
            mode = "BingoRoomMode.Special" if room["type"] == "Special" else "BingoRoomMode.Standard"
            label = "Blackout" if room["type"] == "Special" else "Standard"
            lines += [
                "                new RoomDefinition(",
                f'                    "{escape(room["name"])}",',
                f'                    "{escape(room["potion"])}",',
                "                    new List<IngredientRequirement>",
                "                    {",
            ]
            for ingredient in room["ingredients"]:
                lines.append(
                    f'                        new IngredientRequirement("{escape(ingredient["name"])}", {ingredient["count"]}, "{escape(ingredient["rarity"])}", ""),'
                )
            lines += [
                "                    },",
                f"                    {mode},",
                f'                    "{label}",',
                f'                    RoomProgressionProfile.Create({realm["index"]}, {room["index"]}, {room["index"] - 1})),',
            ]
        lines += [
            "            }),",
        ]

    lines += [
        "    };",
        "",
        "    public static readonly RealmDefinition SunpetalConservatory = AllRealms[0];",
        "",
        "    public static RealmDefinition ActivePrototypeRealm => AllRealms[ActivePrototypeRealmIndex];",
        "    public static RoomDefinition ActivePrototypeRoom => ActivePrototypeRealm.Rooms[ActivePrototypeRoomIndex];",
        "",
        "    public static void SetActivePrototypeRealm(int realmIndex)",
        "    {",
        "        if (realmIndex < 0 || realmIndex >= AllRealms.Count)",
        "        {",
        "            return;",
        "        }",
        "",
        "        ActivePrototypeRealmIndex = realmIndex;",
        "        ActivePrototypeRoomIndex = 0;",
        "    }",
        "",
        "    public static void SetActivePrototypeRoom(int roomIndex)",
        "    {",
        "        if (roomIndex < 0 || roomIndex >= ActivePrototypeRealm.Rooms.Count)",
        "        {",
        "            return;",
        "        }",
        "",
        "        ActivePrototypeRoomIndex = roomIndex;",
        "    }",
        "",
        "    public static void SetActivePrototypeRoom(int realmIndex, int roomIndex)",
        "    {",
        "        SetActivePrototypeRealm(realmIndex);",
        "        SetActivePrototypeRoom(roomIndex);",
        "    }",
        "}",
    ]

    return "\n".join(lines) + "\n"


def main():
    realms = load_realms()
    OUTPUT.write_text(build_catalog(realms), encoding="utf-8")
    print(f"Wrote {OUTPUT} with {len(realms)} realms and {sum(len(realm['rooms']) for realm in realms)} rooms.")


if __name__ == "__main__":
    main()
