# Squad Decisions

## Active Decisions

### Decision 001 — Local HTTP for IPC
**Date:** 2026-05-12 | **Agent:** Bender
- Use `System.Net.HttpListener` bound to `http://localhost:54321/` in the WPF app
- CLI uses `HttpClient` to POST to `/notify`
- Simple to implement, debug, and test with curl
- Port 54321 chosen to avoid common conflicts

### Decision 002 — Two-Project CLI + WPF Split
**Date:** 2026-05-12 | **Agent:** Bender
- `src/ElBruno.CopilotCLIMonitor` — WPF systray app (`net10.0-windows`, `WinExe`)
- `src/ElBruno.CopilotCLIMonitor.Cli` — .NET Tool CLI (`net10.0`, `PackAsTool=true`)
- `src/ElBruno.CopilotCLIMonitor.Core` — Shared models/services (`net10.0`)
- WPF can't be packed as a .NET Tool; CLI must target portable TFM

### Decision 003 — Initial CLI Command Name is `copilotmon`
**Date:** 2026-05-12 | **Agent:** Bender | **Status:** Superseded by Decision 007
- Set `ToolCommandName=copilotmon`, `AssemblyName=copilotmon`
- Shorter, faster to type, matches task constraint
- Package identity stays `ElBruno.CopilotCLIMonitor`

### Decision 004 — WinForms Interop for Systray
**Date:** 2026-05-12 | **Agent:** Bender
- Use `UseWindowsForms=true` and `System.Windows.Forms.NotifyIcon`
- Built-in, maintained, zero-dependency, works alongside WPF

### Decision 005 — Hook Script Uses `copilotmon notify`
**Date:** 2026-05-12 | **Agent:** Bender | **Status:** Updated in Decision 007
- `.copilotclimonitor/notify.ps1` script calls `copilotmon notify`
- Updated HookInstaller accordingly

### Decision 006 — Branch Detection Uses `git branch --show-current`
**Date:** 2026-05-12 | **Agent:** Bender
- Primary: `git branch --show-current` (safe in all modern git)
- Fallback: `git symbolic-ref --short HEAD`
- Works out-of-the-box in fresh repos and CI environments

### Decision 007 — CLI Command Name Changed to `copilotclimon`
**Date:** 2026-05-12 | **Agent:** Bender | **Supersedes:** Decision 003
- `AssemblyName=copilotclimon`
- `ToolCommandName=copilotclimon`
- All user-facing strings, docs, and tests updated to `copilotclimon`
- Package identity remains `ElBruno.CopilotCLIMonitor`
- Hook config directory (`.copilotclimonitor`) unchanged
- `copilotclimon` more descriptive, aligns with product name, eliminates confusion

### Decision 8 — Command-name assertions kept in sync
**Date:** 2026-05-12 | **Agent:** Amy
- All test assertions containing CLI command name updated atomically with source changes
- Breaking assertions fixed: `DoctorHandlerTests`, `HookInstallerTests`, temp-dir prefixes
- Non-breaking cosmetic updates ensure test output clarity and easier triage

### Decision 9 — Add command-metadata regression tests
**Date:** 2026-05-12 | **Agent:** Amy
- Added three `[Fact]` tests to `SystemCommandLineWiringTests`:
  - `RootCommand_Description_ContainsCopilotclimon`
  - `InitCommand_Description_ContainsCopilotclimon`
  - `DoctorCommand_Description_ContainsCopilotclimon`
- Prevents silent stale text in help output; catches failures at test time

### Documentation standard: Microsoft Style Guide
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Follow Microsoft docs conventions for all markdown files
- Sentence-case headings, active voice, second person, present tense
- Consistent with Azure/Microsoft ecosystem; familiar to .NET developers

### Publishing model: NuGet Trusted Publisher
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Use GitHub OIDC (Trusted Publisher) instead of API key secrets
- Zero security risk, no credentials stored, audit trail via GitHub
- Workflows use `GITHUB_TOKEN` only, no `NUGET_API_KEY` required

### Package type: .NET Tool
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Distribute as a global .NET Tool via `dotnet tool install -g ElBruno.CopilotCLIMonitor`
- Modern .NET distribution; single executable; version management via NuGet

### Repository root: Minimal files only
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Only README.md, LICENSE, .csproj files at root
- Clean, professional appearance
- All source → `/src`, tests → `/tests`, docs → `/docs`

### Release workflow: GitHub Release → Automated NuGet publish
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Publish only on GitHub Release (not push to main)
- Explicit versioning, deliberate release process, prevents accidental publishes
- Create GitHub Release → Workflow extracts version, publishes to NuGet

### Documentation organization: Scenario-based
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Split docs by task type (setup, troubleshooting, architecture, etc.)
- Setup/install guides first → examples → architecture → troubleshooting
- Users can find what they need; easier to maintain and link

### Contribution guidelines: Modern .NET patterns
**Date:** 2026-05-12 | **Agent:** Farnsworth
- Require async/await, DI, structured logging, XML docs
- Professional code quality; maintainable long-term
- Enforced via code review guidelines in CONTRIBUTING.md

### Decision 13 — GitHub Issues as Primary Work Tracking SSOT
**Date:** 2026-05-13 | **Agent:** Squad (Team Initialization)
- GitHub Issues is the single source of truth for all work items
- All tasks, bugs, features, and documentation tracked as issues
- Replaces ad-hoc task tracking; centralizes visibility
- Team members review and pull work from issue queue

### Decision 14 — Ralph Auto-Triage and Routing
**Date:** 2026-05-13 | **Agent:** Squad (Team Initialization)
- Ralph agent activated for continuous work monitoring and auto-triage
- Automatically routes issues by category and priority
- Prevents bottlenecks; ensures work reaches right team member
- Monitoring dashboards track issue lifecycle and team velocity

### Decision 15 — Triage Routing Rules by Category
**Date:** 2026-05-13 | **Agent:** Squad (Team Initialization)
- **`type:feature`** → Feature backlog (prioritized by roadmap, assigned to Martin/Dolph)
- **`type:bug`** → Bug triage (assessed for severity/impact, routed to affected domain)
- **`type:docs`** → Documentation (assigned to River)
- **`type:test`** → Testing and QA (assigned to Chevy)
- **`type:infrastructure`** → DevOps and tooling (assigned to Sish)
- Weekly triage review in team ceremonies ensures accuracy

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
