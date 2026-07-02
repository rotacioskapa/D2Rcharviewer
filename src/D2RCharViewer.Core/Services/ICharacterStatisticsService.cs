using D2RCharViewer.Core.Models;

namespace D2RCharViewer.Core.Services;

public interface ICharacterStatisticsService
{
    Task<DisplayStats?> CalculateStatsAsync(string savePath);
    DisplayStats? GetCachedStats();
    void ClearCache();
}