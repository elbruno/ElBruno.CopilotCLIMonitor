# Dolph — History

**Agent:** Dolph (Backend/Core Dev)  
**Project:** ElBruno.CopilotCLIMonitor  
**Tech Stack:** .NET 10, C#, xUnit, NuGet Global Tool  
**Requestor:** Copilot  

## Day 1 Context (2026-05-13)

**Project State:**
- Version: 0.1.0 (alpha)
- Core library exists but incomplete (services, models, interfaces exist; gaps in settings, IPC, persistence)
- CLI tool packaged as Global Tool (working baseline)
- Test coverage: 13 xUnit files (mostly Core/CLI layers; WPF untested)

**Your Key Files:**
- `src/ElBruno.CopilotCLIMonitor.Core/` — shared library (where most of your work lives)
- `src/ElBruno.CopilotCLIMonitor.Cli/` — CLI tool entry point
- `tests/` — xUnit suite (coordinate with River for coverage goals)

**Integration Points:**
- Chevy consumes your Core APIs for WPF app
- River writes tests for your implementations
- Martin approves architectural changes
- Sish packages your binaries for NuGet + MSI

## Learnings

### Wave 1 Foundation (2026-05-13) — PR: core/wave1-foundation

**Bug fixed — EventType IPC round-trip (critical):**
- `HttpEventNotifier` was converting `EventType.TaskCompleted` → `"taskcompleted"` via `.ToString().ToLowerInvariant()`
- `MonitorEvent.Parse` expected hyphenated strings (`"task-completed"`), so ALL events arrived as `EventType.Unknown` in the WPF app
- Fix: Added `EventTypeExtensions.ToEventString()` in `Core/Models/` — canonical hyphenated serialization. `HttpEventNotifier` now calls `.ToEventString()`.

**New Core APIs (Chevy and River can use these now):**

| Symbol | Where | Purpose |
|---|---|---|
| `IEventStore` | `Core/Interfaces/IEventStore.cs` | Mockable store contract; `Add`, `Clear`, `Recent` |
| `EventStore : IEventStore` | `Core/Services/EventStore.cs` | Now injectable; configurable `capacity` ctor param |
| `IIpcServer` | `Core/Interfaces/IIpcServer.cs` | `Start()`, `Stop()`, `EventReceived` event; WPF `IpcServer` implements it |
| `CoreSettings` | `Core/Infrastructure/CoreSettings.cs` | Record with `IpcPort`, `EventStoreCapacity`, `IpcTimeoutSeconds`; `CoreSettings.Default` for zero-config |
| `ServiceCollectionExtensions` | `Core/Infrastructure/ServiceCollectionExtensions.cs` | `AddCopilotCLIMonitorCore(settings?)` — registers all Core services into any `IServiceCollection` |
| `EventTypeExtensions.ToEventString()` | `Core/Models/EventTypeExtensions.cs` | Enum → hyphenated string; round-trip safe with `MonitorEvent.Parse` |
| `FakeEventStore` | `tests/Fakes/FakeEventStore.cs` | Test double for `IEventStore`; available to River immediately |

**WPF cleanup:**
- Removed duplicate `Services/EventStore.cs` from WPF project; `App.xaml.cs` and `DashboardWindow.xaml.cs` now reference `Core.Services.EventStore` directly
- `IpcServer` now implements `IIpcServer` — River can add integration tests for the HTTP server

**NuGet additions to Core:**
- `Microsoft.Extensions.DependencyInjection.Abstractions 10.0.0`
- `Microsoft.Extensions.Logging.Abstractions 10.0.0` (ready for structured logging wiring in a follow-up PR)

**Test count:** 80 → 103 (added `EventTypeExtensionsTests`, `CoreSettingsTests`, `FakeEventStore`)

**Still to do (next PRs):**
- Wire `ILogger<T>` into `HttpIpcClient`, `HookInstaller`, `RepositoryDetector`
- Persist events to disk (JSON) for dashboard reload after restart
- `IpcServer` into Core as a non-WPF implementation (needed for cross-platform CLI tests)
