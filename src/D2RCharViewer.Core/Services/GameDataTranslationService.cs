using Serilog;

namespace D2RCharViewer.Core.Services;

public class GameDataTranslationService : IGameDataTranslationService
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, string> _itemNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _itemCodes = new(StringComparer.OrdinalIgnoreCase);

    public bool IsInitialized { get; private set; }

    public GameDataTranslationService(ILogger logger)
    {
        _logger = logger;
    }

    public void LoadGameData(string gameDataPath)
    {
        try
        {
            var itemNamesPath = Path.Combine(gameDataPath, "tcbyitemname.txt");
            var itemCodesPath = Path.Combine(gameDataPath, "tcbyitemcode.txt");

            if (File.Exists(itemNamesPath))
            {
                LoadItemNamesFile(itemNamesPath);
            }

            if (File.Exists(itemCodesPath))
            {
                LoadItemCodesFile(itemCodesPath);
            }

            IsInitialized = true;
            _logger.Information("Game data translation service initialized with {ItemCount} items", _itemNames.Count);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading game data from path: {GameDataPath}", gameDataPath);
        }
    }

    private void LoadItemNamesFile(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    _itemNames[parts[0]] = parts[1];
                }
            }

            _logger.Debug("Loaded {ItemCount} item names from {FilePath}", _itemNames.Count, filePath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading item names file: {FilePath}", filePath);
        }
    }

    private void LoadItemCodesFile(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;

                var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    _itemCodes[parts[0]] = parts[1];
                }
            }

            _logger.Debug("Loaded {ItemCount} item codes from {FilePath}", _itemCodes.Count, filePath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading item codes file: {FilePath}", filePath);
        }
    }

    public string? GetItemName(string itemCode) =>
        _itemNames.TryGetValue(itemCode, out var name) ? name : null;

    public string? GetItemDescription(string itemCode) =>
        _itemCodes.TryGetValue(itemCode, out var description) ? description : null;
}