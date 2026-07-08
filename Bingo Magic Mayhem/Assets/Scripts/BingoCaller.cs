using System.Collections.Generic;
using UnityEngine;

public sealed class BingoCaller
{
    public readonly List<int> History = new List<int>();
    public readonly HashSet<int> CalledNumbers = new HashSet<int>();

    private readonly List<int> callPool = new List<int>();

    public int RemainingCount => callPool.Count;

    public void Reset()
    {
        callPool.Clear();
        History.Clear();
        CalledNumbers.Clear();

        for (int value = 1; value <= 75; value++)
        {
            callPool.Add(value);
        }
    }

    public bool TryCallNext(out int calledNumber)
    {
        calledNumber = 0;
        if (callPool.Count == 0)
        {
            return false;
        }

        int index = Random.Range(0, callPool.Count);
        calledNumber = callPool[index];
        callPool.RemoveAt(index);
        History.Insert(0, calledNumber);
        CalledNumbers.Add(calledNumber);
        return true;
    }

    public static string GetBingoLetter(int number)
    {
        if (number <= 15) return "B";
        if (number <= 30) return "I";
        if (number <= 45) return "N";
        if (number <= 60) return "G";
        return "O";
    }
}
