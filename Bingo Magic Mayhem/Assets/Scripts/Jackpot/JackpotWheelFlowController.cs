using System;
using System.Collections.Generic;
using UnityEngine;

namespace BingoMagicMayhem.Jackpot
{
    public sealed class JackpotWheelSpinResult
    {
        public JackpotWheelSpinResult(string label, int manaAward, bool resetPot)
        {
            Label = label ?? "";
            ManaAward = manaAward;
            ResetPot = resetPot;
        }

        public string Label { get; }
        public int ManaAward { get; }
        public bool ResetPot { get; }
    }

    public sealed class JackpotWheelCollectResult
    {
        public JackpotWheelCollectResult(int collectedMana, int spinResultCount, bool resetPot)
        {
            CollectedMana = collectedMana;
            SpinResultCount = spinResultCount;
            ResetPot = resetPot;
        }

        public int CollectedMana { get; }
        public int SpinResultCount { get; }
        public bool ResetPot { get; }
    }

    /// <summary>
    /// Holds local jackpot wheelspin gameplay state so the prototype UI can render
    /// and animate against a smaller, behavior-focused runtime model.
    /// </summary>
    public sealed class JackpotWheelFlowController
    {
        private readonly List<JackpotWheelSpinResult> collectedResults = new List<JackpotWheelSpinResult>();

        public IReadOnlyList<JackpotWheelSpinResult> CollectedResults => collectedResults;
        public int CollectedMana { get; private set; }
        public int LastCollectedMana { get; private set; }
        public bool LastCollectResetPot { get; private set; }
        public JackpotWheelSpinResult LastSpinResult { get; private set; }
        public JackpotWheelSpinResult PendingSpinResult { get; private set; }
        public bool IsAnimating { get; private set; }
        public int AnimationSequence { get; private set; }
        public float Rotation { get; set; }
        public int TargetSegment { get; private set; }
        public bool CollectionConfirmed { get; private set; }
        public bool HasStackToCollect => CollectedMana > 0 && collectedResults.Count > 0;

        public bool CanSpin(int pendingSpins)
        {
            return pendingSpins > 0 && !IsAnimating;
        }

        public bool CanCollect(int pendingSpins)
        {
            return HasStackToCollect && pendingSpins <= 0 && !IsAnimating;
        }

        public void ClearLatestSpinResult()
        {
            LastSpinResult = null;
        }

        public bool TryBeginSpin(int pendingSpinsBeforeConsume, int targetSegment, JackpotWheelSpinResult pendingResult)
        {
            if (IsAnimating || pendingSpinsBeforeConsume <= 0 || pendingResult == null)
            {
                return false;
            }

            CollectionConfirmed = false;
            TargetSegment = targetSegment;
            PendingSpinResult = pendingResult;
            IsAnimating = true;
            AnimationSequence++;
            return true;
        }

        public void CompleteSpin()
        {
            IsAnimating = false;
            LastSpinResult = PendingSpinResult;
            PendingSpinResult = null;

            if (LastSpinResult == null)
            {
                return;
            }

            collectedResults.Add(LastSpinResult);
            CollectedMana += LastSpinResult.ManaAward;
        }

        public JackpotWheelCollectResult Collect()
        {
            if (!HasStackToCollect)
            {
                return null;
            }

            int collectedMana = CollectedMana;
            bool shouldResetPot = false;
            for (int index = 0; index < collectedResults.Count; index++)
            {
                shouldResetPot = shouldResetPot || collectedResults[index].ResetPot;
            }

            JackpotWheelCollectResult result = new JackpotWheelCollectResult(
                collectedMana,
                collectedResults.Count,
                shouldResetPot);

            LastCollectedMana = collectedMana;
            LastCollectResetPot = shouldResetPot;
            CollectionConfirmed = true;
            CollectedMana = 0;
            collectedResults.Clear();
            LastSpinResult = null;
            PendingSpinResult = null;
            IsAnimating = false;
            AnimationSequence++;
            return result;
        }

        public void ResetForReturnHome()
        {
            AnimationSequence++;
            CollectedMana = 0;
            LastCollectedMana = 0;
            LastCollectResetPot = false;
            CollectionConfirmed = false;
            collectedResults.Clear();
            LastSpinResult = null;
            PendingSpinResult = null;
            IsAnimating = false;
            TargetSegment = 0;
        }
    }

    /// <summary>
    /// Centralizes wheel segment mapping and prototype reward interpretation.
    /// Final jackpot odds/tuning remain open and should stay configurable later.
    /// </summary>
    public static class JackpotWheelRules
    {
        private static readonly float[] StandardMultipliers = { 0.2f, 0.3f, 0.4f, 0.6f, 0.8f, 1f, 1.25f, 1.5f };

        public static int GetSegmentCount()
        {
            return 11;
        }

        public static float GetRestingRotation(int targetSegment)
        {
            float angleStep = 360f / GetSegmentCount();
            return Mathf.Repeat(-(targetSegment * angleStep), 360f);
        }

        public static float GetDeltaToTarget(float startRotation, int targetSegment)
        {
            float targetMod = GetRestingRotation(targetSegment);
            float currentMod = Mathf.Repeat(startRotation, 360f);
            return Mathf.Repeat(targetMod - currentMod, 360f);
        }

        public static int GetSegmentForRoll(int roll)
        {
            if (roll < 18)
            {
                return 0;
            }

            if (roll < 32)
            {
                return 1;
            }

            if (roll < 46)
            {
                return 2;
            }

            if (roll < 58)
            {
                return 3;
            }

            if (roll < 69)
            {
                return 4;
            }

            if (roll < 79)
            {
                return 5;
            }

            if (roll < 87)
            {
                return 6;
            }

            if (roll < 94)
            {
                return 7;
            }

            if (roll < 97)
            {
                return 8;
            }

            if (roll < 99)
            {
                return 9;
            }

            return 10;
        }

        public static JackpotWheelSpinResult ResolveSpinResult(
            int segmentIndex,
            Func<float, int> standardValueResolver,
            int jackpotValue,
            int epicValue,
            int legendaryValue)
        {
            if (standardValueResolver == null)
            {
                throw new ArgumentNullException(nameof(standardValueResolver));
            }

            if (segmentIndex < 8)
            {
                int standardValue = GetStandardValue(segmentIndex, standardValueResolver);
                return new JackpotWheelSpinResult("STANDARD", standardValue, false);
            }

            if (segmentIndex == 8)
            {
                return new JackpotWheelSpinResult("JACKPOT", jackpotValue, true);
            }

            if (segmentIndex == 9)
            {
                return new JackpotWheelSpinResult("EPIC", epicValue, true);
            }

            return new JackpotWheelSpinResult("LEGENDARY", legendaryValue, true);
        }

        public static string GetSegmentLabel(int segmentIndex, Func<float, int> standardValueResolver)
        {
            if (segmentIndex < 8)
            {
                return GetStandardValue(segmentIndex, standardValueResolver).ToString("N0");
            }

            if (segmentIndex == 8)
            {
                return "JACKPOT";
            }

            if (segmentIndex == 9)
            {
                return "EPIC";
            }

            return "LEGENDARY";
        }

        public static int GetStandardValue(int segmentIndex, Func<float, int> standardValueResolver)
        {
            if (standardValueResolver == null)
            {
                throw new ArgumentNullException(nameof(standardValueResolver));
            }

            float multiplier = StandardMultipliers[Mathf.Clamp(segmentIndex, 0, StandardMultipliers.Length - 1)];
            return standardValueResolver(multiplier);
        }
    }
}
