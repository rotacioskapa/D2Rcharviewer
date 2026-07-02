# D2R Character Viewer - C# WPF Migration

This is a complete migration of the Diablo II: Resurrected Character Viewer from Java/Quarkus to C# WPF (Windows Presentation Foundation).

## Project Structure

### Core Layer (`D2RCharViewer.Core`)
- **Models**: Data structures (DisplayStats, GameConstants, ConfigOptions)
- **Services**: Business logic and data operations
  - `ISaveGameWatcherService` / `SaveGameWatcherService`: File system monitoring
  - `ICharacterStatisticsService` / `CharacterStatisticsService`: Stat calculations
  - `IGameDataTranslationService` / `GameDataTranslationService`: Game data loading
  - `IGearSyncService` / `GearSyncService`: D2Armory integration
- **Utils**: Helper functions (PathUtilities)

### Parser Layer (`D2RCharViewer.Parsers`)
- `SaveGameParser`: Parses D2S binary save game files
- **TODO**: Implement actual D2S format parsing or integrate existing C# parser library

### UI Layer (`D2RCharViewer.UI`)
- **Views**: XAML windows and user controls
  - `MainWindow.xaml`: Primary application window with tabs for Stats, Health, Chronicle
- **ViewModels**: MVVM pattern implementation
  - `MainWindowViewModel`: Binds UI to services
- **App.xaml**: Application configuration and dependency injection

### Tests (`D2RCharViewer.Tests`)
- Unit tests using xUnit and Moq

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code

### Building

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build D2RCharViewer.sln

# Run tests
dotnet test tests/D2RCharViewer.Tests/D2RCharViewer.Tests.csproj

# Run application
dotnet run --project src/D2RCharViewer.UI/D2RCharViewer.UI.csproj
```

## Configuration

Configuration is managed via `appsettings.json`:

```json
{
  "SaveGame": {
    "Location": null,  // Auto-detects if null
    "PollInterval": 5000
  },
  "GearSync": {
    "Enabled": false,
    "Url": "https://d2armory.littlebluefrog.nl"
  }
}
```

## Architecture

### Data Flow
1. `SaveGameWatcherService` monitors the D2R save game folder using `FileSystemWatcher`
2. File changes trigger `OnSaveGameChanged` event
3. `CharacterStatisticsService` processes the save file
4. `SaveGameParser` extracts binary data from .d2s files
5. Results bound to UI via `MainWindowViewModel` (MVVM pattern)
6. Optional gear sync via `GearSyncService` to D2Armory

### Service Dependencies
```
App.xaml.cs (Startup)
    ↓
Dependency Injection Container (IServiceProvider)
    ↓
├─ ISaveGameWatcherService → SaveGameWatcherService
├─ ICharacterStatisticsService → CharacterStatisticsService
├─ IGameDataTranslationService → GameDataTranslationService
├─ IGearSyncService → GearSyncService
└─ MainWindowViewModel → MainWindow (UI)
```

## Key Differences from Java Version

| Aspect | Java/Quarkus | C# WPF |
|--------|--------------|--------|
| **File Watching** | Polling loop | Native `FileSystemWatcher` |
| **REST API** | HTTP server on port 8080 | Direct in-process |
| **UI Rendering** | HTML templates | XAML binding |
| **Threading** | Reactive streams | async/await Tasks |
| **Configuration** | `.properties` files | JSON `appsettings.json` |
| **Packaging** | Native executable via Quarkus | .NET WPF exe |

## TODO/Implementation Notes

- [ ] Implement D2S save game binary parsing in `SaveGameParser`
  - Reference: https://github.com/Paladijn/d2rsavegameparser
  - Or integrate existing C# D2R parser if available
- [ ] Load game data files (tcbyitemname.txt, tcbyitemcode.txt)
- [ ] Implement full stat calculations (breakpoints, damage, etc.)
- [ ] Add settings/preferences UI window
- [ ] Implement character inventory display
- [ ] Add skill tree visualization
- [ ] Build standalone installer (WiX or NSIS)
- [ ] Cross-platform support (.NET MAUI alternative)

## Logging

Logs are written to `%APPDATA%/D2RCharViewer/logs/` using Serilog:
- Rolling daily log files
- Console output during development
- Configurable via appsettings.json

## Next Steps

1. **Implement D2S Parser**: The most critical component. Either:
   - Port the Java `d2rsavegameparser` library to C#
   - Find existing C# D2R save parser
   - Implement from binary format specification

2. **Testing**: Add integration tests that parse actual D2R save files

3. **UI Polish**: Add themes, settings dialog, about window

4. **Packaging**: Create installer for end users

## License

Apache License 2.0 (same as original Java project)
