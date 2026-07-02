namespace D2RCharViewer.Core.Models;

public class ConfigOptions
{
    public string? SaveGameLocation { get; set; }
    public int SaveGamePollInterval { get; set; } = 5000;
    public int UiRefreshInterval { get; set; } = 5000;
    public bool GearSyncEnabled { get; set; } = false;
    public string GearSyncUrl { get; set; } = "https://d2armory.littlebluefrog.nl";
    public string LogLevel { get; set; } = "Information";
    public string LogFilePath { get; set; } = "logs/d2rcharviewer.log";
}