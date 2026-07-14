using BingoMagicMayhem.Rounds;
using NUnit.Framework;

public sealed class BingoRoundCountdownPresenterTests
{
    [Test]
    public void Build_UsesLatestCallWhenAvailable()
    {
        BingoRoundCountdownDisplayState state = BingoRoundCountdownPresenter.Build(3, "B-12", "Final ball called.");

        Assert.That(state.TimerLabel, Is.EqualTo("END 3"));
        Assert.That(state.CalledNumberLabel, Is.EqualTo("B-12\nEND 3"));
        Assert.That(state.BallsLeftLabel, Is.EqualTo("3"));
        Assert.That(state.BurstLabel, Is.EqualTo("ROUND ENDS IN 3"));
        Assert.That(state.StatusLabel, Is.EqualTo("Final ball called. Round ends in 3."));
    }

    [Test]
    public void Build_FallsBackToEndLabelWithoutLatestCall()
    {
        BingoRoundCountdownDisplayState state = BingoRoundCountdownPresenter.Build(1, "", "Room bingo pool exhausted.");

        Assert.That(state.CalledNumberLabel, Is.EqualTo("END 1"));
    }
}
