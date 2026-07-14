using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class BingoRoundFlowControllerTests
{
    [Test]
    public void BeginRound_ResetsCallerHistoryAndStartsActiveState()
    {
        BingoRoundFlowController flow = new BingoRoundFlowController(new BingoRoomRules());

        flow.BeginRound();

        Assert.That(flow.IsActive, Is.True);
        Assert.That(flow.HasJackpotState, Is.False);
        Assert.That(flow.History.Count, Is.EqualTo(0));
        Assert.That(flow.CalledCount, Is.EqualTo(0));
    }

    [Test]
    public void TryCallNext_TracksHistoryCalledSetAndTimestamp()
    {
        BingoRoundFlowController flow = new BingoRoundFlowController(new BingoRoomRules());
        flow.BeginRound();

        bool called = flow.TryCallNext(12.5f, out int number);

        Assert.That(called, Is.True);
        Assert.That(number, Is.InRange(1, 75));
        Assert.That(flow.History.Count, Is.EqualTo(1));
        Assert.That(flow.IsNumberCalled(number), Is.True);
        Assert.That(flow.TryGetCalledAtTime(number, out float calledAtTime), Is.True);
        Assert.That(calledAtTime, Is.EqualTo(12.5f));
    }

    [Test]
    public void ResetSession_ClearsCallsAndStopsRound()
    {
        BingoRoundFlowController flow = new BingoRoundFlowController(new BingoRoomRules());
        flow.BeginRound();
        flow.TryCallNext(3f, out _);
        flow.MarkJackpotState();

        flow.ResetSession();

        Assert.That(flow.IsActive, Is.False);
        Assert.That(flow.History.Count, Is.EqualTo(0));
        Assert.That(flow.CalledCount, Is.EqualTo(0));
        Assert.That(flow.TryGetCalledAtTime(1, out _), Is.False);
    }
}
