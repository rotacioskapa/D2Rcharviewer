namespace D2RCharViewer.Core.Models;

public class SaveGameEventArgs : EventArgs
{
    public required string SaveGamePath { get; init; }
    public required string FileName { get; init; }
    public DateTime DetectedTime { get; init; } = DateTime.Now;
}