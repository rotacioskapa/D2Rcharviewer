using D2RCharViewer.Core.Models;
using D2RCharViewer.Parsers.Binary;
using Serilog;

namespace D2RCharViewer.Core.Services;

public class CharacterStatisticsService : ICharacterStatisticsService
{
    private readonly ILogger _logger;
    private readonly IGameDataTranslationService _translationService;
    private readonly D2SFileParser _parser;
    private DisplayStats? _cachedStats;

    public CharacterStatisticsService(ILogger logger, IGameDataTranslationService translationService)
    {
        _logger = logger;
        _translationService = translationService;
        _parser = new D2SFileParser(logger);
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

            // Parse the D2S file
            var d2sData = await Task.Run(() => _parser.Parse(savePath));
            if (d2sData == null)
            {
                _logger.Warning("Failed to parse D2S file: {SavePath}", savePath);
                return null;
            }

            // Convert parsed data to DisplayStats
            var stats = ConvertToDisplayStats(d2sData);
            _cachedStats = stats;

            _logger.Information("Calculated stats for character: {CharacterName} (Level {Level})",
                stats.CharacterName, stats.Level);

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

    private DisplayStats ConvertToDisplayStats(D2SSaveFile d2sData)
    {
        var className = GetClassName(d2sData.Class);
        var resistances = new Resistances
        {
            Fire = 0,    // TODO: Parse from item stats
            Cold = 0,
            Lightning = 0,
            Poison = 0
        };

        var breakpoints = new Breakpoints
        {
            CastRate = 0,    // TODO: Calculate from stats
            AttackSpeed = 0,
            BlockRate = 0,
            HitRecovery = 0
        };

        var stats = new DisplayStats
        {
            CharacterName = d2sData.CharacterName,
            CharacterClass = className,
            Level = d2sData.Level,
            Experience = (int)(d2sData.Stats.Experience & 0xFFFFFFFF),
            Life = CalculateLife(d2sData),
            MaxLife = CalculateMaxLife(d2sData),
            Mana = CalculateMana(d2sData),
            MaxMana = CalculateMaxMana(d2sData),
            Attributes = new DisplayAttributes
            {
                Strength = (int)d2sData.Stats.Strength,
                Dexterity = (int)d2sData.Stats.Dexterity,
                Vitality = (int)d2sData.Stats.Vitality,
                Energy = (int)d2sData.Stats.Energy
            },
            Resistances = resistances,
            Breakpoints = breakpoints,
            ChronicleStats = new ChronicleStats
            {
                MonstersKilled = 0,      // TODO: Parse from chronicle data
                ItemsPickedUp = 0,
                NormalMonsters = 0,
                ChampionMonsters = 0,
                UniqueMonsters = 0
            },
            IsAlive = !d2sData.IsDead,
            LastUpdated = DateTime.Now
        };

        return stats;
    }

    private string GetClassName(CharacterClass characterClass)
    {
        return characterClass switch
        {
            CharacterClass.Amazon => "Amazon",
            CharacterClass.Sorceress => "Sorceress",
            CharacterClass.Necromancer => "Necromancer",
            CharacterClass.Paladin => "Paladin",
            CharacterClass.Druid => "Druid",
            CharacterClass.Assassin => "Assassin",
            CharacterClass.Barbarian => "Barbarian",
            _ => "Unknown"
        };
    }

    private int CalculateLife(D2SSaveFile d2sData)
    {
        // Life is stored as: (base life + (vitality - base vitality) * bonus) / 256
        // Simplified calculation
        var baseLife = GetBaseLife(d2sData.Class);
        var vitalityBonus = ((int)d2sData.Stats.Vitality - 10) / 4; // Rough approximation
        return (baseLife + vitalityBonus) / 256;
    }

    private int CalculateMaxLife(D2SSaveFile d2sData)
    {
        var baseLife = GetBaseLife(d2sData.Class);
        var vitalityBonus = ((int)d2sData.Stats.Vitality - 10) / 4;
        return (baseLife + vitalityBonus) / 256;
    }

    private int CalculateMana(D2SSaveFile d2sData)
    {
        var baseMana = GetBaseMana(d2sData.Class);
        var energyBonus = ((int)d2sData.Stats.Energy - 10) / 2;
        return (baseMana + energyBonus) / 256;
    }

    private int CalculateMaxMana(D2SSaveFile d2sData)
    {
        var baseMana = GetBaseMana(d2sData.Class);
        var energyBonus = ((int)d2sData.Stats.Energy - 10) / 2;
        return (baseMana + energyBonus) / 256;
    }

    private int GetBaseLife(CharacterClass characterClass)
    {
        return characterClass switch
        {
            CharacterClass.Amazon => 55,
            CharacterClass.Sorceress => 35,
            CharacterClass.Necromancer => 45,
            CharacterClass.Paladin => 55,
            CharacterClass.Druid => 55,
            CharacterClass.Assassin => 45,
            CharacterClass.Barbarian => 65,
            _ => 50
        };
    }

    private int GetBaseMana(CharacterClass characterClass)
    {
        return characterClass switch
        {
            CharacterClass.Amazon => 25,
            CharacterClass.Sorceress => 35,
            CharacterClass.Necromancer => 25,
            CharacterClass.Paladin => 25,
            CharacterClass.Druid => 30,
            CharacterClass.Assassin => 25,
            CharacterClass.Barbarian => 15,
            _ => 25
        };
    }
}