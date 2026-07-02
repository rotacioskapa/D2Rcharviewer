using D2RCharViewer.Core.Models;

namespace D2RCharViewer.Core.Services;

public interface ISaveGameWatcherService
{
    event EventHandler<SaveGameEventArgs>? SaveGameChanged;
    
    void Start(string? savePath = null);
    void Stop();
    bool IsMonitoring { get; }
    string? CurrentMonitoringPath { get; }
}