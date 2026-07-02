namespace D2RCharViewer.Core.Models;

public record DisplayStats
{
    public required string CharacterName { get; init; }
    public required string CharacterClass { get; init; }
    public required int Level { get; init; }
    public required int Experience { get; init; }
    public required int Life { get; init; }
    public required int MaxLife { get; init; }
    public required int Mana { get; init; }
    public required int MaxMana { get; init; }
    public required DisplayAttributes Attributes { get; init; }
    public required Resistances Resistances { get; init; }
    public required Breakpoints Breakpoints { get; init; }
    public required ChronicleStats ChronicleStats { get; init; }
    public required bool IsAlive { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.Now;
}

public record DisplayAttributes
{
    public required int Strength { get; init; }
    public required int Dexterity { get; init; }
    public required int Vitality { get; init; }
    public required int Energy { get; init; }
}

public record Resistances
{
    public required int Fire { get; init; }
    public required int Cold { get; init; }
    public required int Lightning { get; init; }
    public required int Poison { get; init; }
}

public record Breakpoints
{
    public required int CastRate { get; init; }
    public required int AttackSpeed { get; init; }
    public required int BlockRate { get; init; }
    public required int HitRecovery { get; init; }
}

public record ChronicleStats
{
    public required int MonstersKilled { get; init; }
    public required int ItemsPickedUp { get; init; }
    public required int NormalMonsters { get; init; }
    public required int ChampionMonsters { get; init; }
    public required int UniqueMonsters { get; init; }
}