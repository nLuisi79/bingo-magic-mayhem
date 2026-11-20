using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// Regression: BingoPrototype's field initializers ran PlayerInventoryState's initializer, which read EconomyTuning and
// called Resources.Load during MonoBehaviour construction. Unity throws there, so the component came up half-built and
// LateUpdate threw NullReferenceException every frame (EnsureRuntimeState). Adding the component for real reproduces
// Unity's construction rules, unlike `new`.
public sealed class BingoPrototypeConstructionTests
{
    private GameObject host;

    [TearDown]
    public void TearDown()
    {
        if (host != null)
        {
            Object.DestroyImmediate(host);
        }

        EconomyTuning.OverrideForTests(null);
    }

    [Test]
    public void AddingTheComponentConstructsEveryServiceWithoutLoadingTuning()
    {
        EconomyTuning.OverrideForTests(null);
        int deferredBefore = EconomyTuning.DeferredLoadCount;

        host = new GameObject("BingoPrototypeConstructionTest");
        BingoPrototype prototype = host.AddComponent<BingoPrototype>();

        Assert.AreEqual(deferredBefore, EconomyTuning.DeferredLoadCount, "construction must not read the tuning Resource");

        // Edit mode does not call Awake on plain MonoBehaviours; run it the way Play mode would.
        typeof(BingoPrototype).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(prototype, null);
        Assert.AreEqual(deferredBefore, EconomyTuning.DeferredLoadCount, "Awake may read tuning normally");
        foreach (string fieldName in new[] { "inventory", "prototypeMultiplayerRuntimeProvider", "prototypeMultiplayerRuntime", "friendsState", "playerCards", "roundFlow", "rewards" })
        {
            FieldInfo field = typeof(BingoPrototype).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"field {fieldName} not found");
            Assert.IsNotNull(field.GetValue(prototype), $"{fieldName} was not constructed");
        }
    }
}
