# River — History

**Agent:** River (Tester/QA)  
**Project:** ElBruno.CopilotCLIMonitor  
**Tech Stack:** .NET 10, xUnit, C#  
**Requestor:** Copilot  

## Day 1 Context (2026-05-13)

**Project State:**
- Version: 0.1.0 (alpha)
- xUnit test suite exists (13 test files, covering Core and CLI layers)
- Test Coverage: Unknown (no coverage tool installed yet)
- WPF test coverage: Zero (systray, dashboard, IPC server untested)
- Integration tests: Missing (hook → notification full flow)

**Your Key Files:**
- `tests/ElBruno.CopilotCLIMonitor.Tests/` — xUnit test project
- `.github/workflows/dotnet.yml` — CI pipeline (you can add coverage gates here)
- `src/` — Source under test

**Current Gaps:**
- No code coverage reporting (can use OpenCover, Coverlet)
- No mutation testing
- No performance baselines for notification latency
- No load testing (behavior under high hook frequency)

## Session 1 — 2026-05-13

**Task:** Full test infrastructure audit + Wave 1 test strategy

**Findings:**
- 80 tests, all passing. xUnit + coverlet.collector already installed.
- Coverage baseline: 83.5% line / 85.4% branch overall
  - Core: 91.5% / 94.3% — excellent
  - CLI: 78.5% / 80.5% — good; `HttpEventNotifier` and `RunOpenAsync` are zero-coverage
  - WPF: 0% — TFM mismatch (`net10.0-windows` vs `net10.0` test project) blocks reference
- CI (`dotnet.yml`) runs tests but does NOT collect or gate on coverage
- No WPF test project exists; no mocking library (handwritten fakes only — acceptable at current scale)
- `IpcServer` round-trip (real HTTP server ↔ client) has zero integration test coverage — highest risk gap

**Decisions made:**
- WPF Option A: extract `EventViewModel` and `FormatEventType` to Core → test in existing project
- WPF Option B (Wave 2): add `net10.0-windows` test project for headless WPF tests
- P0 gaps to close first: `HttpEventNotifier`, `IpcServer` round-trip, `RunOpenAsync`

**Artifacts written:**
- `.squad/decisions/inbox/river-test-strategy.md` — full audit, strategy, coverage targets, CI gate proposal

## Learnings

- `coverlet.collector` is present but CI never invokes `--collect:"XPlat Code Coverage"` — add it before claiming coverage baseline in PRs
- WPF tests require a second test project targeting `net10.0-windows`; the existing test project cannot reference the WPF assembly
- `IpcServer` is the riskiest untested component: async accept loop, cancellation, JSON parsing, and HTTP response paths are all uncovered
- Handwritten fakes are sufficient today; consider Moq/NSubstitute only if fake maintenance becomes a burden (> 6 fake classes)
