using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerCardSet
{
    public readonly List<int[,]> Numbers = new List<int[,]>();
    public readonly List<bool[,]> Marks = new List<bool[,]>();
    public readonly List<int> BingoCounts = new List<int>();
    public readonly List<int> PotionMultipliers = new List<int>();
    public readonly List<HashSet<string>> WinningCells = new List<HashSet<string>>();

    public int Count => Numbers.Count;

    public void Generate(int count)
    {
        Numbers.Clear();
        Marks.Clear();
        BingoCounts.Clear();
        PotionMultipliers.Clear();
        WinningCells.Clear();

        for (int index = 0; index < count; index++)
        {
            int[,] generatedNumbers = new int[BingoRoomRules.BoardSize, BingoRoomRules.BoardSize];
            bool[,] generatedMarks = new bool[BingoRoomRules.BoardSize, BingoRoomRules.BoardSize];
            FillCard(generatedNumbers, generatedMarks);
            Numbers.Add(generatedNumbers);
            Marks.Add(generatedMarks);
            BingoCounts.Add(0);
            PotionMultipliers.Add(Random.Range(1, 5));
            WinningCells.Add(new HashSet<string>());
        }
    }

    public bool HasAnyCards => Numbers.Count > 0 && Marks.Count > 0;

    public int GetNumber(int cardIndex, int row, int column)
    {
        return Numbers[cardIndex][row, column];
    }

    public bool IsMarked(int cardIndex, int row, int column)
    {
        return Marks[cardIndex][row, column];
    }

    public void Mark(int cardIndex, int row, int column)
    {
        Marks[cardIndex][row, column] = true;
    }

    public int GetBingoCount(int cardIndex)
    {
        return BingoCounts.Count > cardIndex ? BingoCounts[cardIndex] : 0;
    }

    public int GetPotionMultiplier(int cardIndex)
    {
        return PotionMultipliers.Count > cardIndex ? PotionMultipliers[cardIndex] : 1;
    }

    public bool IsWinningCell(int cardIndex, int row, int column)
    {
        return cardIndex >= 0
            && cardIndex < WinningCells.Count
            && WinningCells[cardIndex].Contains(GetCellKey(row, column));
    }

    public bool HasNumberOnAnyCard(int calledNumber)
    {
        foreach (int[,] card in Numbers)
        {
            for (int row = 0; row < BingoRoomRules.BoardSize; row++)
            {
                for (int column = 0; column < BingoRoomRules.BoardSize; column++)
                {
                    if (card[row, column] == calledNumber)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public int RefreshBingoProgress(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= Marks.Count)
        {
            return 0;
        }

        HashSet<string> winningCells = new HashSet<string>();
        int completedLines = CountCompletedLines(Marks[cardIndex], winningCells);
        BingoCounts[cardIndex] = Mathf.Min(completedLines, BingoRoomRules.JackpotBingosPerCard);
        WinningCells[cardIndex] = winningCells;
        return BingoCounts[cardIndex];
    }

    public int RefreshBlackoutProgress(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= Marks.Count)
        {
            return 0;
        }

        HashSet<string> winningCells = new HashSet<string>();
        bool complete = CountMarkedPlayable(cardIndex) >= BingoRoomRules.BlackoutPlayableSquares;
        if (complete)
        {
            for (int row = 0; row < BingoRoomRules.BoardSize; row++)
            {
                for (int column = 0; column < BingoRoomRules.BoardSize; column++)
                {
                    winningCells.Add(GetCellKey(row, column));
                }
            }
        }

        BingoCounts[cardIndex] = complete ? BingoRoomRules.JackpotBingosPerCard : 0;
        WinningCells[cardIndex] = winningCells;
        return BingoCounts[cardIndex];
    }

    public int CountMarkedPlayable(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= Marks.Count)
        {
            return 0;
        }

        int markedCount = 0;
        for (int row = 0; row < BingoRoomRules.BoardSize; row++)
        {
            for (int column = 0; column < BingoRoomRules.BoardSize; column++)
            {
                if (row == 2 && column == 2)
                {
                    continue;
                }

                if (Marks[cardIndex][row, column])
                {
                    markedCount++;
                }
            }
        }

        return markedCount;
    }

    public int GetTotalBingos()
    {
        int total = 0;
        for (int index = 0; index < BingoCounts.Count; index++)
        {
            total += BingoCounts[index];
        }

        return total;
    }

    public bool AllCardsReachedJackpot()
    {
        if (BingoCounts.Count == 0)
        {
            return false;
        }

        for (int index = 0; index < BingoCounts.Count; index++)
        {
            if (BingoCounts[index] < BingoRoomRules.JackpotBingosPerCard)
            {
                return false;
            }
        }

        return true;
    }

    public static string GetCellKey(int row, int column)
    {
        return $"{row}:{column}";
    }

    private static void FillCard(int[,] targetNumbers, bool[,] targetMarks)
    {
        int[] minByColumn = { 1, 16, 31, 46, 61 };

        for (int column = 0; column < BingoRoomRules.BoardSize; column++)
        {
            List<int> available = new List<int>();
            for (int value = minByColumn[column]; value < minByColumn[column] + 15; value++)
            {
                available.Add(value);
            }

            for (int row = 0; row < BingoRoomRules.BoardSize; row++)
            {
                int index = Random.Range(0, available.Count);
                targetNumbers[row, column] = available[index];
                available.RemoveAt(index);
                targetMarks[row, column] = false;
            }
        }

        targetMarks[2, 2] = true;
    }

    private static int CountCompletedLines(bool[,] marks, HashSet<string> winningCells)
    {
        int completedLines = 0;

        for (int row = 0; row < BingoRoomRules.BoardSize; row++)
        {
            bool complete = true;
            for (int column = 0; column < BingoRoomRules.BoardSize; column++)
            {
                complete &= marks[row, column];
            }

            if (complete)
            {
                completedLines++;
                for (int column = 0; column < BingoRoomRules.BoardSize; column++)
                {
                    winningCells.Add(GetCellKey(row, column));
                }
            }
        }

        for (int column = 0; column < BingoRoomRules.BoardSize; column++)
        {
            bool complete = true;
            for (int row = 0; row < BingoRoomRules.BoardSize; row++)
            {
                complete &= marks[row, column];
            }

            if (complete)
            {
                completedLines++;
                for (int row = 0; row < BingoRoomRules.BoardSize; row++)
                {
                    winningCells.Add(GetCellKey(row, column));
                }
            }
        }

        bool leftToRight = true;
        bool rightToLeft = true;
        for (int index = 0; index < BingoRoomRules.BoardSize; index++)
        {
            leftToRight &= marks[index, index];
            rightToLeft &= marks[index, BingoRoomRules.BoardSize - 1 - index];
        }

        if (leftToRight)
        {
            completedLines++;
            for (int index = 0; index < BingoRoomRules.BoardSize; index++)
            {
                winningCells.Add(GetCellKey(index, index));
            }
        }

        if (rightToLeft)
        {
            completedLines++;
            for (int index = 0; index < BingoRoomRules.BoardSize; index++)
            {
                winningCells.Add(GetCellKey(index, BingoRoomRules.BoardSize - 1 - index));
            }
        }

        return completedLines;
    }
}
