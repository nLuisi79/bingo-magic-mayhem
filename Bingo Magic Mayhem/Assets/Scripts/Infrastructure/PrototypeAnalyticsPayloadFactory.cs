using System;
using UnityEngine;

namespace BingoMagicMayhem.Infrastructure
{
    [Serializable]
    public class PrototypeRoomAnalyticsPayload
    {
        public int RealmIndex;
        public int RoomIndex;
        public string RoomName = "";
        public string RoomMode = "";
        public int SelectedCardCount;
        public int ManaBetPerCard;
        public bool RoomRestored;
    }

    [Serializable]
    public sealed class PrototypeRoundStartedAnalyticsPayload : PrototypeRoomAnalyticsPayload
    {
        public int EntryCost;
    }

    [Serializable]
    public sealed class PrototypeBingoClaimedAnalyticsPayload : PrototypeRoomAnalyticsPayload
    {
        public int CardIndex;
        public int BingoDelta;
        public int ClaimedCount;
        public int RoomBingosClaimed;
        public int RoomBingoPool;
        public bool BlackoutRoom;
        public string Trigger = "";
    }

    [Serializable]
    public sealed class PrototypeRoundCompletedAnalyticsPayload : PrototypeRoomAnalyticsPayload
    {
        public string Reason = "";
        public int PlayerBingos;
        public int SimulatedBingos;
        public int RoomBingos;
        public int BestCardBingos;
        public int JackpotCards;
        public int ManaReward;
        public int PlayerXp;
    }

    [Serializable]
    public sealed class PrototypeDailyRewardAnalyticsPayload
    {
        public string ClaimSurface = "";
        public int StreakDay;
        public int Mana;
        public int Crystals;
        public string ItemName = "";
        public int ItemQuantity;
        public string Label = "";
    }

    [Serializable]
    public sealed class PrototypeDailySpinAnalyticsPayload
    {
        public string ClaimSurface = "";
        public int RewardIndex;
        public int Mana;
        public int Crystals;
        public int ClairvoyanceMinutes;
        public string ItemName = "";
        public int ItemQuantity;
    }

    [Serializable]
    public sealed class PrototypeInboxAnalyticsPayload
    {
        public string Category = "";
        public bool ClaimAll;
        public bool HasCovenGift;
        public string RewardSource = "";
        public int Mana;
        public int Crystals;
        public int ClairvoyanceMinutes;
        public int JackpotSpins;
        public string ItemName = "";
        public int ItemQuantity;
    }

    [Serializable]
    public sealed class PrototypeInboxMessageReadAnalyticsPayload
    {
        public string Category = "";
        public string ReadSource = "";
        public int UnreadRemaining;
        public int ClaimableRemaining;
    }

    [Serializable]
    public sealed class PrototypeInboxCategoryClearedAnalyticsPayload
    {
        public string Category = "";
        public int ClearedCount;
    }

    [Serializable]
    public class PrototypeRewardGrantAnalyticsPayload
    {
        public string Source = "";
        public string Context = "";
        public int Mana;
        public int Crystals;
        public int ClairvoyanceMinutes;
        public int JackpotSpins;
        public string ItemName = "";
        public int ItemQuantity;
    }

    [Serializable]
    public sealed class PrototypeAlbumRewardAnalyticsPayload : PrototypeRewardGrantAnalyticsPayload
    {
        public string Album = "";
        public string RewardScope = "";
        public int EntryNumber;
        public int SetNumber;
    }

    [Serializable]
    public sealed class PrototypeRoundRewardsCollectedAnalyticsPayload : PrototypeRewardGrantAnalyticsPayload
    {
        public int PlayerBingos;
        public int SimulatedBingos;
        public int RoomBingos;
        public int PlayerXp;
    }

    [Serializable]
    public sealed class PrototypeRoomRestoredAnalyticsPayload : PrototypeRoomAnalyticsPayload
    {
        public int RestoreRewardMana;
    }

    [Serializable]
    public sealed class PrototypeSocialFreebieAnalyticsPayload : PrototypeRewardGrantAnalyticsPayload
    {
        public string Code = "";
    }

    [Serializable]
    public sealed class PrototypeSocialHelpRequestAnalyticsPayload
    {
        public string Context = "";
        public bool CovenTab;
        public string CardId = "";
        public string CardName = "";
        public int UsedToday;
        public int DailyLimit;
    }

    [Serializable]
    public sealed class PrototypeFriendManaAnalyticsPayload
    {
        public string Context = "";
        public int Amount;
        public int DailyCount;
        public int DailyLimit;
    }

    [Serializable]
    public sealed class PrototypeCovenOrbsAnalyticsPayload
    {
        public int Contributed;
        public int RemainingHeldOrbs;
        public string Context = "";
    }

    [Serializable]
    public sealed class PrototypeWildCardUsedAnalyticsPayload
    {
        public string Context = "";
        public string Album = "";
        public int EntryNumber;
        public string CardId = "";
        public string CardName = "";
        public string Tier = "";
        public int RemainingWildCards;
    }

    [Serializable]
    public sealed class PrototypeCovenWishGiftAnalyticsPayload
    {
        public string Context = "";
        public string WishType = "";
        public string ItemName = "";
        public int ItemQuantity;
        public int RemainingInventoryCount;
    }

    [Serializable]
    public sealed class PrototypeCovenEmporiumPurchaseAnalyticsPayload : PrototypeRewardGrantAnalyticsPayload
    {
        public string OfferId = "";
        public string OfferName = "";
        public int OrbCost;
        public int RemainingHeldOrbs;
    }

    [Serializable]
    public sealed class PrototypeJackpotCollectedAnalyticsPayload
    {
        public string Context = "";
        public int CollectedMana;
        public int SpinResultCount;
        public bool ResetPot;
        public int PendingSpinsRemaining;
    }

    public static class PrototypeAnalyticsPayloadFactory
    {
        public static string CreateRoomEnteredJson(int selectedCardCount, int manaBetPerCard, bool roomRestored)
        {
            return JsonUtility.ToJson(BuildRoomContext(selectedCardCount, manaBetPerCard, roomRestored));
        }

        public static string CreateRoundStartedJson(int selectedCardCount, int manaBetPerCard, bool roomRestored, int entryCost)
        {
            PrototypeRoundStartedAnalyticsPayload payload = new PrototypeRoundStartedAnalyticsPayload
            {
                EntryCost = entryCost
            };
            CopyRoomContext(BuildRoomContext(selectedCardCount, manaBetPerCard, roomRestored), payload);
            return JsonUtility.ToJson(payload);
        }

        public static string CreateBingoClaimedJson(
            int selectedCardCount,
            int manaBetPerCard,
            bool roomRestored,
            int cardIndex,
            int bingoDelta,
            int claimedCount,
            int roomBingosClaimed,
            int roomBingoPool,
            bool blackoutRoom,
            string trigger)
        {
            PrototypeBingoClaimedAnalyticsPayload payload = new PrototypeBingoClaimedAnalyticsPayload
            {
                CardIndex = cardIndex,
                BingoDelta = bingoDelta,
                ClaimedCount = claimedCount,
                RoomBingosClaimed = roomBingosClaimed,
                RoomBingoPool = roomBingoPool,
                BlackoutRoom = blackoutRoom,
                Trigger = trigger ?? ""
            };
            CopyRoomContext(BuildRoomContext(selectedCardCount, manaBetPerCard, roomRestored), payload);
            return JsonUtility.ToJson(payload);
        }

        public static string CreateRoundCompletedJson(
            int selectedCardCount,
            int manaBetPerCard,
            bool roomRestored,
            string reason,
            global::RewardPreview preview)
        {
            PrototypeRoundCompletedAnalyticsPayload payload = new PrototypeRoundCompletedAnalyticsPayload
            {
                Reason = reason ?? "",
                PlayerBingos = preview?.PlayerBingos ?? 0,
                SimulatedBingos = preview?.SimulatedBingos ?? 0,
                RoomBingos = preview?.RoomBingos ?? 0,
                BestCardBingos = preview?.BestCardBingos ?? 0,
                JackpotCards = preview?.JackpotCards ?? 0,
                ManaReward = preview?.ManaReward ?? 0,
                PlayerXp = preview?.PlayerXp ?? 0
            };
            CopyRoomContext(BuildRoomContext(selectedCardCount, manaBetPerCard, roomRestored), payload);
            return JsonUtility.ToJson(payload);
        }

        public static string CreateDailyBonusClaimedJson(global::DailyBonusRewardDefinition reward, int streakDay)
        {
            return JsonUtility.ToJson(new PrototypeDailyRewardAnalyticsPayload
            {
                ClaimSurface = "daily_bonus_den",
                StreakDay = streakDay,
                Mana = reward.Mana,
                Crystals = reward.Crystals,
                ItemName = reward.ItemName ?? "",
                ItemQuantity = reward.ItemQuantity,
                Label = reward.Label ?? ""
            });
        }

        public static string CreateDailySpinClaimedJson(global::PrototypeRewardGrant reward, int rewardIndex)
        {
            return JsonUtility.ToJson(new PrototypeDailySpinAnalyticsPayload
            {
                ClaimSurface = "daily_spin_den",
                RewardIndex = rewardIndex,
                Mana = reward.Mana,
                Crystals = reward.Crystals,
                ClairvoyanceMinutes = reward.ClairvoyanceMinutes,
                ItemName = reward.Items.Count > 0 ? reward.Items[0].Name : "",
                ItemQuantity = reward.Items.Count > 0 ? reward.Items[0].Quantity : 0
            });
        }

        public static string CreateInboxMessageReadJson(string readSource, int unreadRemaining, int claimableRemaining)
        {
            return JsonUtility.ToJson(new PrototypeInboxMessageReadAnalyticsPayload
            {
                Category = global::PrototypeInboxCategory.Messages.ToString(),
                ReadSource = readSource ?? "",
                UnreadRemaining = unreadRemaining,
                ClaimableRemaining = claimableRemaining
            });
        }

        public static string CreateInboxItemClaimedJson(global::PrototypeInboxItem item, bool claimAll)
        {
            return JsonUtility.ToJson(BuildInboxPayload(item, claimAll));
        }

        public static string CreateInboxCategoryClearedJson(global::PrototypeInboxCategory category, int clearedCount)
        {
            return JsonUtility.ToJson(new PrototypeInboxCategoryClearedAnalyticsPayload
            {
                Category = category.ToString(),
                ClearedCount = clearedCount
            });
        }

        public static string CreateRoundRewardsCollectedJson(global::RewardPreview preview)
        {
            return JsonUtility.ToJson(new PrototypeRoundRewardsCollectedAnalyticsPayload
            {
                Source = "round_rewards",
                Context = "round_collect",
                Mana = preview?.ManaReward ?? 0,
                JackpotSpins = preview?.JackpotSpinsEarned ?? 0,
                PlayerBingos = preview?.PlayerBingos ?? 0,
                SimulatedBingos = preview?.SimulatedBingos ?? 0,
                RoomBingos = preview?.RoomBingos ?? 0,
                PlayerXp = preview?.PlayerXp ?? 0
            });
        }

        public static string CreateRoomRestoredJson(int selectedCardCount, int manaBetPerCard, int restoreRewardMana)
        {
            PrototypeRoomRestoredAnalyticsPayload payload = new PrototypeRoomRestoredAnalyticsPayload
            {
                RestoreRewardMana = restoreRewardMana
            };
            CopyRoomContext(BuildRoomContext(selectedCardCount, manaBetPerCard, true), payload);
            return JsonUtility.ToJson(payload);
        }

        public static string CreateAlbumRewardClaimedJson(
            string source,
            string album,
            string rewardScope,
            global::AlbumRewardDefinition reward,
            int entryNumber = 0,
            int setNumber = 0)
        {
            return JsonUtility.ToJson(new PrototypeAlbumRewardAnalyticsPayload
            {
                Source = source ?? "",
                Context = "album_claim",
                Mana = reward.Mana,
                Crystals = reward.Crystals,
                ClairvoyanceMinutes = reward.ClairvoyanceHours * 60,
                Album = album ?? "",
                RewardScope = rewardScope ?? "",
                EntryNumber = entryNumber,
                SetNumber = setNumber
            });
        }

        public static string CreateSocialFreebieRedeemedJson(global::PrototypeRewardGrant reward, string code)
        {
            return JsonUtility.ToJson(new PrototypeSocialFreebieAnalyticsPayload
            {
                Source = reward.Source ?? "",
                Context = "social_freebie",
                Mana = reward.Mana,
                Crystals = reward.Crystals,
                ClairvoyanceMinutes = reward.ClairvoyanceMinutes,
                JackpotSpins = reward.JackpotSpins,
                ItemName = reward.Items.Count > 0 ? reward.Items[0].Name : "",
                ItemQuantity = reward.Items.Count > 0 ? reward.Items[0].Quantity : 0,
                Code = code ?? ""
            });
        }

        public static string CreateSocialHelpRequestSentJson(bool covenTab, global::AlbumCardDefinition card, int usedToday, int dailyLimit)
        {
            return JsonUtility.ToJson(new PrototypeSocialHelpRequestAnalyticsPayload
            {
                Context = "ingredient_help",
                CovenTab = covenTab,
                CardId = card?.Id ?? "",
                CardName = card?.CardName ?? "",
                UsedToday = usedToday,
                DailyLimit = dailyLimit
            });
        }

        public static string CreateFriendManaJson(string context, int amount, int dailyCount, int dailyLimit)
        {
            return JsonUtility.ToJson(new PrototypeFriendManaAnalyticsPayload
            {
                Context = context ?? "",
                Amount = amount,
                DailyCount = dailyCount,
                DailyLimit = dailyLimit
            });
        }

        public static string CreateCovenOrbsContributedJson(int contributed, int remainingHeldOrbs)
        {
            return JsonUtility.ToJson(new PrototypeCovenOrbsAnalyticsPayload
            {
                Contributed = contributed,
                RemainingHeldOrbs = remainingHeldOrbs,
                Context = "coven_circle"
            });
        }

        public static string CreateWildCardUsedJson(global::GrimoireEntryDefinition entry, global::AlbumCardDefinition card, int remainingWildCards)
        {
            return JsonUtility.ToJson(new PrototypeWildCardUsedAnalyticsPayload
            {
                Context = "grimoire_use_wild",
                Album = "grimoire",
                EntryNumber = entry?.EntryNumber ?? 0,
                CardId = card?.Id ?? "",
                CardName = card?.CardName ?? "",
                Tier = card?.Tier.ToString() ?? "",
                RemainingWildCards = remainingWildCards
            });
        }

        public static string CreateCovenWishGiftSentJson(string wishType, string itemName, int itemQuantity, int remainingInventoryCount)
        {
            return JsonUtility.ToJson(new PrototypeCovenWishGiftAnalyticsPayload
            {
                Context = "coven_wish_gift",
                WishType = wishType ?? "",
                ItemName = itemName ?? "",
                ItemQuantity = itemQuantity,
                RemainingInventoryCount = remainingInventoryCount
            });
        }

        public static string CreateCovenEmporiumPurchaseJson(
            string offerId,
            string offerName,
            int orbCost,
            int remainingHeldOrbs,
            int mana = 0,
            int crystals = 0,
            string itemName = "",
            int itemQuantity = 0)
        {
            return JsonUtility.ToJson(new PrototypeCovenEmporiumPurchaseAnalyticsPayload
            {
                Source = "coven_emporium",
                Context = "coven_emporium_purchase",
                OfferId = offerId ?? "",
                OfferName = offerName ?? "",
                OrbCost = orbCost,
                RemainingHeldOrbs = remainingHeldOrbs,
                Mana = mana,
                Crystals = crystals,
                ItemName = itemName ?? "",
                ItemQuantity = itemQuantity
            });
        }

        public static string CreateJackpotCollectedJson(int collectedMana, int spinResultCount, bool resetPot, int pendingSpinsRemaining)
        {
            return JsonUtility.ToJson(new PrototypeJackpotCollectedAnalyticsPayload
            {
                Context = "jackpot_wheel_collect",
                CollectedMana = collectedMana,
                SpinResultCount = spinResultCount,
                ResetPot = resetPot,
                PendingSpinsRemaining = pendingSpinsRemaining
            });
        }

        public static PrototypeInboxAnalyticsPayload BuildInboxPayload(global::PrototypeInboxItem item, bool claimAll)
        {
            PrototypeInboxAnalyticsPayload payload = new PrototypeInboxAnalyticsPayload
            {
                Category = item.Category.ToString(),
                ClaimAll = claimAll,
                RewardSource = item.Reward.Source ?? "",
                Mana = item.Reward.Mana,
                Crystals = item.Reward.Crystals,
                ClairvoyanceMinutes = item.Reward.ClairvoyanceMinutes,
                JackpotSpins = item.Reward.JackpotSpins,
                ItemName = item.Reward.Items.Count > 0 ? item.Reward.Items[0].Name : "",
                ItemQuantity = item.Reward.Items.Count > 0 ? item.Reward.Items[0].Quantity : 0
            };

            if (item.TryGetCovenGift(out global::CovenInboxGiftInfo gift))
            {
                payload.HasCovenGift = true;
                payload.ItemName = gift.ItemName ?? payload.ItemName;
                payload.ItemQuantity = gift.Quantity;
            }

            return payload;
        }

        public static PrototypeRoomAnalyticsPayload BuildRoomContext(int selectedCardCount, int manaBetPerCard, bool roomRestored)
        {
            return new PrototypeRoomAnalyticsPayload
            {
                RealmIndex = global::RealmContentCatalog.ActivePrototypeRealmIndex + 1,
                RoomIndex = global::RealmContentCatalog.ActivePrototypeRoomIndex + 1,
                RoomName = global::RealmContentCatalog.ActivePrototypeRoom.Name,
                RoomMode = global::RealmContentCatalog.ActivePrototypeRoom.ModeLabel,
                SelectedCardCount = selectedCardCount,
                ManaBetPerCard = manaBetPerCard,
                RoomRestored = roomRestored
            };
        }

        private static void CopyRoomContext(PrototypeRoomAnalyticsPayload source, PrototypeRoomAnalyticsPayload target)
        {
            target.RealmIndex = source.RealmIndex;
            target.RoomIndex = source.RoomIndex;
            target.RoomName = source.RoomName;
            target.RoomMode = source.RoomMode;
            target.SelectedCardCount = source.SelectedCardCount;
            target.ManaBetPerCard = source.ManaBetPerCard;
            target.RoomRestored = source.RoomRestored;
        }
    }
}
