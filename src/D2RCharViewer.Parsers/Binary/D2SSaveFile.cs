namespace D2RCharViewer.Parsers.Binary;

/// <summary>
/// Represents character attributes and stats from D2S file
/// </summary>
public class CharacterStats
{
    public uint Strength { get; set; }
    public uint Dexterity { get; set; }
    public uint Vitality { get; set; }
    public uint Energy { get; set; }
    public uint Life { get; set; }
    public uint MaxLife { get; set; }
    public uint Mana { get; set; }
    public uint MaxMana { get; set; }
    public uint Experience { get; set; }
    public uint Gold { get; set; }
    public uint GoldInStash { get; set; }
}

/// <summary>
/// Represents character quest progress
/// </summary>
public class QuestData
{
    public bool[] CompletedQuests { get; set; } = new bool[30];
}

/// <summary>
/// Represents waypoint locations discovered
/// </summary>
public class WaypointData
{
    public bool[][] Waypoints { get; set; } = new bool[5][]; // 5 acts

    public WaypointData()
    {
        for (int i = 0; i < 5; i++)
            Waypoints[i] = new bool[9]; // 9 waypoints per act
    }
}

/// <summary>
/// Represents mercenary information
/// </summary>
public class MercenaryData
{
    public bool IsActive { get; set; }
    public ushort MercType { get; set; }
    public uint Experience { get; set; }
    public ushort Status { get; set; }
}

/// <summary>
/// Represents character status flags
/// </summary>
[Flags]
public enum CharacterStatus : byte
{
    Alive = 0x01,
    Hardcore = 0x02,
    Expansion = 0x04,
    Ladder = 0x08,
    ScourgeExpansion = 0x10
}

/// <summary>
/// Represents character class
/// </summary>
public enum CharacterClass : byte
{
    Amazon = 0,
    Sorceress = 1,
    Necromancer = 2,
    Paladin = 3,
    Druid = 4,
    Assassin = 5,
    Barbarian = 6
}

/// <summary>
/// Complete parsed D2S save game data
/// </summary>
public class D2SSaveFile
{
    // Header
    public string Signature { get; set; } = string.Empty;
    public uint Version { get; set; }
    public uint SaveLength { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public CharacterStatus Status { get; set; }
    public byte Progression { get; set; }
    public CharacterClass Class { get; set; }
    public byte Level { get; set; }
    public uint CreationTime { get; set; }
    public uint LastPlayedTime { get; set; }
    public uint TimeSpent { get; set; }
    public uint LastPlayedTimeResurrected { get; set; }
    
    // Character Data
    public CharacterStats Stats { get; set; } = new();
    public QuestData Quests { get; set; } = new();
    public WaypointData Waypoints { get; set; } = new();
    public MercenaryData Mercenary { get; set; } = new();
    
    // Game Progress
    public byte Difficulty { get; set; }
    public byte CurrentAct { get; set; }
    public uint Experience { get; set; }
    public bool IsExpansion { get; set; }
    public bool IsHardcore { get; set; }
    public bool IsDead { get; set; }
}
