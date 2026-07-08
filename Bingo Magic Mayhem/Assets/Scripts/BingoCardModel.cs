using System.Collections.Generic;
using UnityEngine;

public sealed class BingoCardModel
{
    public const int BoardSize = 5;

    private static readonly int[] ColumnMinimums = { 1, 16, 31, 46, 61 };

    public readonly int[,] Numbers = new int[BoardSize, BoardSize];
    public readonly bool[,] Marks = new bool[BoardSize, BoardSize];
    public readonly HashSet<string> WinningCells = new HashSet<string>();

    public int BingoCount { get; private set; }

    public BingoCardModel()
    {
        Generate();
    }

    public bool MarkIfCalled(int row, int column, HashSet<int> calledNumbers)
    {
        if (IsFree(row, column) || Marks[row, column])
        {
            return false;
        }

        int value = Numbers[row, column];
        if (!calledNumbers.Contains(value))
        {
            return false;
        }

        Marks[row, column] = true;
        return true;
    }

    public bool HasNumber(int value)
    {
        for (int row = 0; row < BoardSize; row++)
        {
            for (int column = 0; column < BoardSize; column++)
            {
                if (Numbers[row, column] == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int RefreshBingos(int jackpotTarget)
    {
        WinningCells.Clear();
        BingoCount = Mathf.Min(CountCompletedLines(), jackpotTarget);
        return BingoCount;
    }

    public static string GetCellKey(int row, int column)
    {
        return $"{row}:{column}";
    }

    public static bool IsFree(int row, int column)
    {
        return row == 2 && column == 2;
    }

    private void Generate()
    {
        for (int column = 0; column < BoardSize; column++)
        {
            List<int> available = new List<int>();
            for (int value = ColumnMinimums[column]; value < ColumnMinimums[column] + 15; value++)
            {
                available.Add(value);
            }

            for (int row = 0; row < BoardSize; row++)
            {
                int index = Random.Range(0, available.Count);
                Numbers[row, column] = available[index];
                available.RemoveAt(index);
                Marks[row, column] = false;
            }
        }

        Marks[2, 2] = true;
    }

    private int CountCompletedLines()
    {
        int completedLines = 0;

        for (int row = 0; row < BoardSize; row++)
        {
            bool complete = true;
            for (int column = 0; column < BoardSize; column++)
            {
                complete &= Marks[row, column];
            }

            if (complete)
            {
                completedLines++;
                for (int column = 0; column < BoardSize; column++)
                {
                    WinningCells.Add(GetCellKey(row, column));
                }
            }
        }

        for (int column = 0; column < BoardSize; column++)
        {
            bool complete = true;
            for (int row = 0; row < BoardSize; row++)
            {
                complete &= Marks[row, column];
            }

            if (complete)
            {
                completedLines++;
                for (int row = 0; row < BoardSize; row++)
                {
                    WinningCells.Add(GetCellKey(row, column));
                }
            }
        }

        bool leftToRight = true;
        bool rightToLeft = true;
        for (int index = 0; index < BoardSize; index++)
        {
            leftToRight &= Marks[index, index];
            rightToLeft &= Marks[index, BoardSize - 1 - index];
        }

        if (leftToRight)
        {
            completedLines++;
            for (int index = 0; index < BoardSize; index++)
            {
                WinningCells.Add(GetCellKey(index, index));
            }
        }

        if (rightToLeft)
        {
            completedLines++;
            for (int index = 0; index < BoardSize; index++)
            {
                WinningCells.Add(GetCellKey(index, BoardSize - 1 - index));
            }
        }

        return completedLines;
    }
}
