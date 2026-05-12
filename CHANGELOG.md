# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial public release setup
- Documentation scaffolding
- GitHub Actions publishing workflows
- MIT licensing

### Changed
- (No changes yet)

### Deprecated
- (No deprecations yet)

### Removed
- (No removals yet)

### Fixed
- (No fixes yet)

### Security
- (No security updates yet)

## [1.0.0] - 2026-05-12

### Added
- Windows Systray application for notification monitoring
- GitHub Copilot CLI hook integration
- Toast notification support
- Event history dashboard
- Repository-level configuration
- CLI commands for initialization and testing
- Settings management UI
- Logging and diagnostics
- NuGet publishing support
- GitHub Actions CI/CD workflows

### Features

#### Systray Application
- Native Windows Systray integration
- Context menu with quick actions
- Background execution support
- Windows toast notifications
- Event history tracking
- Settings management

#### CLI Tool
- `copilotclimonitor init` – Initialize repository
- `copilotclimonitor notify` – Send notifications
- `copilotclimonitor doctor` – Validate setup
- `copilotclimonitor open` – Open dashboard
- `--version` – Display version

#### Hook Integration
- Automatic hook installation
- Event forwarding to Systray
- Repository configuration
- Multiple hook types supported

### Security
- No elevated privileges required
- Local-only communication
- No external dependencies
- No telemetry or cloud sync

---

[Unreleased]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/releases/tag/v1.0.0
