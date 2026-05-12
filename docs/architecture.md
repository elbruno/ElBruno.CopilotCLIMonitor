# Architecture

## Overview

ElBruno.copilotclimon is a Windows-first monitoring solution for long-running GitHub Copilot CLI tasks. The architecture consists of two main components that work together to deliver notifications.

## System architecture

```
GitHub Copilot CLI
       ↓ (triggers hooks)
Repository hooks (.copilotclimon/hooks/)
       ↓ (forwards events)
CLI notification command
       ↓ (IPC communication)
Systray application (WPF)
       ↓ (displays)
Windows Toast Notifications
```

## Component 1: Systray application

### Purpose

Persistent Windows background application that receives events and displays notifications.

### Responsibilities

- **Notification display** – Render Windows toast notifications
- **Event routing** – Process incoming events from CLI
- **Settings management** – Store and retrieve user preferences
- **Event history** – Track recent events for dashboard
- **IPC listening** – Accept connections from CLI tool

### Architecture

**Entry point:** `ElBruno.copilotclimon.exe`

**Main projects:**
- `ElBruno.copilotclimon` – WPF Systray application
- `ElBruno.copilotclimon.Core` – Shared business logic
- `ElBruno.copilotclimon.Notifications` – Windows notification abstractions

### Technology stack

- **WPF** – Windows Presentation Foundation for UI
- **Windows Notifications API** – Native toast notifications
- **Named pipes or HTTP endpoint** – IPC with CLI tool
- **Structured logging** – Event tracking and debugging

### Data storage

**Global configuration:**

```text
%AppData%\ElBruno\copilotclimon\
├── config.json
├── settings.json
└── logs\
    └── *.log
```

**Event history:** In-memory cache with optional local persistence

### Process model

- Single application instance
- Background Windows service-like behavior (no visible window by default)
- Context menu accessible from system tray

## Component 2: Repository hook integration

### Purpose

Repository-level CLI integration that forwards Copilot CLI events to the Systray application.

### Responsibilities

- **Hook installation** – Deploy hook scripts to repository
- **Hook validation** – Verify hook configuration
- **Event forwarding** – Send events to Systray application
- **Error handling** – Log integration failures

### Architecture

**CLI entry point:** `copilotclimon`

**Main projects:**
- `ElBruno.copilotclimon.CLI` – Command-line interface
- `ElBruno.copilotclimon.Hooks` – Hook installation and management

### CLI commands

#### `copilotclimon init`

Installs hooks in current repository:

1. Detect repository root
2. Create `.copilotclimon/` directory
3. Deploy hook scripts
4. Create repository configuration
5. Validate setup
6. Display summary

#### `copilotclimon notify`

Forward event to Systray application:

```bash
copilotclimon notify \
  --event task-completed \
  --message "Migration finished" \
  --repository "my-repo" \
  --branch "main"
```

#### `copilotclimon doctor`

Validate system configuration:

1. Check .NET runtime
2. Verify Systray is running
3. Validate hook files
4. Test IPC connectivity
5. Display diagnostics

#### `copilotclimon open`

Open events dashboard window

#### `copilotclimon --version`

Display version information

### Hook model

Hooks are repository-scoped shell scripts deployed in `.copilotclimon/hooks/`:

**Example hook:** `.copilotclimon/hooks/on-task-completed.sh`

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)

copilotclimon notify \
  --event task-completed \
  --message "Task finished successfully" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --timestamp "$(date -u +%Y-%m-%dT%H:%M:%SZ)"
```

**Hook triggers:**
- Copilot CLI task completion
- Error occurrence
- Approval required
- Agent waiting for input

### Configuration

Repository configuration stored in `.copilotclimon/config.json`:

```json
{
  "version": "1.0",
  "repository": "my-repo",
  "enabled": true,
  "hooks": {
    "task-completed": true,
    "error": true,
    "approval-required": true,
    "agent-waiting": false
  },
  "notification-overrides": {
    "task-completed": {
      "priority": "normal",
      "sound": true
    }
  }
}
```

## Communication flow

### Event flow

1. **GitHub Copilot CLI** executes hook on event trigger
2. **Hook script** calls `copilotclimon notify` with event details
3. **CLI tool** connects to running Systray application via IPC
4. **CLI tool** sends event payload (JSON)
5. **Systray application** receives and processes event
6. **Systray application** renders toast notification
7. **Systray application** logs event to history

### IPC communication

Two options (to be determined during development):

**Option A: Local HTTP endpoint**

```
CLI → POST http://127.0.0.1:{port}/api/notify
      ↓ (JSON payload)
Systray app processes request
```

**Option B: Named pipes** (alternative)

```
CLI → Named pipe: \\.\pipe\copilotclimon
      ↓ (Binary protocol)
Systray app processes stream
```

**Recommendation:** Start with HTTP endpoint for simpler development and debugging.

## Data models

### Event model

```json
{
  "id": "uuid-string",
  "eventType": "task-completed|error|approval-required|agent-waiting",
  "timestamp": "2026-05-12T18:04:53Z",
  "repository": "my-repo",
  "branch": "main",
  "message": "Your message here",
  "priority": "normal|high|low",
  "source": "cli|hook",
  "metadata": {}
}
```

### Notification model

```json
{
  "title": "GitHub Copilot CLI",
  "body": "Task completed successfully",
  "duration": 5000,
  "sound": true,
  "priority": "normal",
  "actions": [
    {
      "id": "open",
      "label": "Open"
    }
  ]
}
```

## Deployment model

### Installation

User runs: `dotnet tool install -g ElBruno.copilotclimon`

This installs:
- CLI executable (`copilotclimon`)
- Systray application executable
- Supporting libraries

### Startup

User runs: `copilotclimon`

This starts:
- Systray application (background)
- Named pipe or HTTP listener
- System tray icon

### Repository integration

User runs: `copilotclimon init`

This creates:
- `.copilotclimon/` directory
- Hook scripts in `.copilotclimon/hooks/`
- Repository configuration file

## Security model

### Principles

- **No elevated privileges** – Application runs as standard user
- **No telemetry** – All events are local-only
- **No external dependencies** – Standalone operation
- **Local communication** – IPC only on localhost

### Permissions

- Systray app: User-level process
- CLI tool: User-level process
- IPC: Localhost only (no network exposure)
- File access: User home directory and repository paths only

## Extensibility

### Future considerations

- **Multi-agent support** – SQUAD agent awareness
- **Custom hooks** – User-defined event handlers
- **Plugin model** – Third-party notification providers
- **Remote monitoring** – Future cross-machine support (v2+)

## Performance targets

- **Memory footprint** – < 50 MB at rest
- **CPU usage** – < 1% idle
- **Startup time** – < 2 seconds
- **Notification latency** – < 500ms from event to display

## Testing strategy

### Unit tests

- Event model validation
- Configuration parsing
- Hook installation logic

### Integration tests

- End-to-end event forwarding
- IPC communication
- Hook trigger simulation

### Manual testing

- Windows notification rendering
- System tray interaction
- Repository initialization
- Multi-repository scenarios
