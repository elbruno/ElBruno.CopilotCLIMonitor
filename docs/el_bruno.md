# ElBruno.copilotclimon – Product Requirements Document (PRD)

## Overview

`ElBruno.copilotclimon` is a Windows-first developer productivity tool designed to monitor long-running GitHub Copilot CLI tasks and provide rich desktop notifications through a lightweight system tray (Systray) application.

The product is inspired by the structure, publishing model, and user experience patterns used in the existing repository:

- https://github.com/elbruno/ElBruno.AspireMonitor

The main goal is to help developers who run long AI-assisted development sessions using GitHub Copilot CLI and want to minimize the terminal while still being aware of important events such as:

- Task completed
- Agent waiting for input
- Error occurred
- Approval required
- Long-running task finished
- Build/test execution completed
- Multi-agent workflow status changes

The application should provide a native Windows desktop experience with a clean WPF-based Systray application, customizable notifications, and repository-level hook integration.

---

# Vision

Modern AI-assisted development workflows increasingly involve long-running autonomous or semi-autonomous tasks.

Examples:

- GitHub Copilot CLI
- SQUAD
- Multi-agent repository orchestration
- Large repository analysis
- AI-assisted migrations
- Test generation
- Documentation generation
- Long-running builds
- Local LLM execution via Ollama
- Azure OpenAI agent orchestration

These workflows can run from several minutes to multiple hours.

Developers often:

- Minimize the terminal
- Continue working on other tasks
- Lose visibility into the workflow status
- Miss approvals or task completion events

`ElBruno.copilotclimon` solves this by creating a desktop notification bridge between GitHub Copilot CLI hooks and a native Windows monitoring application.

---

# Main Goals

## Primary Goal

Provide real-time Windows desktop notifications for GitHub Copilot CLI events.

## Secondary Goals

- Improve long-running AI workflow usability
- Provide a clean developer experience
- Minimize terminal babysitting
- Create a reusable hook integration system
- Deliver a polished Windows Systray experience
- Support future expansion into multi-agent monitoring
- Publish as a reusable .NET Tool
- Demonstrate modern .NET desktop engineering patterns

---

# Non Goals

The following are explicitly NOT part of v1:

- Cross-platform desktop support
- Linux tray applications
- macOS menu bar support
- Real-time terminal embedding
- GitHub Copilot CLI modification
- Cloud-hosted monitoring
- Telemetry collection
- Remote monitoring dashboards
- Authentication systems
- Web applications
- Browser extensions
- AI model execution
- Terminal replacement

---

# Target Audience

## Primary Audience

Developers using:

- GitHub Copilot CLI
- SQUAD
- Multi-agent workflows
- Long-running AI tasks
- .NET AI workflows
- Aspire-based orchestration
- Ollama
- Azure OpenAI

## Secondary Audience

Developers interested in:

- WPF Systray apps
- Developer tooling
- AI workflow UX
- Notification systems
- .NET desktop applications

---

# High-Level Architecture

The solution contains two major components.

## Component 1 – Systray Application

A native WPF Windows application that:

- Lives in the Windows Systray
- Receives notifications/events
- Displays toast notifications
- Manages settings
- Tracks recent events
- Exposes configuration options
- Runs in the background

### Responsibilities

- Notification display
- Event routing
- User preferences
- Event history
- Status indicators
- Tray icon interaction
- Toast notification integration

---

## Component 2 – Repository Hook Integration

Repository-level integration installed via CLI command.

The integration:

- Installs GitHub Copilot CLI hooks
- Registers event handlers
- Sends events to the Systray app
- Supports repository-local configuration

### Responsibilities

- Hook installation
- Hook updates
- Hook validation
- Event forwarding
- Repository initialization

---

# Hook Model

GitHub Copilot CLI already exposes hook support.

The application will leverage:

- notification hooks
- shell completion hooks
- task completion hooks
- approval-required hooks
- error hooks

Hooks are repository-scoped.

This is important because:

- Each repository can define different behaviors
- Users may only want notifications in selected repositories
- Different repos may use different workflows

---

# Repository-Level Workflow

## Example Initialization

```bash
copilotclimon init
```

Expected behavior:

- Detect current repository
- Create required hook configuration
- Register event forwarding scripts
- Validate monitor availability
- Create local config if needed
- Show validation summary

---

## Example Runtime Flow

### Step 1

User starts a long Copilot CLI task.

### Step 2

Copilot CLI triggers a repository hook.

### Step 3

Hook executes:

```bash
copilotclimon notify --event task-completed --message "Migration finished"
```

### Step 4

Systray application receives event.

### Step 5

Application displays:

- Windows Toast Notification
- Tray popup
- Sound (optional)
- Event history entry

---

# Technology Stack

## Main Application

- .NET 10
- WPF
- Windows Notifications
- Systray integration

## CLI Integration

- .NET Tool
- Command-line parsing
- JSON configuration

## Packaging

- NuGet package
- Trusted Publisher
- GitHub Actions

## Documentation

- Markdown
- Structured docs folder

## Promotional Assets

- ElBruno.Text2Image
- `t2i` CLI integration

---

# Solution Structure

The repository structure should follow the same clean organizational principles used in:

- https://github.com/elbruno/ElBruno.AspireMonitor

---

# Repository Structure

```text
/README.md
/LICENSE
/images
/docs
/docs/promotions
/src
/tests
```

---

# Root Folder Rules

Only these files should exist at repository root:

- README.md
- LICENSE
- solution file
- minimal repository metadata

Avoid:

- loose scripts
- random markdown files
- temporary docs
- duplicated assets

---

# Docs Folder Structure

## /docs

Contains:

- architecture
- setup guides
- CLI usage
- troubleshooting
- notification examples
- publishing docs
- hook examples

## /docs/promotions

Contains:

- blog drafts
- social posts
- image prompts
- launch materials
- LinkedIn copy
- Twitter/X copy
- demo scripts

---

# Images Folder

Contains:

- NuGet logos
- application icons
- screenshots
- promotional images
- social cards
- blog images

All generated via:

- ElBruno.Text2Image
- `t2i`

---

# Source Code Structure

## /src

Main projects only.

Example:

```text
/src
  /ElBruno.copilotclimon
  /ElBruno.copilotclimon.Core
  /ElBruno.copilotclimon.Hooks
```

---

# Tests Structure

```text
/tests
  /ElBruno.copilotclimon.Tests
```

---

# Application Features

# Feature – Systray Integration

## Requirements

- Native Windows Systray app
- Persistent tray icon
- Context menu
- Background execution
- Auto-start optional

## Tray Menu

### Suggested Menu

- Open Dashboard
- Recent Events
- Settings
- Enable Notifications
- Quiet Mode
- Open Logs Folder
- Open Repository
- About
- Exit

---

# Feature – Toast Notifications

## Requirements

- Native Windows toast notifications
- Configurable behavior
- Optional sound
- Priority levels
- Action buttons

## Notification Types

### Informational

Examples:

- task completed
- workflow completed
- build successful

### Warning

Examples:

- waiting for approval
- long-running task warning

### Error

Examples:

- execution failed
- hook failed
- timeout

---

# Feature – Event History

## Requirements

- Track recent events
- Store timestamps
- Allow filtering
- Allow clearing history

## Future Enhancements

- Search
- Export logs
- Event tagging

---

# Feature – Settings Window

## Requirements

Allow users to configure:

- Notification type
- Toast duration
- Sound enabled
- Quiet hours
- Repository filters
- Event filtering
- Startup behavior
- Logging level

---

# Feature – Hook Installer

## Requirements

Command:

```bash
copilotclimon init
```

Should:

- Detect repository
- Install hooks
- Validate Copilot CLI support
- Validate permissions
- Configure event routing

---

# Feature – Repository Awareness

Notifications should include:

- Repository name
- Current branch
- Event source
- Event type

Example:

```text
[OpenClaw.NetAgent]
Migration task completed successfully
```

---

# Feature – Logging

## Requirements

- Structured logging
- Local log files
- Error tracking
- Debug mode

---

# Feature – Multi-Agent Awareness (Future)

Potential future support for:

- SQUAD agent names
- Multi-agent progress
- Agent status indicators
- Agent-specific notifications

---

# UI Design Principles

## Design Goals

- Minimalistic
- Clean
- Native Windows feel
- Low distraction
- Fast access
- Developer focused

## UX Goals

- Zero-friction setup
- Quiet background behavior
- Clear notifications
- Easy repository initialization

---

# Notification UX Ideas

## Example Behaviors

### Task Completed

Toast:

```text
GitHub Copilot CLI
Migration completed successfully
```

### Approval Required

Toast:

```text
Approval required for command execution
```

### Error

Toast:

```text
Task failed after 2h 14m
```

---

# CLI Commands

## Main Commands

### Initialize Repository

```bash
copilotclimon init
```

### Send Notification

```bash
copilotclimon notify
```

### Validate Setup

```bash
copilotclimon doctor
```

### Open Dashboard

```bash
copilotclimon open
```

### Show Version

```bash
copilotclimon --version
```

---

# Configuration

## Global Configuration

Stored under:

```text
%AppData%\ElBruno\copilotclimon
```

## Repository Configuration

Stored locally in repository.

Potential location:

```text
.copilotclimon
```

---

# Publishing Strategy

The repository should follow the same publishing patterns as:

- https://github.com/elbruno/ElBruno.AspireMonitor

---

# NuGet Publishing

## Requirements

- Trusted Publisher
- GitHub Actions
- OIDC authentication
- Automated publishing
- Semantic versioning

---

# Package Types

## .NET Tool

Main distribution method.

Example installation:

```bash
dotnet tool install -g ElBruno.copilotclimon
```

---

# GitHub Actions

## CI Requirements

- build
- test
- package
- publish
- validation

---

# README Requirements

The README should include:

- badges
- install instructions
- screenshots
- demo GIFs
- quick start
- architecture diagram
- hook examples
- troubleshooting
- contribution guide

---

# README Badges

Suggested badges:

- NuGet version
- Downloads
- Build status
- License
- .NET version

---

# Documentation Requirements

## Setup Guide

Explain:

- install .NET tool
- run app
- initialize repository
- validate hooks
- test notifications

---

# Security Considerations

## Requirements

- No elevated privileges
- No telemetry by default
- No external service dependencies
- Local-only event handling
- No cloud sync

---

# Performance Goals

## Requirements

- Lightweight memory footprint
- Minimal CPU usage
- Fast startup
- Low notification latency

---

# Accessibility Goals

## Requirements

- Keyboard accessible
- High contrast friendly
- Screen-reader friendly where possible

---

# Future Enhancements

## Potential v2 Features

- Multi-platform support
- Web dashboard
- Multi-machine sync
- Rich workflow analytics
- AI-generated summaries
- Notification grouping
- Teams integration
- Discord integration
- Mobile notifications
- OpenTelemetry integration
- Agent timeline visualization

---

# Risks

## Technical Risks

- Copilot CLI hook changes
- Windows notification API changes
- WPF tray limitations
- Background execution edge cases

---

# Open Questions

## Architecture

- How should inter-process communication work?
- Named pipes?
- Local HTTP endpoint?
- File-based queue?

## Notifications

- Toast only?
- Tray popup fallback?
- Multiple notification providers?

## Hook Strategy

- Centralized hook template?
- Repository overrides?

---

# Proposed IPC Options

## Option 1 – Local HTTP Endpoint

Pros:

- Simple
- Flexible
- Easy debugging

Cons:

- Port management

---

## Option 2 – Named Pipes

Pros:

- Native Windows
- Lightweight
- Secure local communication

Cons:

- More implementation complexity

---

## Recommendation

Start with:

- Local HTTP endpoint

Reason:

- Simpler development
- Easier debugging
- Easier CLI integration

---

# Example Workflow

## Install

```bash
dotnet tool install -g ElBruno.copilotclimon
```

## Start App

```bash
copilotclimon
```

## Initialize Repository

```bash
cd myrepo
copilotclimon init
```

## Run Copilot CLI

```bash
copilot task run
```

## Receive Notification

Windows toast appears automatically.

---

# ElBruno.Text2Image Integration

The project should leverage:

- ElBruno.Text2Image
- `t2i`

For:

- NuGet logo generation
- App icon generation
- Social media assets
- Blog post images
- Demo images

---

# Promotional Strategy

## Blog Posts

Potential topics:

- Build a Systray app with WPF
- Add notifications to GitHub Copilot CLI
- AI workflows on Windows
- Long-running AI task monitoring
- Developer UX for AI agents

---

# Social Promotion

## Platforms

- LinkedIn
- Twitter/X
- Bluesky
- GitHub

---

# Example Launch Messaging

## Key Message

"Stop babysitting long-running Copilot CLI tasks."

---

# Sample Twitter/X Messaging

"Running Copilot CLI tasks for 2 hours while your terminal is minimized? 👀

Meet ElBruno.copilotclimon 🚀

A tiny Windows Systray app that keeps you informed with native notifications when your AI workflows finish.

#dotnet #githubcopilot #ai"

---

# Demo Ideas

## Demo 1

Run a long migration.

Show:

- minimized terminal
- toast notification on completion
- tray history

---

# MVP Scope

## Included in MVP

- Windows Systray app
- Toast notifications
- Repository hook installer
- Event forwarding
- Settings UI
- Event history
- Logging
- NuGet publishing
- GitHub Actions
- Basic docs

## Excluded from MVP

- Cross-platform support
- Cloud dashboards
- Mobile notifications
- Teams integration
- Analytics

---

# Success Criteria

## Technical Success

- Easy installation
- Reliable notifications
- Stable background execution
- Minimal resource usage

## UX Success

- Users can minimize terminal safely
- Users receive notifications consistently
- Repository setup takes less than 1 minute

---

# Engineering Standards

## Repository Standards

- Clean repository root
- Consistent naming
- Structured docs
- Structured images
- Source under `/src`
- Tests under `/tests`

## Code Standards

- Modern .NET patterns
- DI-based architecture
- MVVM for WPF
- Structured logging
- Async-first design

---

# Final Vision

`ElBruno.copilotclimon` should feel like:

- a polished developer utility
- a natural extension of GitHub Copilot CLI workflows
- a modern .NET desktop tool
- a practical AI productivity enhancer

The product should prioritize:

- simplicity
- reliability
- low friction
- clean UX
- strong developer experience

while showcasing:

- modern .NET desktop development
- GitHub Copilot CLI extensibility
- AI-assisted workflow tooling
- reusable developer infrastructure

