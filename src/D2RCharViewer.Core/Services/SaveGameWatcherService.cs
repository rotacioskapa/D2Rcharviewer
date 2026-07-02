using D2RCharViewer.Core.Models;
using Serilog;

namespace D2RCharViewer.Core.Services;

public class SaveGameWatcherService : ISaveGameWatcherService, IDisposable
{
    private FileSystemWatcher? _watcher;
    private readonly ILogger _logger;
    private DateTime _lastEventTime = DateTime.MinValue;
    private const int DebounceMilliseconds = 500;

    public event EventHandler<SaveGameEventArgs>? SaveGameChanged;
    public bool IsMonitoring { get; private set; }
    public string? CurrentMonitoringPath { get; private set; }

    public SaveGameWatcherService(ILogger logger)
    {
        _logger = logger;
    }

    public void Start(string? savePath = null)
    {
        try
        {
            var pathToMonitor = savePath ?? GetDefaultSaveGamePath();
            
            if (string.IsNullOrEmpty(pathToMonitor) || !Directory.Exists(pathToMonitor))
            {
                _logger.Warning("Save game path not found: {SavePath}", pathToMonitor);
                return;
            }

            CurrentMonitoringPath = pathToMonitor;
            _watcher = new FileSystemWatcher(pathToMonitor)
            {
                Filter = "*.d2s",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Changed += OnFileChanged;
            _watcher.EnableRaisingEvents = true;
            IsMonitoring = true;

            _logger.Information("Save game watcher started for path: {Path}", pathToMonitor);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error starting save game watcher");
            throw;
        }
    }

    public void Stop()
    {
        try
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }

            IsMonitoring = false;
            _logger.Information("Save game watcher stopped");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error stopping save game watcher");
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var now = DateTime.Now;
        if ((now - _lastEventTime).TotalMilliseconds < DebounceMilliseconds)
        {
            return;
        }

        _lastEventTime = now;

        try
        {
            _logger.Debug("Save game file changed: {FileName}", e.Name);
            SaveGameChanged?.Invoke(this, new SaveGameEventArgs
            {
                SaveGamePath = e.FullPath,
                FileName = e.Name ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling save game change for file: {FileName}", e.Name);
        }
    }

    private static string? GetDefaultSaveGamePath()
    {
        try
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var d2rPath = Path.Combine(userProfile, "Saved Games", "Diablo II Resurrected");
            return Directory.Exists(d2rPath) ? d2rPath : null;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Error detecting default save game path");
            return null;
        }
    }

    public void Dispose()
    {
        Stop();
        _watcher?.Dispose();
    }
}