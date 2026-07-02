namespace D2RCharViewer.Core.Models;

public static class GameConstants
{
    public const string CharacterClassAmazon = "Amazon";
    public const string CharacterClassSorceress = "Sorceress";
    public const string CharacterClassNecromancer = "Necromancer";
    public const string CharacterClassPaladin = "Paladin";
    public const string CharacterClassDruid = "Druid";
    public const string CharacterClassAssassin = "Assassin";
    public const string CharacterClassBarbarian = "Barbarian";

    public const int MaxLevel = 99;
    public const int MaxResistance = 75;
    public const int MinResistance = -100;

    public static readonly Dictionary<int, string> ClassNames = new()
    {
        { 0, CharacterClassAmazon },
        { 1, CharacterClassSorceress },
        { 2, CharacterClassNecromancer },
        { 3, CharacterClassPaladin },
        { 4, CharacterClassDruid },
        { 5, CharacterClassAssassin },
        { 6, CharacterClassBarbarian }
    };

    public static readonly Dictionary<string, int> DifficultyLevels = new()
    {
        { "Normal", 0 },
        { "Nightmare", 1 },
        { "Hell", 2 }
    };
}