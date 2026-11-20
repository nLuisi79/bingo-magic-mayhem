using System.Collections.Generic;
using BingoMagicMayhem.UI.Common;
using NUnit.Framework;
using UnityEngine;

// Checks that the 1600x900 stage fits inside the safe area of common landscape phones and tablets, using the same
// CanvasScaler settings as BingoPrototype.ConfigureCanvasScaler (1080x1920 reference, match 0.5) and SafePadding 32.
// Safe areas are the published landscape insets (notch/Dynamic Island on the sides, home indicator at the bottom).
public sealed class StageFitTests
{
    private static readonly Vector2 Reference = new Vector2(1080f, 1920f);
    private const float Match = 0.5f;
    private static readonly Vector2 Stage = new Vector2(1600f, 900f);
    private const float SafePadding = 32f;

    public sealed class Device
    {
        public Device(string name, float width, float height, Rect safeArea)
        {
            Name = name;
            Screen = new Vector2(width, height);
            SafeArea = safeArea;
        }

        public string Name { get; }
        public Vector2 Screen { get; }
        public Rect SafeArea { get; }
        public override string ToString() => Name;
    }

    public static IEnumerable<Device> Devices()
    {
        yield return new Device("iPhone SE (3rd gen)", 1334, 750, new Rect(0, 0, 1334, 750));
        yield return new Device("iPhone 13 mini", 2340, 1080, new Rect(132, 63, 2076, 1017));
        yield return new Device("iPhone 15 Pro", 2556, 1179, new Rect(177, 63, 2202, 1116));
        yield return new Device("iPhone 15 Pro Max", 2796, 1290, new Rect(177, 63, 2442, 1227));
        yield return new Device("Pixel 8 (left cutout)", 2400, 1080, new Rect(130, 0, 2270, 1080));
        yield return new Device("Budget Android 20:9", 1600, 720, new Rect(0, 0, 1600, 720));
        yield return new Device("Galaxy Z Fold inner", 2176, 1812, new Rect(0, 0, 2176, 1812));
        yield return new Device("iPad mini", 2266, 1488, new Rect(0, 40, 2266, 1448));
        yield return new Device("iPad 10th gen", 2360, 1640, new Rect(0, 40, 2360, 1600));
        yield return new Device("iPad Pro 12.9", 2732, 2048, new Rect(0, 40, 2732, 2008));
        yield return new Device("Desktop 1080p", 1920, 1080, new Rect(0, 0, 1920, 1080));
    }

    [TestCaseSource(nameof(Devices))]
    public void StandardStageFitsInsideTheSafeAreaWithoutHittingTheMinimumScale(Device device)
    {
        AssertFits(device, fillsViewport: false);
    }

    [TestCaseSource(nameof(Devices))]
    public void FullBleedStageFitsInsideTheSafeArea(Device device)
    {
        AssertFits(device, fillsViewport: true);
    }

    [Test]
    public void SafeAreaInsetsConvertFromScreenPixelsToCanvasUnits()
    {
        Vector2 screen = new Vector2(2000f, 1000f);
        Vector2 root = new Vector2(1000f, 500f);
        StageFit.Insets insets = StageFit.GetSafeAreaInsets(root, screen, new Rect(100f, 40f, 1700f, 940f));

        Assert.AreEqual(50f, insets.Left, 0.001f);
        Assert.AreEqual(100f, insets.Right, 0.001f);
        Assert.AreEqual(20f, insets.Bottom, 0.001f);
        Assert.AreEqual(10f, insets.Top, 0.001f);
    }

    [Test]
    public void CanvasSizeMatchesTheCanvasScalerAtTheReferenceResolution()
    {
        Vector2 size = StageFit.CanvasSizeFor(Reference, Reference, Match);
        Assert.AreEqual(Reference.x, size.x, 0.01f);
        Assert.AreEqual(Reference.y, size.y, 0.01f);
    }

    private static void AssertFits(Device device, bool fillsViewport)
    {
        Vector2 root = StageFit.CanvasSizeFor(device.Screen, Reference, Match);
        StageFit.Result fit = StageFit.Compute(root, device.Screen, device.SafeArea, Stage, SafePadding, fillsViewport);

        float halfWidth = Stage.x * fit.Scale * 0.5f;
        float halfHeight = Stage.y * fit.Scale * 0.5f;
        float stageLeft = root.x * 0.5f + fit.Offset.x - halfWidth;
        float stageRight = root.x * 0.5f + fit.Offset.x + halfWidth;
        float stageBottom = root.y * 0.5f + fit.Offset.y - halfHeight;
        float stageTop = root.y * 0.5f + fit.Offset.y + halfHeight;

        TestContext.WriteLine(
            $"{device.Name}: canvas {root.x:0}x{root.y:0}, scale {fit.Scale:0.000}, " +
            $"stage {Stage.x * fit.Scale:0}x{Stage.y * fit.Scale:0}, insets L{fit.Insets.Left:0} R{fit.Insets.Right:0} B{fit.Insets.Bottom:0} T{fit.Insets.Top:0}");

        const float tolerance = 0.5f;
        Assert.Greater(fit.Scale, StageFit.MinScale, $"{device.Name}: stage would be clamped to the minimum scale and overflow");
        Assert.GreaterOrEqual(stageLeft, fit.Insets.Left - tolerance, $"{device.Name}: stage crosses the left safe edge");
        Assert.LessOrEqual(stageRight, root.x - fit.Insets.Right + tolerance, $"{device.Name}: stage crosses the right safe edge");
        Assert.GreaterOrEqual(stageBottom, fit.Insets.Bottom - tolerance, $"{device.Name}: stage crosses the bottom safe edge");
        Assert.LessOrEqual(stageTop, root.y - fit.Insets.Top + tolerance, $"{device.Name}: stage crosses the top safe edge");
    }
}
