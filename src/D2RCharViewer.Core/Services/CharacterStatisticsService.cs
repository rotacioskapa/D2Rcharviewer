using D2RCharViewer.Core.Models;
using Serilog;

namespace D2RCharViewer.Core.Services;

public class CharacterStatisticsService : ICharacterStatisticsService
{
    private readonly ILogger _logger;
    private readonly IGameDataTranslationService _translationService;
    private DisplayStats? _cachedStats;

    public CharacterStatisticsService(ILogger logger, IGameDataTranslationService translationService)
    {
        _logger = logger;
        _translationService = translationService;
    }

    public async Task<DisplayStats?> CalculateStatsAsync(string savePath)
    {
        try
        {
            if (!File.Exists(savePath))
            {
                _logger.Warning("Save game file not found: {SavePath}", savePath);
                return null;
            }

            var stats = new DisplayStats
            {
                CharacterName = Path.GetFileNameWithoutExtension(savePath),
                CharacterClass = GameConstants.CharacterClassBarbarian,
                Level = 99,
                Experience = 0,
                Life = 100,
                MaxLife = 100,
                Mana = 50,
                MaxMana = 50,
                Attributes = new DisplayAttributes
                {
                    Strength = 10,
                    Dexterity = 10,
                    Vitality = 10,
                    Energy = 10
                },
                Resistances = new Resistances
                {
                    Fire = 0,
                    Cold = 0,
                    Lightning = 0,
                    Poison = 0
                },
                Breakpoints = new Breakpoints
                {
                    CastRate = 0,
                    AttackSpeed = 0,
                    BlockRate = 0,
                    HitRecovery = 0
                },
                ChronicleStats = new ChronicleStats
                {
                    MonstersKilled = 0,
                    ItemsPickedUp = 0,
                    NormalMonsters = 0,
                    ChampionMonsters = 0,
                    UniqueMonsters = 0
                },
                IsAlive = true
            };

            _cachedStats = stats;
            _logger.Information("Calculated stats for character: {CharacterName}", stats.CharacterName);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating statistics for save file: {SavePath}", savePath);
            return null;
        }
    }

    public DisplayStats? GetCachedStats() => _cachedStats;

    public void ClearCache() => _cachedStats = null;
}