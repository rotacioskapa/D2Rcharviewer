using D2RCharViewer.Core.Models;
using D2RCharViewer.Core.Services;
using Prism.Mvvm;
using Serilog;

namespace D2RCharViewer.UI;

public class MainWindowViewModel : BindableBase
{
    private readonly ICharacterStatisticsService _statisticsService;
    private readonly ISaveGameWatcherService _watcherService;
    private readonly ILogger _logger;

    private string _characterName = "No Character Loaded";
    private string _characterClass = "N/A";
    private int _level;
    private long _experience;
    private string _statusMessage = "Initializing...";
    private DateTime _lastUpdate = DateTime.Now;
    private DisplayStats? _displayStats;

    public string CharacterName
    {
        get => _characterName;
        set => SetProperty(ref _characterName, value);
    }

    public string CharacterClass
    {
        get => _characterClass;
        set => SetProperty(ref _characterClass, value);
    }

    public int Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public long Experience
    {
        get => _experience;
        set => SetProperty(ref _experience, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public DateTime LastUpdate
    {
        get => _lastUpdate;
        set => SetProperty(ref _lastUpdate, value);
    }

    public DisplayStats? DisplayStats
    {
        get => _displayStats;
        set => SetProperty(ref _displayStats, value);
    }

    public MainWindowViewModel(
        ICharacterStatisticsService statisticsService,
        ISaveGameWatcherService watcherService,
        ILogger logger)
    {
        _statisticsService = statisticsService;
        _watcherService = watcherService;
        _logger = logger;

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            StatusMessage = "Starting save game watcher...";
            _watcherService.SaveGameChanged += OnSaveGameChanged;
            _watcherService.Start();
            StatusMessage = "Monitoring save games...";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error initializing application");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private async void OnSaveGameChanged(object? sender, SaveGameEventArgs e)
    {
        try
        {
            StatusMessage = $"Loading save game: {e.FileName}";
            _logger.Information("Save game changed: {FileName}", e.FileName);

            var stats = await _statisticsService.CalculateStatsAsync(e.SaveGamePath);
            if (stats != null)
            {
                CharacterName = stats.CharacterName;
                CharacterClass = stats.CharacterClass;
                Level = stats.Level;
                Experience = stats.Experience;
                DisplayStats = stats;
                LastUpdate = DateTime.Now;
                StatusMessage = $"Loaded character: {stats.CharacterName}";
            }
            else
            {
                StatusMessage = "Failed to load save game";
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing save game change");
            StatusMessage = $"Error: {ex.Message}";
        }
    }
}