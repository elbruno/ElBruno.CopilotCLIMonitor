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

## [0.3.0] - 2026-08-16

### Changed
- Bumped all project versions to 0.3.0 for the release.
- Added README metadata to the Core NuGet package.
- Updated the trusted publish workflow to pack and publish both NuGet packages at the release tag version.

## [0.2.6] - 2026-07-30

### Fixed
- Stabilized IPC timing test for CI runners with proper synchronization (#113)
- Fixed race condition in UserPreferencesStore env var isolation for parallel test execution (#112)
- Updated actions/setup-dotnet from v5 to v6 for latest runtime fixes (#110)

### Security
- Updated Microsoft.Extensions.Logging.Console from 10.0.8 to 10.0.10 for security and reliability (#111)

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
- `copilotclimon init` – Initialize repository
- `copilotclimon notify` – Send notifications
- `copilotclimon doctor` – Validate setup
- `copilotclimon open` – Open dashboard
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

[Unreleased]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/releases/tag/v0.3.0
[0.2.6]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/releases/tag/v0.2.6
[1.0.0]: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/releases/tag/v1.0.0
