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

## Troubleshooting

See [troubleshooting.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/troubleshooting.md) for common issues and solutions.

## Architecture documentation

See [architecture.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/architecture.md) for technical design details.

## Versioning and release strategy

This project uses Semantic Versioning (`MAJOR.MINOR.PATCH`) with an automated release pipeline.
See [versioning.md](https://github.com/elbruno/ElBruno.CopilotCLIMonitor/blob/main/docs/versioning.md) for bump rules and release flow.

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
