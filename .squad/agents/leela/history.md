# Leela — History

## 2026-05-12 — Initial Architecture Session

**Task:** Bootstrap architecture for ElBruno.CopilotCLIMonitor from PRD.

**Decisions made:**
- Three-project solution: WPF app (`.`), Core library (`.Core`), CLI .NET Tool (`.Hooks`)
- IPC: Local HTTP via Kestrel Minimal API hosted inside WPF app
  - Default port 5150, fallback to 5151–5159
  - Lock file at `%AppData%\ElBruno\CopilotCLIMonitor\monitor.lock`
- Systray: `H.NotifyIcon.Wpf` (actively maintained fork)
- MVVM: `CommunityToolkit.Mvvm` (source-generated)
- CLI parsing: `System.CommandLine`
- Toast: `Microsoft.Toolkit.Uwp.Notifications` with balloon-tip fallback
- Logging: `Serilog` + file sink

**Artifacts created or updated:**
- `.squad/decisions/inbox/leela-architecture.md` — full ADR with risks, package table, and actual-state reconciliation
- `.squad/agents/leela/charter.md` — this agent's charter  
- `.squad/agents/leela/history.md` — this file
- `ElBruno.CopilotCLIMonitor.sln` — solution file created (was missing)
- `src/ElBruno.CopilotCLIMonitor.Core/Models/MonitorEvent.cs` — fixed `OccurredAt` property initializer bug

**Learnings:**
- WPF app + Kestrel co-hosting works cleanly via `IHostedService`; cross-thread calls need `Dispatcher.InvokeAsync`
- `H.NotifyIcon.Wpf` is the correct package (not the older `Hardcodet.NotifyIcon.Wpf`)
- Toast notifications may require AUMID registration — implement dual-provider from day one
- Lock file approach for port discovery is simpler than named pipes for hook script authors who just need `curl`
- The glob tool may return stale paths from bin/obj; always verify with `Get-ChildItem -Recurse | Where-Object { -not in bin/obj }` when debugging file structure issues
- `MonitorEvent.OccurredAt` must be a property initializer (`{ get; } = Timestamp ?? DateTimeOffset.UtcNow`) not a computed property (`=> Timestamp ?? DateTimeOffset.UtcNow`) — computed properties re-evaluate on each access, causing timing test failures
- The `GetCurrentBranch` test is correctly resilient to unborn-branch repos; null is an acceptable result there
- Build verdict: **all 4 projects compile, all 67 tests pass**
