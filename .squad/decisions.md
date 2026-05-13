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

### Decision 16 — 6-Wave Execution Plan for v1.0 Release
**Date:** 2026-05-13 | **Agent:** Martin (Lead)
- Organize 63 GitHub issues into 6 execution waves
- Wave 1 (infrastructure, 7 issues): Foundation for all downstream work; 2-week sprint
- Wave 2-4 (22 high-priority issues): Features, testing, docs/security; 6 weeks total
- Wave 5 (28 medium-priority): Enhancements based on team capacity
- Wave 6 (6 low-priority): Post-1.0 parking lot
- Team allocation: Dolph (25 issues), Chevy (15), River (12), Sish (11)
- Target: v1.0 ready by 2026-06-30 (6 weeks from Wave 1 start)
- Critical path: #5→#6→#8→#49→#18 (config + IPC) + #29→#64 (release)
- Feature branches per issue, PR review by River, daily main sync, keep main green

### Decision 17 — Execution Plan & Wave Strategy
**Date:** 2026-05-13 | **Agent:** Martin (Lead)
- 63 issues → 6 waves → v1.0 by 2026-06-30
- Wave 1 (Weeks 1-2): Infrastructure (#5 config, #6 env vars, #8 validation, #18 IPC, #49 defaults, #29 release strategy, #64 versioning)
- Wave 2 (Weeks 3-4): Core features (systray UI, context menu, notifications, dashboard)
- Wave 3 (Weeks 5-6): Testing & QA (#61-#26, >85% coverage target)
- Wave 4 (Weeks 6-7): Docs, security, polish
- Wave 5 (Ongoing): Medium-priority enhancements (branding, i18n, logging, performance)
- Wave 6 (Post-1.0): Low-priority (distribution channels, advanced features)
- **Critical path:** #5→#6→#8→#49→#18 unblocks Wave 2 UI; #29→#64 unblocks all releases
- **Exit criteria:** Config tested, IPC stable, release process documented, main is green
- Team: Dolph (25 issues), Chevy (15), River (12), Sish (11)

### Decision 18 — UI Design System (GitHub-Blue)
**Date:** 2026-05-13 | **Agent:** Chevy (UI/Frontend)
- **Colors:** GitHub-blue accent (#0969DA), dark text (#1F2328), light surface (#F3F4F6), severity color-coding (green/yellow/red)
- **Typography:** Segoe UI body (13px), Cascadia Code mono (11px), titles 14–18px bold
- **Spacing:** 8px base unit; controls 28px tall, 4px corner radius
- **Components:** WPF-native buttons, inputs, lists with GitHub-inspired styling
- **Settings UI:** Modal dialog with Notifications + General sections (awaiting Dolph's API)
- **Dashboard:** Color-coded rows (severity), expandable for full messages, event-type filter, 200-item cap
- **Notifications:** Windows native balloons for Wave 1; WinRT toasts upgrade in Wave 2
- **Rationale:** Familiar to target users (Copilot CLI devs); reduces cognitive load; matches Microsoft ecosystem

### Decision 19 — Test Strategy & Coverage Targets
**Date:** 2026-05-13 | **Agent:** River (QA/Testing)
- **Baseline:** 83.5% overall (Core 91.5%, CLI 78.5%, WPF 0% due to TFM mismatch)
- **Wave 1 P0 tests:**
  - `HttpEventNotifier` happy path + connection-refused fallback
  - `RunOpenAsync` tests (monitor running/not running)
  - `IpcServer` ↔ `HttpIpcClient` round-trip integration test
- **Wave 1 P1 tests:** Hook install verification, full hook→IPC→EventStore flow, `DoctorCommand` branch coverage
- **WPF strategy:** Option A (Wave 1) extract logic to Core; Option B (Wave 2) add WPF-specific test project
- **Wave 1 target:** Core ≥92%, CLI ≥85%, Overall ≥85%
- **1.0 target:** Core ≥90%, CLI ≥85%, WPF extracted ≥50%, Overall ≥80%
- **CI gate:** Add `--collect:"XPlat Code Coverage"` to dotnet.yml; upload artifacts; threshold enforcement post-Wave 1

### Decision 20 — Release Checklist (9 Phases)
**Date:** 2026-05-13 | **Agent:** Sish (DevOps/Release)
- **Phase 1:** Feature completeness, final QA (Chevy), docs finalization (River), security audit
- **Phase 2:** Version bump (SemVer) in .csproj, CHANGELOG.md update
- **Phase 3:** Local package build, contents verification, tool install test, CLI commands test
- **Phase 4:** Git commit + annotated tag, push to GitHub
- **Phase 5:** Create GitHub Release (triggers publish-nuget.yml), monitor workflow, verify NuGet.org publication
- **Phase 6:** Chocolatey (deferred to 1.1+), WinGet (deferred to 1.1+)
- **Phase 7:** Auto-update infrastructure (deferred to 1.1+)
- **Phase 8:** Release announcement and comms
- **Phase 9:** Post-release verification (real-world install, docs check, issue triage for 1.1)
- **Success:** v1.0.0 on NuGet.org, GitHub Release published, `copilotclimon --version` confirms 1.0.0

### Decision 21 — NuGet Icon & Branding
**Date:** 2026-05-13 | **Agent:** Bender (DevOps)
- Generated NuGet icon (GPT-Image-2) stays at `images\nuget-icon.png` (packed path preserved)
- Icon includes monitor/notification overlay for readability at 256px and below
- Satisfies branding request; keeps existing package metadata untouched
- Deterministic asset generation; no external dependencies required

### Decision 22 — Dedicated NuGet Publish Workflow
**Date:** 2026-05-13 | **Agent:** Bender (DevOps)
- Keep `.github/workflows/dotnet.yml` as CI only (restore, build, test, pack, upload artifacts)
- Publish exclusively from `.github/workflows/publish-nuget.yml` via NuGet Trusted Publisher (OIDC)
- Mark WPF and Core projects as `IsPackable=false` so solution-level pack emits only CLI tool package
- Fallback icon: `images\nuget-icon.png` (repo-local, no external image generation required)
- Rationale: Eliminates workflow failures on push CI; keeps package publish on deliberate release path; zero long-lived API keys

### Decision 23 — Workflows Separated: CI vs. Release
**Date:** 2026-05-13 | **Agent:** Bender (DevOps)
- `.github/workflows/dotnet.yml` handles CI (restore, build, test, pack, upload artifacts)
- `.github/workflows/publish-nuget.yml` (or `release.yml`) handles NuGet publishing (OIDC Trusted Publisher)
- Remove publish logic from push CI to eliminate secret-based gate and prevent accidental publishes
- Release workflow matches repo decision: GitHub Release creates tag → workflow publishes
- Maintains audit trail; explicit version control; industry best practice

### Decision 24 — v1.0 Release Readiness
**Date:** 2026-05-13 | **Agent:** Sish (DevOps/Release)
- **Infrastructure: 90% ready** (NuGet OIDC, CI/CD, packaging, version strategy all production-ready)

### Decision 25 — Wave 1 Foundation Complete & Merged
**Date:** 2026-05-13 | **Agent:** Scribe (Team Lead) | **Status:** ACTIVE
- **Merged branches:** squad/5-structured-logging, squad/44-ui-design-system, squad/64-version-bump
- **Commits:**
  - d53bea1: [squad:dolph] Add structured logging to Core services (fixes #5) — 108 tests passing
  - 7da2fe0: [squad:sish] Release pipeline skeleton, version-bump script, NuGet metadata validation
  - 0fb006f: [squad:chevy] UI design system foundation: MVVM structure, GitHub-blue palette, Settings UI mockup
- **Main branch HEAD:** 0fb006f (fast-forward merge complete)
- **Files added:** 31 files, 9,668 insertions (+) / 23 deletions (-)
- **Key deliverables:**
  - Core: ILogger<T>? support in all 4 services (EventStore, MonitorEventParser, IpcServer, HttpEventNotifier)
  - Core: New MonitorEventParser service with round-trip logging tests
  - Release: `.github/workflows/release-v1.0.yml`, `scripts/version-bump.ps1`, NuGet metadata validation
  - WPF: MVVM structure (ViewModels/, Views/, Styles/), GitHub-blue design system (Colors.xaml, Buttons.xaml, TextBlocks.xaml), Settings UI mockup
- **Next phase:** Wave 1B (IPC config, event persistence, quiet hours), then Wave 2 (Settings UI binding, toast notifications, dashboard)
- **Blockers for v1.0 core:** None (MSI installer and auto-update deferred to v1.1)
- **v1.1 roadmap:** MSI installer (WiX recommended), auto-update mechanism (NuGet API check), WinGet submission
- **Workarounds:** .NET Tool installation for v1.0 (ship now); MSI 2-4 weeks post-1.0
- **Distribution:** NuGet (primary, OIDC-secured), GitHub Releases (artifacts), MSI/WinGet/Chocolatey (post-1.0)
- **Success criteria:** Semantic versioning adopted, features complete, QA sign-off, docs finalized, v1.0.0 published

### Decision 25 — Semantic Versioning Strategy
**Date:** 2026-05-13 | **Agent:** Sish (DevOps/Release)
- **Format:** MAJOR.MINOR.PATCH (e.g., 1.0.0, 1.1.0-rc1)
- **MAJOR:** Breaking changes (v2.0.0)
- **MINOR:** New features, backward-compatible (v1.1.0 = MSI + auto-update + WinGet)
- **PATCH:** Bug fixes, hotfixes (v1.0.1)
- **Pre-release:** Only for major releases (1.0.0-rc1); SemVer complete rules apply
- **Version source:** Single source of truth = `.csproj` `<Version>` property
- **Workflow:** Commit version bump + CHANGELOG → tag (vX.Y.Z) → GitHub Release → auto-publish
- **Phases:** 1.0.0 (core features, 2026-05), 1.1.0 (MSI + auto-update, 2026-06), 1.2.0 (Chocolatey + Store, 2026-07), 2.0.0 (breaking/cross-platform, TBD)
- **Decision points:** Skip 1.0.0-rc1? (recommended: yes, go straight to GA); bump Core/WPF versions? (recommended: keep in sync with CLI)

### Decision 26 — Wave 1 Kickoff (Infrastructure Sprint)
**Date:** 2026-05-13 | **Agent:** Martin (Lead)
- **Duration:** 2026-05-13 → 2026-05-27 (2 weeks)
- **Team:** Dolph (config + IPC), Sish (release + version), River (prep WPF tests), Chevy (review specs)
- **Issues:** #5, #6, #8, #18, #49 (Dolph config stream), #29, #64 (Sish release stream)
- **Key decisions locked in:** JSON config format, IPC on localhost:54321, release branch strategy, SemVer
- **Execution rules:** Feature branch per issue, River PR review before merge, daily main sync (keep green), all code tested
- **Exit criteria:** Config system tested, IPC stable, defaults working, env vars functional, release process dry-run passed, v0.2.0 tagged
- **Success:** Infrastructure solid; Wave 2 (UI features) unblocked; team moves to features 2026-05-30

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
