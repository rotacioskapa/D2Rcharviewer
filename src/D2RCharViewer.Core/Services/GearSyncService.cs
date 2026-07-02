using Serilog;
using System.Text.Json;

namespace D2RCharViewer.Core.Services;

public class GearSyncService : IGearSyncService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _gearSyncUrl;

    public bool IsConfigured => !string.IsNullOrEmpty(_gearSyncUrl);

    public GearSyncService(ILogger logger, string gearSyncUrl = "https://d2armory.littlebluefrog.nl")
    {
        _logger = logger;
        _gearSyncUrl = gearSyncUrl;
        _httpClient = new HttpClient();
    }

    public async Task<bool> SyncGearAsync(string characterName, string gearData)
    {
        if (!IsConfigured)
        {
            _logger.Warning("Gear sync service is not configured");
            return false;
        }

        try
        {
            var payload = new { characterName, gearData };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_gearSyncUrl}/api/sync", content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.Information("Successfully synced gear for character: {CharacterName}", characterName);
                return true;
            }
            else
            {
                _logger.Warning("Gear sync failed for character {CharacterName}: {StatusCode}", characterName, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error syncing gear for character: {CharacterName}", characterName);
            return false;
        }
    }

    public async Task<string?> GetGearAsync(string characterName)
    {
        if (!IsConfigured)
        {
            _logger.Warning("Gear sync service is not configured");
            return null;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_gearSyncUrl}/api/gear/{characterName}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.Information("Successfully retrieved gear for character: {CharacterName}", characterName);
                return content;
            }
            else
            {
                _logger.Warning("Failed to retrieve gear for character {CharacterName}: {StatusCode}", characterName, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving gear for character: {CharacterName}", characterName);
            return null;
        }
    }
}