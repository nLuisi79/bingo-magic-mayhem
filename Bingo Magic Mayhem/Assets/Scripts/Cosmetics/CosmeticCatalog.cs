using System;
using System.Collections.Generic;
using UnityEngine;

namespace BingoMagicMayhem.Cosmetics
{
    public enum CosmeticKind
    {
        Avatar,
        Frame,
        Dauber
    }

    public sealed class CosmeticDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public CosmeticKind Kind { get; }
        public string PrimaryAssetKey { get; }
        public string SecondaryAssetKey { get; }
        public bool IsPlaceholder { get; }

        public CosmeticDefinition(
            string id,
            string displayName,
            CosmeticKind kind,
            string primaryAssetKey,
            string secondaryAssetKey = "",
            bool isPlaceholder = false)
        {
            Id = id;
            DisplayName = displayName;
            Kind = kind;
            PrimaryAssetKey = primaryAssetKey;
            SecondaryAssetKey = secondaryAssetKey;
            IsPlaceholder = isPlaceholder;
        }
    }

    public interface ICosmeticSpriteResolver
    {
        Sprite Load(string assetKey);
    }

    /// <summary>
    /// Local Beta resolver. A future Addressables resolver can replace it while
    /// retaining catalog ids and logical asset keys.
    /// </summary>
    public sealed class ResourcesCosmeticSpriteResolver : ICosmeticSpriteResolver
    {
        public Sprite Load(string assetKey)
        {
            return string.IsNullOrWhiteSpace(assetKey) ? null : Resources.Load<Sprite>(assetKey);
        }
    }

    public static class CosmeticCatalog
    {
        private static readonly CosmeticDefinition[] AvatarDefinitions =
        {
            new CosmeticDefinition("avatar_moon_witch", "Moon Witch", CosmeticKind.Avatar, "cosmetics/avatars/avatar_moon_witch_v001"),
            new CosmeticDefinition("avatar_sun_mage", "Sun Mage", CosmeticKind.Avatar, "cosmetics/avatars/avatar_sun_mage_v001"),
            new CosmeticDefinition("avatar_garden_seer", "Garden Seer", CosmeticKind.Avatar, "cosmetics/avatars/avatar_garden_seer_v001"),
            new CosmeticDefinition("avatar_star_caller", "Star Caller", CosmeticKind.Avatar, "cosmetics/avatars/avatar_star_caller_v001")
        };

        private static readonly CosmeticDefinition[] FrameDefinitions =
        {
            new CosmeticDefinition("frame_plain_gold", "Plain Gold", CosmeticKind.Frame, "cosmetics/frames/frame_plain_gold_v001"),
            new CosmeticDefinition("frame_violet_gem", "Violet Gem", CosmeticKind.Frame, "cosmetics/frames/frame_violet_gem_v001"),
            new CosmeticDefinition("frame_moonlit_vine", "Moonlit Vine", CosmeticKind.Frame, "cosmetics/frames/frame_moonlit_vine_v001"),
            new CosmeticDefinition("frame_rank_placeholder", "Rank Frame", CosmeticKind.Frame, "", isPlaceholder: true)
        };

        private static readonly CosmeticDefinition[] DauberDefinitions =
        {
            new CosmeticDefinition("dauber_classic_star", "Classic Star", CosmeticKind.Dauber, "cosmetics/daubers/icons/dauber_classic_star_icon_v001", "cosmetics/daubers/marks/dauber_classic_star_mark_v001"),
            new CosmeticDefinition("dauber_moon_drop", "Moon Drop", CosmeticKind.Dauber, "cosmetics/daubers/icons/dauber_moon_drop_icon_v001", "cosmetics/daubers/marks/dauber_moon_drop_mark_v001"),
            new CosmeticDefinition("dauber_crystal_spark", "Crystal Spark", CosmeticKind.Dauber, "cosmetics/daubers/icons/dauber_crystal_spark_icon_v001", "cosmetics/daubers/marks/dauber_crystal_spark_mark_v001"),
            new CosmeticDefinition("dauber_garden_bloom", "Garden Bloom", CosmeticKind.Dauber, "cosmetics/daubers/icons/dauber_garden_bloom_icon_v001", "cosmetics/daubers/marks/dauber_garden_bloom_mark_v001")
        };

        public static IReadOnlyList<CosmeticDefinition> Avatars => AvatarDefinitions;
        public static IReadOnlyList<CosmeticDefinition> Frames => FrameDefinitions;
        public static IReadOnlyList<CosmeticDefinition> Daubers => DauberDefinitions;

        public static CosmeticDefinition Find(CosmeticKind kind, string id)
        {
            IReadOnlyList<CosmeticDefinition> definitions = kind == CosmeticKind.Avatar
                ? Avatars
                : kind == CosmeticKind.Frame ? Frames : Daubers;

            foreach (CosmeticDefinition definition in definitions)
            {
                if (string.Equals(definition.Id, id, StringComparison.Ordinal))
                {
                    return definition;
                }
            }

            return definitions[0];
        }
    }
}
