using System.Text;
using Serilog;

namespace D2RCharViewer.Parsers.Binary;

/// <summary>
/// Parses Diablo II: Resurrected D2S save game files
/// </summary>
public class D2SFileParser
{
    private readonly ILogger _logger;

    public D2SFileParser(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses a D2S save file
    /// </summary>
    public D2SSaveFile? Parse(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.Warning("Save file not found: {FilePath}", filePath);
                return null;
            }

            using (var stream = File.OpenRead(filePath))
            using (var reader = new BinaryReaderEx(stream, Encoding.ASCII))
            {
                return ParseSaveFile(reader);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error parsing D2S file: {FilePath}", filePath);
            return null;
        }
    }

    private D2SSaveFile ParseSaveFile(BinaryReaderEx reader)
    {
        var save = new D2SSaveFile();

        // Read file signature
        byte[] sigBytes = reader.ReadBytes(4);
        save.Signature = Encoding.ASCII.GetString(sigBytes);

        if (save.Signature != "D2XS" && save.Signature != "D2S\0")
        {
            throw new InvalidOperationException($"Invalid save file signature: {save.Signature}");
        }

        // Read file version
        save.Version = reader.ReadUInt32();
        _logger.Debug("D2S Version: {Version}", save.Version);

        // Read save length
        save.SaveLength = reader.ReadUInt32();

        // Read character name (16 bytes, null-terminated)
        byte[] nameBytes = reader.ReadBytes(16);
        save.CharacterName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
        _logger.Information("Parsing character: {CharacterName}", save.CharacterName);

        // Read status flags
        save.Status = (CharacterStatus)reader.ReadByte();
        save.IsExpansion = (save.Status & CharacterStatus.Expansion) != 0;
        save.IsHardcore = (save.Status & CharacterStatus.Hardcore) != 0;
        save.IsDead = (save.Status & CharacterStatus.Alive) == 0;

        // Read progression (skip 1 byte)
        save.Progression = reader.ReadByte();
        
        // Skip 1 byte
        reader.ReadByte();

        // Read class
        save.Class = (CharacterClass)reader.ReadByte();

        // Skip 2 bytes (unknown)
        reader.ReadBytes(2);

        // Read level
        save.Level = reader.ReadByte();

        // Skip 1 byte
        reader.ReadByte();

        // Read creation time (UNIX timestamp)
        save.CreationTime = reader.ReadUInt32();

        // Read last played time
        save.LastPlayedTime = reader.ReadUInt32();

        // Skip 4 bytes
        reader.ReadBytes(4);

        // Read time spent (in seconds)
        save.TimeSpent = reader.ReadUInt32();

        // Skip 4 bytes
        reader.ReadBytes(4);

        // Read last played time for Resurrected (if version supports it)
        if (save.Version >= 0x60)
        {
            save.LastPlayedTimeResurrected = reader.ReadUInt32();
        }

        // Parse character attributes
        ParseAttributes(reader, save);

        // Parse skills
        ParseSkills(reader, save);

        // Parse items
        ParseItems(reader, save);

        // Parse quests
        ParseQuests(reader, save);

        // Parse waypoints
        ParseWaypoints(reader, save);

        // Parse mercenary
        ParseMercenary(reader, save);

        _logger.Information("Successfully parsed D2S file: {Character} (Level {Level}, Class {Class})",
            save.CharacterName, save.Level, save.Class);

        return save;
    }

    private void ParseAttributes(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Look for the attributes section header "gf"
        byte b1 = reader.ReadByte();
        byte b2 = reader.ReadByte();

        while (b1 != 'g' || b2 != 'f')
        {
            b1 = b2;
            b2 = reader.ReadByte();
        }

        // Read attributes (each stat is 10 bits)
        int bitOffset = 0;
        var statReader = new BinaryReaderEx(new MemoryStream());

        // Strength
        save.Stats.Strength = reader.ReadUInt16();
        // Dexterity
        save.Stats.Dexterity = reader.ReadUInt16();
        // Vitality
        save.Stats.Vitality = reader.ReadUInt16();
        // Energy
        save.Stats.Energy = reader.ReadUInt16();

        _logger.Debug("Parsed attributes - STR: {Str}, DEX: {Dex}, VIT: {Vit}, ENE: {Ene}",
            save.Stats.Strength, save.Stats.Dexterity, save.Stats.Vitality, save.Stats.Energy);
    }

    private void ParseSkills(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Look for skills section header "if"
        byte b1 = reader.PeekChar();
        if (b1 == -1) return;

        byte b2 = reader.PeekChar();
        
        // Skills section is complex, for now we'll skip detailed parsing
        _logger.Debug("Skills section parsing deferred");
    }

    private void ParseItems(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Items section - complex structure
        // For now, we'll skip detailed parsing
        _logger.Debug("Items section parsing deferred");
    }

    private void ParseQuests(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Look for quests section header "qu"
        try
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();

            while ((b1 != 'q' || b2 != 'u') && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                b1 = b2;
                b2 = reader.ReadByte();
            }

            if (b1 == 'q' && b2 == 'u')
            {
                _logger.Debug("Found quests section");
                // Quest data is typically 6 bytes per difficulty, 3 difficulties = 18 bytes
                // Each byte represents 8 quests (bitfield)
                // Simplified parsing
            }
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Error parsing quests section");
        }
    }

    private void ParseWaypoints(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Look for waypoints section header "wp"
        try
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();

            while ((b1 != 'w' || b2 != 'p') && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                b1 = b2;
                b2 = reader.ReadByte();
            }

            if (b1 == 'w' && b2 == 'p')
            {
                _logger.Debug("Found waypoints section");
                // Waypoint data: 5 acts * 9 waypoints per act = 45 waypoints
                // Stored as bitfields
            }
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Error parsing waypoints section");
        }
    }

    private void ParseMercenary(BinaryReaderEx reader, D2SSaveFile save)
    {
        // Look for mercenary section header "gm"
        try
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();

            while ((b1 != 'g' || b2 != 'm') && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                b1 = b2;
                b2 = reader.ReadByte();
            }

            if (b1 == 'g' && b2 == 'm')
            {
                save.Mercenary.IsActive = reader.ReadUInt16() != 0;
                if (save.Mercenary.IsActive)
                {
                    save.Mercenary.MercType = reader.ReadUInt16();
                    save.Mercenary.Experience = reader.ReadUInt32();
                    save.Mercenary.Status = reader.ReadUInt16();
                    _logger.Debug("Mercenary found - Type: {Type}, Exp: {Exp}",
                        save.Mercenary.MercType, save.Mercenary.Experience);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Debug(ex, "Error parsing mercenary section");
        }
    }
}
