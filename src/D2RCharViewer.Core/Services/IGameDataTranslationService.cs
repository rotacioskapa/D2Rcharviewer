namespace D2RCharViewer.Core.Services;

public interface IGameDataTranslationService
{
    void LoadGameData(string gameDataPath);
    string? GetItemName(string itemCode);
    string? GetItemDescription(string itemCode);
    bool IsInitialized { get; }
}