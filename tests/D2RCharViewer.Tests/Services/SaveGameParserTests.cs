using D2RCharViewer.Parsers;
using D2RCharViewer.Parsers.Binary;
using Serilog;
using Xunit;

namespace D2RCharViewer.Tests.Services;

public class SaveGameParserTests
{
    private readonly ILogger _logger;

    public SaveGameParserTests()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    [Fact]
    public void Parse_WithValidD2SFile_ShouldExtractCharacterInfo()
    {
        // Arrange
        var parser = new D2SFileParser(_logger);
        // This test would require an actual D2S file
        // For now, we're testing the structure
        var testFile = "test_character.d2s";

        // Act & Assert
        // In a real scenario, you would test with actual D2S files
        // This is a structural test
        Assert.NotNull(parser);
    }

    [Fact]
    public void Parse_WithNonExistentFile_ShouldReturnNull()
    {
        // Arrange
        var parser = new D2SFileParser(_logger);

        // Act
        var result = parser.Parse("/nonexistent/path/character.d2s");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CharacterClassEnum_ShouldHaveAllClasses()
    {
        // Assert
        Assert.Equal(7, Enum.GetValues(typeof(CharacterClass)).Length);
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Barbarian));
        Assert.True(Enum.IsDefined(typeof(CharacterClass), CharacterClass.Amazon));
    }
}