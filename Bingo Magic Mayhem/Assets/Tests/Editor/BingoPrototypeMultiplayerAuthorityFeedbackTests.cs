using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public sealed class BingoPrototypeMultiplayerAuthorityFeedbackTests
{
    [Test]
    public void ApplyPrototypeAuthorityClaimOutcome_WritesHeadlineAndDetailToStatusText()
    {
        GameObject root = new GameObject("bingo_prototype_test_root");
        try
        {
            BingoPrototype prototype = root.AddComponent<BingoPrototype>();
            Text statusText = CreateStatusText(root.transform);
            SetPrivateField(prototype, "statusText", statusText);

            BingoMagicMayhem.Multiplayer.MultiplayerAuthorityOutcomeModel outcome =
                new BingoMagicMayhem.Multiplayer.MultiplayerAuthorityOutcomeModel(
                    BingoMagicMayhem.Multiplayer.MultiplayerAuthorityFeedbackKind.Warning,
                    "Claim rejected.",
                    "Authority rejected the claim.",
                    shouldCelebrate: false,
                    shouldQueueJackpotHandoff: false);

            InvokePrivateMethod(prototype, "ApplyPrototypeAuthorityClaimOutcome", outcome);

            Assert.That(statusText.text, Is.EqualTo("Claim rejected. Authority rejected the claim."));
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    [Test]
    public void ApplyPrototypeRoundEndOutcome_WritesDetailToStatusText()
    {
        GameObject root = new GameObject("bingo_prototype_round_end_test_root");
        try
        {
            BingoPrototype prototype = root.AddComponent<BingoPrototype>();
            Text statusText = CreateStatusText(root.transform);
            SetPrivateField(prototype, "statusText", statusText);

            BingoMagicMayhem.Multiplayer.MultiplayerAuthorityOutcomeModel outcome =
                new BingoMagicMayhem.Multiplayer.MultiplayerAuthorityOutcomeModel(
                    BingoMagicMayhem.Multiplayer.MultiplayerAuthorityFeedbackKind.Informational,
                    "Round ended.",
                    "Max balls reached.",
                    shouldCelebrate: false,
                    shouldQueueJackpotHandoff: false);

            InvokePrivateMethod(prototype, "ApplyPrototypeRoundEndOutcome", outcome);

            Assert.That(statusText.text, Is.EqualTo("Max balls reached."));
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    private static Text CreateStatusText(Transform parent)
    {
        GameObject textObject = new GameObject("status_text");
        textObject.transform.SetParent(parent, false);
        return textObject.AddComponent<Text>();
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"Expected private field '{fieldName}' to exist.");
        field.SetValue(instance, value);
    }

    private static void InvokePrivateMethod(object instance, string methodName, object argument)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"Expected private method '{methodName}' to exist.");
        method.Invoke(instance, new[] { argument });
    }
}
