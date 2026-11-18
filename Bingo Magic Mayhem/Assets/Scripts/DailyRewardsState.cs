using System;
using BingoMagicMayhem.Infrastructure;
using UnityEngine;

// Daily Bonus streak and Daily Spin claim rules, extracted from PlayerInventoryState. Owns the claim dates, the streak
// and the streak-save count; the inventory still pays streak saves and grants the rewards. Dates are UTC "yyyy-MM-dd"
// and the save keys are unchanged, so existing saves keep loading.
public sealed class DailyRewardsState
{
    private const string SavePrefix = "BMM.Inventory.";
    private static readonly int[] StreakChestMilestones = { 7, 14, 30, 60, 100, 180, 365 };

    private readonly Func<DateTime> utcNow;

    public DailyRewardsState(Func<DateTime> utcNow = null)
    {
        this.utcNow = utcNow ?? (() => DateTime.UtcNow);
    }

    public int BonusStreak { get; private set; }
    public int StreakSaveUses { get; private set; }
    public string LastBonusClaimDate { get; private set; } = "";
    public string LastSpinClaimDate { get; private set; } = "";

    public void Load()
    {
        BonusStreak = DurablePrefs.GetInt(SavePrefix + "DailyBonusStreak", BonusStreak);
        LastBonusClaimDate = DurablePrefs.GetString(SavePrefix + "LastDailyBonusClaimDate", LastBonusClaimDate);
        StreakSaveUses = DurablePrefs.GetInt(SavePrefix + "DailyBonusStreakSaveUses", StreakSaveUses);
        LastSpinClaimDate = DurablePrefs.GetString(SavePrefix + "LastDailySpinClaimDate", LastSpinClaimDate);
    }

    // Stages values for the inventory's save; the inventory flushes DurablePrefs.
    public void Write()
    {
        DurablePrefs.SetInt(SavePrefix + "DailyBonusStreak", BonusStreak);
        DurablePrefs.SetString(SavePrefix + "LastDailyBonusClaimDate", LastBonusClaimDate);
        DurablePrefs.SetInt(SavePrefix + "DailyBonusStreakSaveUses", StreakSaveUses);
        DurablePrefs.SetString(SavePrefix + "LastDailySpinClaimDate", LastSpinClaimDate);
    }

    public void Reset()
    {
        BonusStreak = 0;
        StreakSaveUses = 0;
        LastBonusClaimDate = "";
        LastSpinClaimDate = "";
    }

    public bool CanClaimBonus() => LastBonusClaimDate != TodayKey();

    public bool HasMissedStreak()
    {
        if (BonusStreak <= 0 || string.IsNullOrEmpty(LastBonusClaimDate))
        {
            return false;
        }

        if (!DateTime.TryParse(LastBonusClaimDate, out DateTime lastClaimDate))
        {
            return false;
        }

        return lastClaimDate.Date < utcNow().Date.AddDays(-1);
    }

    public int GetStreakSaveCost(int[] costLadder)
    {
        if (costLadder == null || costLadder.Length == 0)
        {
            return 0;
        }

        return costLadder[Mathf.Clamp(StreakSaveUses, 0, costLadder.Length - 1)];
    }

    // Day 1-7 of the bonus cycle that the next claim (or today's, once claimed) shows.
    public int GetDisplayDay()
    {
        int effectiveStreak = CanClaimBonus() ? BonusStreak + 1 : Mathf.Max(1, BonusStreak);
        return ((effectiveStreak - 1) % 7) + 1;
    }

    public int GetNextChestDay()
    {
        for (int index = 0; index < StreakChestMilestones.Length; index++)
        {
            if (BonusStreak < StreakChestMilestones[index])
            {
                return StreakChestMilestones[index];
            }
        }

        int overage = BonusStreak - 365;
        return 365 + ((overage / 365) + 1) * 365;
    }

    public string GetBonusClaimStateText() => CanClaimBonus() ? $"Ready: Day {GetDisplayDay()}" : "Claimed today";

    // Records today's claim: continues the streak when the last claim was yesterday, otherwise restarts it.
    // Returns the 1-7 cycle day whose reward should be granted.
    public int RegisterBonusClaim()
    {
        DateTime today = utcNow().Date;
        if (DateTime.TryParse(LastBonusClaimDate, out DateTime lastClaimDate) && lastClaimDate.Date == today.AddDays(-1))
        {
            BonusStreak++;
        }
        else
        {
            BonusStreak = 1;
        }

        LastBonusClaimDate = TodayKey();
        return ((BonusStreak - 1) % 7) + 1;
    }

    // A streak save backdates the last claim to yesterday so today's claim continues the streak.
    public void RegisterStreakSave()
    {
        StreakSaveUses++;
        LastBonusClaimDate = YesterdayKey();
    }

    public bool CanClaimSpin() => LastSpinClaimDate != TodayKey();

    public string GetSpinClaimStateText() => CanClaimSpin() ? "free spin ready" : "spun today";

    public void RegisterSpinClaim()
    {
        LastSpinClaimDate = TodayKey();
    }

    public void DevAdvanceBonusDay()
    {
        LastBonusClaimDate = YesterdayKey();
    }

    public void DevResetBonusStreak()
    {
        BonusStreak = 0;
        StreakSaveUses = 0;
        LastBonusClaimDate = "";
    }

    public void DevResetSpinClaim()
    {
        LastSpinClaimDate = "";
        DurablePrefs.DeleteKey(SavePrefix + "LastDailySpinClaimDate");
        DurablePrefs.Save();
    }

    private string TodayKey() => utcNow().Date.ToString("yyyy-MM-dd");

    private string YesterdayKey() => utcNow().Date.AddDays(-1).ToString("yyyy-MM-dd");
}
