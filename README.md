# ElBruno.CopilotCLIMonitor

![license](https://img.shields.io/github/license/elbruno/ElBruno.CopilotCLIMonitor)
![NuGet](https://img.shields.io/nuget/v/ElBruno.CopilotCLIMonitor)
![.NET](https://img.shields.io/badge/.NET-10+-512BD4)
![Build](https://img.shields.io/github/actions/workflow/status/elbruno/ElBruno.CopilotCLIMonitor/dotnet.yml)

Stop babysitting long-running Copilot CLI tasks. ElBruno.CopilotCLIMonitor is a lightweight Windows Systray application that keeps you informed with native notifications when your AI workflows finish.

## Overview

ElBruno.CopilotCLIMonitor bridges GitHub Copilot CLI hooks and Windows desktop notifications. Minimize your terminal and stay aware of important events without constant terminal monitoring.

**Supported events:**
- Task completed
- Workflow finished
- Build/test execution completed
- Agent waiting for input
- Error occurred
- Approval required

## Quick start

### Install

```powershell
dotnet tool install -g ElBruno.CopilotCLIMonitor
```

### Start the app

```powershell
copilotclimon
```

The application runs in the Windows Systray. Look for the notification icon in your system tray.

### Initialize your repository

```bash
cd your-repository
copilotclimon init
```

This installs repository hooks and configures event forwarding.

### Run your Copilot CLI tasks

```bash
copilot -p "Using BYOK, answer in one sentence with the wire model/deployment name you are using."
```

When your task completes, you'll receive a Windows toast notification—no terminal watching required.

## Features

### Systray integration

- Persistent Windows Systray presence
- Context menu with quick actions
- Background execution while you work

### Native toast notifications

- Configurable notification behavior
- Optional sound alerts
- Priority levels (Info, Warning, Error)
- Action buttons for quick responses

### Event history

- Recent events dashboard
- Timestamp tracking
- Event filtering
- History clearing

### Repository awareness

- Per-repository hook configuration
- Custom event routing
- Local configuration support

### Settings

Configure:
- Notification types
- Toast duration
- Sound preferences
- Quiet hours
- Repository filters
- Event filtering
- Startup behavior
- Logging level

### Localization

- Automatic RTL layout support based on UI culture
- Dashboard and settings windows adapt to right-to-left languages
- Event labels localized for English, Spanish, and French

## Architecture

The solution has two main components:

### 1. Systray application

A native WPF Windows application that:
- Lives in the Windows Systray
- Receives and displays notifications
- Manages user settings
- Tracks recent events

### 2. Repository hook integration

CLI-based hook installer that:
- Integrates with GitHub Copilot CLI hooks
- Forwards events to the Systray app
- Supports repository-local configuration
- Validates hook installation

## CLI commands

### Initialize repository

```bash
copilotclimon init
```

Detects the current repository, installs hooks, and registers event forwarding.

### Send notification

```bash
copilotclimon notify --event task-completed --message "Your message here"
```

### Validate setup

```bash
copilotclimon doctor
```

Checks system configuration and hook installation.

### Open dashboard

```bash
copilotclimon open
```

Opens the events dashboard window.

### Show version

```bash
copilotclimon --version
```

### Check for updates

```bash
copilotclimon update
```

Install latest version directly:

```bash
copilotclimon update --install
```

## Configuration

### Global configuration

User settings are stored in:

```text
%AppData%\ElBruno\CopilotCLIMonitor
```

### Repository configuration

Local hook configuration:

```text
.copilotclimonitor
```

### IPC authentication (optional)

To require authentication between CLI and monitor IPC calls, set the same token in both processes:

```powershell
$env:COPILOTCLIMON_IPC_TOKEN = "your-shared-secret"
```

When configured, the monitor accepts only requests containing this token.

## Technology

- **.NET 10** – Modern .NET runtime
- **WPF** – Native Windows desktop framework
- **Windows Notifications** – Native toast notifications
- **GitHub Copilot CLI** – Hook integration

## Setup guide

See [setup.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/setup.md) for detailed installation and configuration instructions.

## Standalone installer packaging

See [installer.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/installer.md) for standalone Windows package generation.

## Portable ZIP distribution

See [portable-distribution.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/portable-distribution.md) for no-installation packaging and usage.

## Chocolatey packaging

See [chocolatey.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/chocolatey.md) for creating and publishing Chocolatey packages.

## Troubleshooting

See [troubleshooting.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/troubleshooting.md) for common issues and solutions.

## Code coverage

See [coverage.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/coverage.md) for local and CI coverage report generation.

## Linting rules

See [linting.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/linting.md) for `.editorconfig` and StyleCop analyzer configuration.

## Accessibility testing

See [accessibility-testing.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/accessibility-testing.md) for keyboard, screen reader, and WCAG-focused validation checklist.

## Performance testing

See [performance-testing.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/performance-testing.md) for memory/CPU/responsiveness guardrail tests.

## Telemetry (optional)

See [telemetry.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/telemetry.md) for opt-in telemetry behavior and data details.

## Privacy policy

See [privacy-policy.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/privacy-policy.md) for data handling practices and user controls.

## User manual

See [user-manual.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/user-manual.md) for end-user step-by-step usage.

## Video tutorials

See [video-tutorials.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/video-tutorials.md) for recording plans, standards, and publishing checklist.

## Architecture documentation

See [architecture.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/architecture.md) for technical design details.

## Developer guide

See [developer-guide.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/developer-guide.md) for contributing workflow, setup, and architecture references.

## Versioning and release strategy

This project uses Semantic Versioning (`MAJOR.MINOR.PATCH`) with an automated release pipeline.
See [versioning.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/versioning.md) for bump rules and release flow.
See [release-branch-strategy.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/release-branch-strategy.md) for `main`/`develop`/`release/*` branch responsibilities.
Use `scripts/version-auto-increment.ps1` to auto-calculate next patch/minor/major before applying version bump updates.
Use `.github/release-notes-template.md` to keep release notes format consistent across versions.
For emergency patch releases, follow [hotfix-process.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/hotfix-process.md).

## Examples

### Hook integration example

The following hook configuration forwards Copilot CLI events:

```bash
# Installed in: .copilotclimonitor/hooks/on-task-complete.sh
#!/bin/bash
copilotclimon notify \
  --event task-completed \
  --message "Migration task finished" \
  --repository "$(basename $(git rev-parse --show-toplevel))" \
  --branch "$(git rev-parse --abbrev-ref HEAD)"
```

## Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## Product screenshots

Screenshot catalog and capture standards are documented in [screenshots.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/screenshots.md).

## Logo variants

Logo source variants and export targets are documented in [logo-variants.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/logo-variants.md).

## Marketing graphics

Social cards and feature-promotion assets are documented in [marketing-graphics.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/marketing-graphics.md).

## Hook configuration guide

For advanced hook setup, custom scripts, and secure event routing, see [hook-configuration.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/hook-configuration.md).

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Inspiration

This project is inspired by and follows patterns from:

- [ElBruno.AspireMonitor](https://github.com/elbruno/ElBruno.AspireMonitor)
- GitHub Copilot CLI
- SQUAD multi-agent orchestration

## Support

- 📖 [Documentation](docs/)
- 🐛 [Issue tracker](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/issues)
- 💬 [Discussions](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/discussions)

## About ElBruno

ElBruno creates developer tools, samples, and educational content for AI-assisted development. Follow for updates:

- 🐦 [Twitter/X](https://twitter.com/elbruno)
- 💼 [LinkedIn](https://linkedin.com/in/elbruno)
- 📝 [Blog](https://elbruno.com)
