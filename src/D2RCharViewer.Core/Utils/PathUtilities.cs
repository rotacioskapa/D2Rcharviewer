using Serilog;

namespace D2RCharViewer.Core.Utils;

public static class PathUtilities
{
    /// <summary>
    /// Gets the default Diablo II: Resurrected save game path.
    /// </summary>
    public static string? GetDefaultSaveGamePath()
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

    /// <summary>
    /// Gets the application data directory for configuration and logs.
    /// </summary>
    public static string GetApplicationDataPath()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "D2RCharViewer");
        
        Directory.CreateDirectory(appDataPath);
        return appDataPath;
    }

    /// <summary>
    /// Gets the logs directory path.
    /// </summary>
    public static string GetLogsPath()
    {
        var logsPath = Path.Combine(GetApplicationDataPath(), "logs");
        Directory.CreateDirectory(logsPath);
        return logsPath;
    }
}