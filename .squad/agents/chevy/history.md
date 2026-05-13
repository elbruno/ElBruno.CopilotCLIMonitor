# Chevy — History

**Agent:** Chevy (Frontend/UI Dev)  
**Project:** ElBruno.CopilotCLIMonitor  
**Tech Stack:** .NET 10, WPF, C#, XAML  
**Requestor:** Copilot  

## Day 1 Context (2026-05-13)

**Project State:**
- Version: 0.1.0 (alpha)
- WPF app scaffold exists (systray host, basic windows)
- Dashboard window incomplete (no history view)
- Icons missing (need app icon, notification icons)
- Settings UI not yet implemented

**Your Key Files:**
- `src/ElBruno.CopilotCLIMonitor/` — WPF app (your main codebase)
- `assets/` — Icon storage (to be created/organized)
- `docs/` — UI documentation (you'll document UX decisions)

**Dependencies:**
- Dolph's Core library APIs (consume settings, event store, notifications)
- River's test suite (help define UI test scenarios)
- Sish's packaging (ensure your binaries are relocatable)

## Learnings

### 2026-05-13 — WPF Audit (Wave 1 prep)

**What's working:**
- Systray via `NotifyIcon` (WinForms interop, Decision 004 ✅)
- IPC server connected and wired to tray notifications
- `DashboardWindow` — functional GridView with Time/Event/Repo/Message columns
- `App.xaml` global font: Segoe UI 13 px
- App lifecycle (hide-on-close, explicit shutdown) correct

**What's missing / needs work:**
- `ViewModels/` directory exists but is **completely empty** — no MVVM framework in place; `EventViewModel` is inline in code-behind
- `Views/` directory **completely empty** — all windows live at project root
- `Resources/` directory **completely empty** — no ResourceDictionaries, no styles
- **No .ico file** — tray icon generated at runtime as a 16×16 DodgerBlue circle (placeholder)
- **No assets folder** — only `images/nuget-icon.png` at repo root
- **No Settings window** at all
- **About** is a plain `MessageBox.Show()` — no dedicated About window
- No color scheme / design tokens — bare bones global font only

**Design system drafted** → `.squad/decisions/inbox/chevy-ui-specs.md`  
**UI spec questions sent to Martin** → same file

**Key design choices made:**
- GitHub-blue (`#0969DA`) as primary accent — matches target audience
- 8 px spacing base unit
- Severity color rows: green=success, amber=warning, red=error
- Typography: Segoe UI body + Cascadia Code for timestamps/mono
