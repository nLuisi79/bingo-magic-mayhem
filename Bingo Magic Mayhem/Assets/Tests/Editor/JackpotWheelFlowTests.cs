using BingoMagicMayhem.Jackpot;
using NUnit.Framework;

public sealed class JackpotWheelFlowTests
{
    [Test]
    public void Rules_MapTopSegmentsToSpecialResultTiers()
    {
        JackpotWheelSpinResult jackpot = JackpotWheelRules.ResolveSpinResult(8, multiplier => (int)(100 * multiplier), 100, 200, 300);
        JackpotWheelSpinResult epic = JackpotWheelRules.ResolveSpinResult(9, multiplier => (int)(100 * multiplier), 100, 200, 300);
        JackpotWheelSpinResult legendary = JackpotWheelRules.ResolveSpinResult(10, multiplier => (int)(100 * multiplier), 100, 200, 300);

        Assert.That(jackpot.Label, Is.EqualTo("JACKPOT"));
        Assert.That(jackpot.ResetPot, Is.True);
        Assert.That(epic.Label, Is.EqualTo("EPIC"));
        Assert.That(epic.ResetPot, Is.True);
        Assert.That(legendary.Label, Is.EqualTo("LEGENDARY"));
        Assert.That(legendary.ResetPot, Is.True);
    }

    [Test]
    public void Flow_StacksCompletedSpinResultsUntilCollected()
    {
        JackpotWheelFlowController flow = new JackpotWheelFlowController();
        JackpotWheelSpinResult first = new JackpotWheelSpinResult("STANDARD", 40, false);
        JackpotWheelSpinResult second = new JackpotWheelSpinResult("STANDARD", 60, false);

        Assert.That(flow.TryBeginSpin(2, 3, first), Is.True);
        flow.CompleteSpin();
        Assert.That(flow.CollectedMana, Is.EqualTo(40));
        Assert.That(flow.CollectedResults.Count, Is.EqualTo(1));

        Assert.That(flow.TryBeginSpin(1, 5, second), Is.True);
        flow.CompleteSpin();
        Assert.That(flow.CollectedMana, Is.EqualTo(100));
        Assert.That(flow.CollectedResults.Count, Is.EqualTo(2));
        Assert.That(flow.LastSpinResult.Label, Is.EqualTo("STANDARD"));
    }

    [Test]
    public void Flow_CollectPreservesResetFlagWhenAnySpecialTierWasStacked()
    {
        JackpotWheelFlowController flow = new JackpotWheelFlowController();
        flow.TryBeginSpin(2, 1, new JackpotWheelSpinResult("STANDARD", 25, false));
        flow.CompleteSpin();
        flow.TryBeginSpin(1, 8, new JackpotWheelSpinResult("JACKPOT", 125, true));
        flow.CompleteSpin();

        JackpotWheelCollectResult collectResult = flow.Collect();

        Assert.That(collectResult, Is.Not.Null);
        Assert.That(collectResult.CollectedMana, Is.EqualTo(150));
        Assert.That(collectResult.SpinResultCount, Is.EqualTo(2));
        Assert.That(collectResult.ResetPot, Is.True);
        Assert.That(flow.CollectionConfirmed, Is.True);
        Assert.That(flow.LastCollectedMana, Is.EqualTo(150));
        Assert.That(flow.HasStackToCollect, Is.False);
    }
}
