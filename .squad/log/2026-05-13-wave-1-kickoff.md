# Wave 1 Kickoff — 2026-05-13

**Team:** Martin (Lead) + Dolph (Backend) + Chevy (Frontend) + River (QA) + Sish (DevOps)  
**Issues:** #5, #6, #8, #18, #49, #29, #64 (7 total)  
**Duration:** 2 weeks (2026-05-13 → 2026-05-27)  
**Success:** Logging + Event persistence + Config system working, main stays green

## Decisions Recorded

- **Decision 17:** Execution plan (6 waves, 12 weeks → v1.0)
  - Wave 1 infrastructure (config, IPC, versioning)
  - Wave 2 core features (systray, notifications)
  - Wave 3-4 quality & hardening
  - Wave 5 enhancements; Wave 6 post-1.0 parking lot
  - Critical path: #5→#49→#18 unblocks Wave 2

- **Decision 18:** UI specs (GitHub-blue design system, MVVM, Settings window)
  - Color-coded severity rows; expandable messages
  - Settings modal with Notifications + General sections
  - GitHub-familiar design; reduces cognitive load for target users
  
- **Decision 19:** Test strategy (HttpEventNotifier + IPC integration tests first)
  - Baseline: 83.5% (Core 91.5%, CLI 78.5%, WPF 0%)
  - Wave 1 targets: Core ≥92%, CLI ≥85%, Overall ≥85%
  - P0 tests: HttpEventNotifier, IpcServer round-trip, RunOpenAsync
  - WPF logic extracted to Core (Option A); dedicated test project Wave 2

- **Decision 20:** Release checklist (v1.0 gates, NuGet publish, auto-update)
  - 9-phase gate: Feature complete → QA sign-off → version bump → git tag → GitHub Release → NuGet publish
  - Automated via OIDC Trusted Publisher; zero API key secrets
  - Auto-update deferred to v1.1 (workaround: manual update docs for v1.0)

- **Decision 21-23:** NuGet publish infrastructure (Trusted Publisher, workflows separated)
  - Dedicated `publish-nuget.yml` workflow (separate from CI)
  - OIDC-secured; no long-lived secrets
  - WPF + Core marked `IsPackable=false`; pack emits CLI tool only

- **Decision 24-25:** Version strategy (Semantic Versioning, phased releases)
  - v1.0.0 (core features, 2026-05)
  - v1.1.0 (MSI + auto-update, 2026-06)
  - v1.2.0+ (Chocolatey, Store, etc.)
  - Single source of truth: `.csproj` `<Version>` property

## Daily Workflow

- **Feature branches:** One per issue (`squad/{issue-number}-{slug}`)
- **PR Review:** River reviews all PRs before merge to feature branch
- **Main sync:** After PR approval, issue author merges to feature branch → sync to main daily (keep builds green)
- **Commit message:** `[squad:{member}] {description} (fixes #{issue})`
  - Include Co-authored-by trailer for all commits
  - Example: `[squad:dolph] Add JSON config validation (fixes #8)`

## Next

1. **Martin** → Create GitHub issue labels (wave, priority, squad assignment); update .squad/decisions.md
2. **Dolph** → Pull #5 (config format design); start tests immediately
3. **Sish** → Pull #29 (release strategy); finalize + dry-run process
4. **River** → Prep WPF test scaffolding; review Dolph's first PR
5. **Chevy** → Review UI specs; standby for Wave 2 kickoff (2026-05-30)

---

**Wave 1 Complete:** 2026-05-27  
**Wave 2 Kickoff:** 2026-05-30

Ready? Let's ship! 🚀
