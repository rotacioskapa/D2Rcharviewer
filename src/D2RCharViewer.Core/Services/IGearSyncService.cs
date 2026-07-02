namespace D2RCharViewer.Core.Services;

public interface IGearSyncService
{
    Task<bool> SyncGearAsync(string characterName, string gearData);
    Task<string?> GetGearAsync(string characterName);
    bool IsConfigured { get; }
}