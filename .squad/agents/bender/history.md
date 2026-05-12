# Bender — History

## Session: 2026-05-12 — MVP Foundation

### What was built

**Core library (`ElBruno.CopilotCLIMonitor.Core`, `net10.0`)**
- `MonitorEvent` record with `EventType` enum and `Parse()` factory
- `IEventNotifier`, `IRepositoryDetector`, `IHookInstaller` interfaces
- `RepositoryDetector` service (uses `git branch --show-current` for safety in orphan repos)
- `HookInstaller` service (creates `.copilotclimonitor/notify.ps1` and `config.json`)
- `HttpIpcClient` service (sends events to systray via HTTP POST)
- `IpcModels` (shared port constant 54321, request/response DTOs)

**WPF systray app (`ElBruno.CopilotCLIMonitor`, `net10.0-windows`)**
- `App.xaml.cs` — startup, tray icon, IPC server lifecycle
- `DashboardWindow.xaml/.cs` — event history list view, clear/close
- `Services/IpcServer.cs` — `HttpListener` on port 54321, handles `/notify`, `/health`, `/status`, `/open`
- `Services/EventStore.cs` — thread-safe ring buffer (200 events)
- WinForms interop (`NotifyIcon`, balloon tips) for systray without extra packages
- Tray context menu: Open Dashboard, About, Exit

**CLI tool (`ElBruno.CopilotCLIMonitor.Cli`, `net10.0`)**
- `PackAsTool=true`, `ToolCommandName=copilotmon`
- Commands: `notify`, `init`, `doctor` (via `System.CommandLine` 3.0-preview)
- `HttpEventNotifier` — sends to systray via IPC with graceful fallback
- Auto-detects repository/branch from git for notification context

### Fixes and learnings

- `System.CommandLine` 3.0-preview.4 uses `Required` not `IsRequired` on `Option<T>`
- WPF project must be `net10.0-windows` + `UseWPF=true` + `UseWindowsForms=true` for systray
- `git rev-parse --abbrev-ref HEAD` exits non-zero in orphan/unborn branch state; use `git branch --show-current` instead
- `MonitorEvent.OccurredAt` must be eagerly captured at construction (private `_capturedAt` field) not lazy-computed — otherwise test timing assertions fail
- WPF entry point comes from `App.xaml` BAML generation; do NOT put a `Main()` in `Program.cs` alongside `App.xaml`

### Tests

67/67 pass after fixes. Key test files cover: MonitorEvent parsing, HookInstaller, RepositoryDetector, and all 3 Commands via FakeEventNotifier/FakeRepositoryDetector/FakeHookInstaller.
