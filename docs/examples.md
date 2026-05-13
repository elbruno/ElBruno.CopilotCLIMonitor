# Examples

## Example 1: Basic repository setup

Initialize a repository for notifications:

```bash
cd my-awesome-project
copilotclimon init
```

Expected output:

```
✓ Repository detected: my-awesome-project
✓ Working directory: C:\repos\my-awesome-project
✓ Git branch: main

Installing hooks...
✓ Created .copilotclimonitor/ directory
✓ Installed 4 hook scripts
✓ Created repository configuration

Validating setup...
✓ Hook files are executable
✓ Systray application is running
✓ Network connectivity verified

✅ Setup complete! Your repository is ready for notifications.

Next steps:
- Run: copilotclimon doctor (to validate)
- Run: copilot task run "your task" (to test)
```

## Example 2: Sending a custom notification

From within a repository or script:

```bash
copilotclimon notify \
  --event task-completed \
  --message "Data migration completed successfully" \
  --repository "my-awesome-project" \
  --branch "feature/migration"
```

Toast notification appears:

```
GitHub Copilot CLI
Data migration completed successfully
[my-awesome-project] feature/migration
```

## Example 3: Hook integration with Copilot CLI

When Copilot CLI completes a task, it triggers the hook automatically:

**What you run:**

```bash
copilot task run "Generate unit tests for UserService"
# ... task runs for 30 minutes ...
```

**What happens automatically:**

1. Copilot CLI completes
2. Copilot CLI fires `on-task-completed` hook
3. Hook script executes (`.copilotclimonitor/hooks/on-task-completed.sh`)
4. Hook calls `copilotclimon notify`
5. Notification appears in Systray

## Example 4: Error notification

When a Copilot CLI task fails:

```bash
copilot task run "Complex refactoring"
# Error occurs during task execution
```

**Hook fires automatically:**

```
GitHub Copilot CLI
Task failed: Execution timeout after 2h
Error: TIMEOUT_EXCEEDED
[my-repo] main
```

## Example 5: Approval required notification

When a task requires user approval:

```bash
copilot task run "Apply generated changes"
# Task pauses, waiting for approval
```

**Notification appears:**

```
GitHub Copilot CLI
Approval required

The task is waiting for your approval to:
- Modify 15 files
- Run tests
- Commit changes

[Open] [Approve] [Deny]
```

## Example 6: Custom hook for build completion

You can extend hooks with custom logic. Example: Notify when a long build completes:

**File: `.copilotclimonitor/hooks/on-build-complete.sh`**

```bash
#!/bin/bash
# Custom hook for build completion notification

REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
BUILD_TIME=$1
BUILD_STATUS=$2

if [ "$BUILD_STATUS" == "success" ]; then
  MESSAGE="Build completed in $BUILD_TIME seconds"
  PRIORITY="normal"
else
  MESSAGE="Build failed after $BUILD_TIME seconds"
  PRIORITY="high"
fi

copilotclimon notify \
  --event build-completed \
  --message "$MESSAGE" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --priority "$PRIORITY"
```

**Usage:**

```bash
./on-build-complete.sh 1234 success
```

## Example 7: Multi-repository setup

Setup multiple repositories with the same tool:

```bash
# Repository 1
cd C:\repos\project-a
copilotclimon init

# Repository 2
cd C:\repos\project-b
copilotclimon init

# Repository 3
cd C:\repos\project-c
copilotclimon init
```

Single Systray application serves all repositories. Notifications include repository name:

```
GitHub Copilot CLI
Migration completed
[project-a] main

GitHub Copilot CLI
Tests passed
[project-b] feature/testing

GitHub Copilot CLI
Build failed
[project-c] dev
```

## Example 8: Filtering events by priority

Configure notifications by priority level:

**File: `.copilotclimonitor/config.json`**

```json
{
  "eventFilters": {
    "task-completed": {
      "priority": "normal",
      "sound": true,
      "toastDuration": 5
    },
    "approval-required": {
      "priority": "high",
      "sound": true,
      "toastDuration": 10
    },
    "agent-waiting": {
      "priority": "low",
      "sound": false,
      "toastDuration": 3
    },
    "error": {
      "priority": "high",
      "sound": true,
      "toastDuration": 10
    }
  }
}
```

## Example 9: Quiet hours

Disable notifications during certain hours:

**File: `.copilotclimonitor/config.json`**

```json
{
  "quietHours": {
    "enabled": true,
    "start": "22:00",
    "end": "08:00",
    "suppressNotifications": true,
    "logToHistory": true
  }
}
```

During quiet hours:
- Notifications are suppressed
- Events are still logged to history
- Dashboard shows queued events

## Example 10: Webhook integration example

Forward events to external systems (future enhancement):

```bash
# Example: POST to webhook endpoint
copilotclimon notify \
  --event task-completed \
  --message "Task finished" \
  --webhook "https://your-webhook-endpoint.com/events"
```

This would enable:
- Slack integration
- Teams integration
- Discord integration
- Custom monitoring systems

## Example 11: Validation workflow

Typical validation workflow before committing:

```bash
# 1. Initialize monitoring
copilotclimon init

# 2. Validate setup
copilotclimon doctor

# 3. Send test notification
copilotclimon notify --event test --message "Test"

# 4. Run your Copilot CLI task
copilot task run "Your task here"

# 5. When complete, check history
copilotclimon open
```

## Example 12: Command reference

Quick reference for common commands:

```bash
# Installation
dotnet tool install -g ElBruno.CopilotCLIMonitor

# Start application
copilotclimon

# Initialize repository
copilotclimon init

# Test notifications
copilotclimon notify --event test --message "Test"

# Run diagnostics
copilotclimon doctor

# Open dashboard
copilotclimon open

# Show version
copilotclimon --version

# Uninstall
dotnet tool uninstall -g ElBruno.CopilotCLIMonitor
```
