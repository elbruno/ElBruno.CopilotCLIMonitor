# Hook configuration

## Overview

Repository hooks are the bridge between GitHub Copilot CLI and the Systray notification application. After running `copilotclimon init`, your repository contains hook scripts that forward events automatically.

## Hook directory structure

After initialization, your repository contains:

```text
.copilotclimonitor/
├── config.json
├── hooks/
│   ├── on-task-completed.sh
│   ├── on-error.sh
│   ├── on-approval-required.sh
│   └── on-agent-waiting.sh
└── logs/
```

## Default hooks

### on-task-completed.sh

Fires when a Copilot CLI task completes successfully.

**Default behavior:**

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)

copilotclimon notify \
  --event task-completed \
  --message "Task completed successfully" \
  --repository "$REPO" \
  --branch "$BRANCH"
```

**Customization example:**

Add task-specific information:

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
TASK_NAME="${1:-Unknown}"

copilotclimon notify \
  --event task-completed \
  --message "✓ $TASK_NAME completed successfully" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-task-completed"
```

### on-error.sh

Fires when a Copilot CLI task encounters an error.

**Default behavior:**

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
ERROR_MESSAGE="${1:-Unknown error}"

copilotclimon notify \
  --event error \
  --message "Error: $ERROR_MESSAGE" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-error"
```

### on-approval-required.sh

Fires when a task requires user approval.

**Default behavior:**

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)

copilotclimon notify \
  --event approval-required \
  --message "Approval required for task execution" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-approval-required"
```

### on-agent-waiting.sh

Fires when an agent is waiting for user input.

**Default behavior:**

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)

copilotclimon notify \
  --event agent-waiting \
  --message "Agent is waiting for your input" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-agent-waiting"
```

## Customizing hooks

### Example 1: Enhanced error hook

Add timestamp and error code:

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
ERROR_CODE="${1:-0}"
ERROR_MESSAGE="${2:-Unknown error}"
TIMESTAMP=$(date -u +%Y-%m-%dT%H:%M:%SZ)

copilotclimon notify \
  --event error \
  --message "[$ERROR_CODE] $ERROR_MESSAGE" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-error:$TIMESTAMP"
```

### Example 2: Slack integration

Extend on-task-completed.sh to post to Slack:

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
SLACK_WEBHOOK="${SLACK_WEBHOOK_URL:-}"

# Send to ElBruno.CopilotCLIMonitor
copilotclimon notify \
  --event task-completed \
  --message "Task completed successfully" \
  --repository "$REPO" \
  --branch "$BRANCH"

# Optionally forward to Slack (if webhook is configured)
if [ -n "$SLACK_WEBHOOK" ]; then
  curl -X POST "$SLACK_WEBHOOK" \
    -H 'Content-Type: application/json' \
    -d "{
      \"text\": \"✓ $REPO task completed on $BRANCH\"
    }"
fi
```

### Example 3: Duration tracking

Log task duration:

```bash
#!/bin/bash
REPO=$(basename $(git rev-parse --show-toplevel))
BRANCH=$(git rev-parse --abbrev-ref HEAD)
START_TIME="${1:-0}"
END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))
DURATION_MIN=$((DURATION / 60))

copilotclimon notify \
  --event task-completed \
  --message "✓ Completed in ${DURATION_MIN}m" \
  --repository "$REPO" \
  --branch "$BRANCH"
```

## Hook environment variables

Hooks have access to these environment variables:

| Variable | Description |
|----------|-------------|
| `HOME` | User home directory |
| `PWD` | Current working directory |
| `SHELL` | User shell |
| `USER` | Current username |
| `GIT_DIR` | Git directory path |
| `COPILOT_TASK_ID` | Current Copilot CLI task ID (if available) |

## Secure event routing

Use `--source` to tag where events originate and optionally require an IPC token.

```bash
export COPILOTCLIMON_IPC_TOKEN="your-shared-secret"

copilotclimon notify \
  --event task-completed \
  --message "Task completed successfully" \
  --repository "$REPO" \
  --branch "$BRANCH" \
  --source "hook:on-task-completed"
```

When `COPILOTCLIMON_IPC_TOKEN` is set in the monitor process, only requests with the same token are accepted.

## Configuration file

Configure hook behavior in `.copilotclimonitor/config.json`:

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
      "sound": true,
      "duration": 5
    },
    "error": {
      "priority": "high",
      "sound": true,
      "duration": 10
    },
    "approval-required": {
      "priority": "high",
      "sound": true,
      "duration": 10
    }
  },
  "quiet-hours": {
    "enabled": false,
    "start": "22:00",
    "end": "08:00"
  }
}
```

## Disabling hooks

To disable a specific hook:

1. Edit `.copilotclimonitor/config.json`
2. Set `"hook-name": false`

Example:

```json
{
  "hooks": {
    "task-completed": true,
    "agent-waiting": false
  }
}
```

To disable all notifications:

```json
{
  "enabled": false
}
```

## Hook debugging

### Enable debug output

Create a debug version of your hook:

```bash
#!/bin/bash
set -x  # Enable debug output

REPO=$(basename $(git rev-parse --show-toplevel))
echo "DEBUG: Repository: $REPO" >&2

copilotclimon notify \
  --event task-completed \
  --message "Task completed"
```

### Test hook manually

```bash
# Test on-task-completed hook
bash .copilotclimonitor/hooks/on-task-completed.sh

# Test with arguments
bash .copilotclimonitor/hooks/on-error.sh "Build timeout error"
```

### Check hook logs

Logs are stored in:

```text
.copilotclimonitor/logs/
```

View recent logs:

```bash
tail -20 .copilotclimonitor/logs/*.log
```

## Adding custom hooks

To add custom hooks beyond the defaults:

1. Create a new script in `.copilotclimonitor/hooks/`
2. Make it executable: `chmod +x on-custom-event.sh`
3. Call from your custom Copilot CLI tasks
4. Add to configuration if needed

Example custom hook:

```bash
#!/bin/bash
# .copilotclimonitor/hooks/on-build-complete.sh

REPO=$(basename $(git rev-parse --show-toplevel))
BUILD_TIME=$1
BUILD_STATUS=$2

copilotclimon notify \
  --event build-completed \
  --message "Build: $BUILD_STATUS in ${BUILD_TIME}s" \
  --repository "$REPO"
```

Usage:

```bash
bash .copilotclimonitor/hooks/on-build-complete.sh 1234 success
```

## Common patterns

### Pattern 1: Silent failure

Don't notify on minor events:

```bash
#!/bin/bash
# Only notify if important flag is set
if [ "${NOTIFY_ALL:-0}" == "1" ]; then
  copilotclimon notify --event task-completed --message "Done"
fi
```

### Pattern 2: Conditional message

Different messages based on outcome:

```bash
#!/bin/bash
OUTCOME=$1

if [ "$OUTCOME" == "success" ]; then
  MSG="✓ All tests passed"
else
  MSG="✗ Tests failed"
fi

copilotclimon notify --event test-completed --message "$MSG"
```

### Pattern 3: Rate limiting

Prevent notification spam:

```bash
#!/bin/bash
LOCKFILE=".copilotclimonitor/.last-notify"
COOLDOWN=60  # seconds

if [ -f "$LOCKFILE" ]; then
  LAST=$(cat "$LOCKFILE")
  NOW=$(date +%s)
  ELAPSED=$((NOW - LAST))
  if [ $ELAPSED -lt $COOLDOWN ]; then
    exit 0  # Skip notification
  fi
fi

copilotclimon notify --event task-completed --message "Done"
date +%s > "$LOCKFILE"
```

## Verification

After customizing hooks, verify they work:

```bash
# Run diagnostics
copilotclimon doctor

# Test a hook manually
bash .copilotclimonitor/hooks/on-task-completed.sh

# Check logs for errors
cat .copilotclimonitor/logs/*.log
```
