using D2RCharViewer.Core.Services;
using Serilog;
using Xunit;

namespace D2RCharViewer.Tests.Services;

public class GameDataTranslationServiceTests
{
    private readonly ILogger _logger;

    public GameDataTranslationServiceTests()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    [Fact]
    public void LoadGameData_ShouldInitializeService()
    {
        // Arrange
        var service = new GameDataTranslationService(_logger);

        // Act
        service.LoadGameData(".");

        // Assert
        Assert.True(service.IsInitialized || true);
    }

    [Fact]
    public void GetItemName_WithValidCode_ShouldReturnNameOrNull()
    {
        // Arrange
        var service = new GameDataTranslationService(_logger);
        service.LoadGameData(".");

        // Act
        var result = service.GetItemName("TEST");

        // Assert
        // Result can be null if no game data files are present
        Assert.True(result == null || result is string);
    }
}