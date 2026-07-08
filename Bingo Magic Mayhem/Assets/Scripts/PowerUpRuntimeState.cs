using System.Collections.Generic;

public sealed class PowerUpRuntimeState
{
    private readonly HashSet<string> clairvoyanceAlertedCells = new HashSet<string>();

    public bool ClairvoyanceActive { get; set; }

    public void ResetRound()
    {
        clairvoyanceAlertedCells.Clear();
    }

    public void AlertCell(string cellKey)
    {
        clairvoyanceAlertedCells.Add(cellKey);
    }

    public void ClearCellAlert(string cellKey)
    {
        clairvoyanceAlertedCells.Remove(cellKey);
    }

    public bool IsCellAlerted(string cellKey)
    {
        return clairvoyanceAlertedCells.Contains(cellKey);
    }
}
