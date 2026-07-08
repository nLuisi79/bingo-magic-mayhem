using UnityEngine;

public sealed class BingoRoundState
{
    private readonly BingoRoomRules rules;

    public bool IsActive { get; private set; }
    public bool HasJackpotState { get; private set; }
    public float NextCallTimer { get; private set; }

    public BingoRoundState(BingoRoomRules rules)
    {
        this.rules = rules;
    }

    public void StartRound()
    {
        IsActive = true;
        HasJackpotState = false;
        NextCallTimer = 0f;
    }

    public void StopRound()
    {
        IsActive = false;
    }

    public void MarkJackpotState()
    {
        HasJackpotState = true;
    }

    public bool TickAutoCallTimer(float deltaTime)
    {
        if (!IsActive)
        {
            return false;
        }

        NextCallTimer -= deltaTime;
        if (NextCallTimer > 0f)
        {
            return false;
        }

        NextCallTimer = rules.AutoCallInterval;
        return true;
    }
}
