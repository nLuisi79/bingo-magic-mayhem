using UnityEngine;

public enum BingoRoomMode
{
    Standard,
    Special
}

public sealed class BingoRoomRules
{
    public const int BoardSize = 5;
    public const int JackpotBingosPerCard = 5;
    public const int BlackoutPlayableSquares = BoardSize * BoardSize - 1;

    public readonly BingoRoomMode Mode = BingoRoomMode.Standard;
    public readonly string RoomModeName = "Standard Room";
    public readonly float AutoCallInterval = 5.0f;
    public readonly int MaxBallCalls = 62;
    public readonly int BlackoutMaxBallCalls = 75;
    public readonly int RoomBingoPool = 16;
    public readonly float ClairvoyanceAlertDelay = 3.8f;
    public readonly float FastDaubWindow = 1.5f;
    public readonly float NormalDaubWindow = 4.0f;
    public readonly int FastDaubXp = 12;
    public readonly int NormalDaubXp = 8;
    public readonly int ClairvoyanceDaubXp = 4;
    public readonly int BingoClaimXp = 25;
    public readonly int JackpotStateXp = 100;

    public bool UsesStandardLinePatterns => Mode == BingoRoomMode.Standard;

    public int GetMaxBallCalls(int activeCardCount, int manaBetPerCard)
    {
        int baseCalls;
        if (activeCardCount <= 1)
        {
            baseCalls = 34;
        }
        else if (activeCardCount <= 2)
        {
            baseCalls = 38;
        }
        else if (activeCardCount <= 4)
        {
            baseCalls = 44;
        }
        else
        {
            baseCalls = 48;
        }

        int betBonus = Mathf.Clamp((manaBetPerCard - 25) / 75, 0, 2);
        return Mathf.Min(50, baseCalls + betBonus);
    }

    public int GetBlackoutMaxBallCalls(int activeCardCount, int manaBetPerCard)
    {
        int baseCalls;
        if (activeCardCount <= 1)
        {
            baseCalls = 70;
        }
        else if (activeCardCount <= 2)
        {
            baseCalls = 72;
        }
        else if (activeCardCount <= 4)
        {
            baseCalls = 74;
        }
        else
        {
            baseCalls = 75;
        }

        int betBonus = Mathf.Clamp((manaBetPerCard - 100) / 100, 0, 3);
        return Mathf.Min(BlackoutMaxBallCalls, baseCalls + betBonus);
    }

    public string GetBlackoutCallRuleText(int activeCardCount, int manaBetPerCard)
    {
        return $"{GetBlackoutMaxBallCalls(activeCardCount, manaBetPerCard)} calls max";
    }

    public float GetClairvoyanceAlertDelay(int activeCardCount)
    {
        if (activeCardCount >= 6)
        {
            return 6.4f;
        }

        if (activeCardCount >= 4)
        {
            return 5.2f;
        }

        return ClairvoyanceAlertDelay;
    }

    public int GetDaubXp(float daubDelay, bool clairvoyanceAssisted)
    {
        if (clairvoyanceAssisted)
        {
            return ClairvoyanceDaubXp;
        }

        if (daubDelay <= FastDaubWindow)
        {
            return FastDaubXp;
        }

        return daubDelay <= NormalDaubWindow ? NormalDaubXp : ClairvoyanceDaubXp + 1;
    }
}
