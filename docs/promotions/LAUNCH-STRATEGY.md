# Launch & promotional strategy

This document outlines the launch strategy and promotional materials for ElBruno.CopilotCLIMonitor.

## Launch phases

### Phase 1: Soft launch (Internal testing)
- Release v0.1.0-alpha for early feedback
- Share with GitHub Copilot CLI users
- Gather usage feedback

### Phase 2: Beta launch
- Release v0.5.0-beta after improvements
- Announce on social media
- Publish launch blog post
- Share in developer communities

### Phase 3: Public release
- Release v1.0.0
- Full promotion campaign
- Documentation complete
- Support infrastructure ready

## Key messaging

### Primary message
"Stop babysitting long-running Copilot CLI tasks."

### Secondary messages
- "Get instant notifications when your AI workflows finish"
- "Minimize your terminal, stay informed with native notifications"
- "Monitor multiple repositories with a single Systray app"
- "Built for developers who use GitHub Copilot CLI"

### Value propositions
1. **Reclaim productivity** – Don't monitor terminal for hours
2. **Stay informed** – Get alerts when workflows finish
3. **Simple integration** – One command to set up per repository
4. **Professional UX** – Native Windows notifications, clean design
5. **Lightweight** – Minimal resource footprint
6. **Open source** – MIT licensed, no telemetry

## Social media templates

### Twitter/X

**Template 1 (28 days pre-launch):**

"Running Copilot CLI tasks for 2 hours while your terminal is minimized? 👀

Meet ElBruno.CopilotCLIMonitor 🚀

A tiny Windows Systray app that keeps you informed with native notifications when your AI workflows finish.

#dotnet #githubcopilot #ai"

**Template 2 (14 days pre-launch):**

"Tired of babysitting long AI tasks? ElBruno.CopilotCLIMonitor is coming soon.

✓ Native Windows notifications
✓ Repository hook integration  
✓ Event history tracking
✓ Zero configuration

Minimize your terminal. Maximize your productivity.

#dotnet #copilot #buildtools"

**Template 3 (Release day):**

"🎉 ElBruno.CopilotCLIMonitor v1.0.0 is live!

Install now:
```
dotnet tool install -g ElBruno.CopilotCLIMonitor
```

Get instant desktop notifications for Copilot CLI tasks. Never miss another workflow completion.

Get started: https://github.com/elbruno/ElBruno.CopilotCLIMonitor

#dotnet #copilot #oss"

### LinkedIn

**Title:** "Building a Windows Systray App for GitHub Copilot CLI"

**Content:**

"I'm excited to announce ElBruno.CopilotCLIMonitor, an open-source Windows desktop application for monitoring long-running GitHub Copilot CLI tasks.

**The Problem:**
Modern AI-assisted development often involves tasks that run for minutes or hours. Developers typically minimize the terminal and lose visibility into task completion, approvals, and errors.

**The Solution:**
ElBruno.CopilotCLIMonitor bridges GitHub Copilot CLI hooks and Windows native notifications. Initialize a repository, then receive desktop alerts automatically.

**Features:**
- Native Windows toast notifications
- Event history dashboard
- Per-repository hook configuration  
- Minimal resource footprint
- MIT open source

**Tech stack:**
- .NET 10 & WPF
- GitHub Copilot CLI integration
- Windows Notifications API
- GitHub Actions for CI/CD

Available now: https://github.com/elbruno/ElBruno.CopilotCLIMonitor

#dotnet #github #opensource #developertoolsbuilding #windowsdevelopment"

### GitHub Discussions

**Title:** "Introducing ElBruno.CopilotCLIMonitor"

"Hi everyone! 👋

I'm happy to share ElBruno.CopilotCLIMonitor, a new open-source tool for Windows developers using GitHub Copilot CLI.

**Problem:** Running long Copilot CLI tasks (migrations, test generation, refactoring) often takes 20 minutes to 2 hours. You minimize the terminal to work on other things, then forget about the task and miss notifications.

**Solution:** A lightweight Systray app that monitors Copilot CLI hooks and sends Windows notifications when tasks complete, fail, or need approval.

**Installation:**
```bash
dotnet tool install -g ElBruno.CopilotCLIMonitor
```

**Setup:**
```bash
copilotclimonitor        # Start the app
cd your-repo
copilotclimonitor init   # Configure your repository
```

**Features:**
- ✅ Native Windows toast notifications
- ✅ Event history dashboard
- ✅ Repository-level configuration
- ✅ Multi-repository support
- ✅ Lightweight and fast
- ✅ MIT licensed

**Feedback welcome!**
- Issues: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/issues
- Discussions: https://github.com/elbruno/ElBruno.CopilotCLIMonitor/discussions

What features would be most valuable for you?"

## Blog post outline

### Title
"Building a Windows Systray App for Copilot CLI: Notifications, Hooks, and Modern .NET"

### Sections

1. **Introduction** (300 words)
   - Problem statement
   - Solution overview
   - Tech stack preview

2. **The problem** (400 words)
   - Why monitoring long tasks is hard
   - Current Copilot CLI workflow
   - Developer productivity impact

3. **Architecture** (800 words)
   - System design overview
   - Systray component details
   - Hook integration model
   - Inter-process communication
   - Diagram: Event flow

4. **Building the Systray app** (600 words)
   - WPF for native Windows UI
   - Toast notification API
   - Background process management
   - Settings persistence

5. **Hook integration** (500 words)
   - Copilot CLI hook model
   - Hook installation
   - Event forwarding
   - Repository configuration

6. **Publishing strategy** (300 words)
   - .NET Tool distribution
   - NuGet publishing
   - Trusted Publisher setup
   - GitHub Actions automation

7. **Getting started** (200 words)
   - Installation steps
   - Repository setup
   - First notification test

8. **Open source and future** (200 words)
   - MIT licensing philosophy
   - Contribution opportunities
   - Planned v2 features
   - Call to action

### Publishing platforms

- **Dev.to** – Developer community
- **Medium** – Broader audience
- **Personal blog** – elbruno.com
- **Microsoft Tech Community** – .NET angle
- **CSS Tricks/Web Dev Community** – Cross-promotion (even if not web-specific)

## Demo script

### Scenario: Long-running migration task

1. **Setup (30 seconds)**
   - Show minimized terminal
   - Show Systray application running
   - Show empty event history

2. **Trigger event (1 minute)**
   - Start Copilot CLI task: "Generate migration for UserService"
   - Terminal shows task running
   - Minimize terminal
   - Continue with other work (show IDE)

3. **Notification** (30 seconds)
   - While "working on something else," show toast notification appear
   - "Copilot CLI: Migration completed successfully"
   - Show notification includes repository and branch name

4. **Review history** (30 seconds)**
   - Click Systray → "Open dashboard"
   - Show event history with multiple tasks
   - Show filtering by repository
   - Show event details

5. **Settings** (30 seconds)**
   - Show Settings → Notification preferences
   - Show quiet hours configuration
   - Show per-repository filtering

**Total runtime:** ~3 minutes

## Content assets needed

### Images
- [ ] Product icon (256x256, 128x128, 64x64)
- [ ] Systray screenshot
- [ ] Settings window screenshot
- [ ] Notification example (with repository name)
- [ ] Architecture diagram
- [ ] Dashboard screenshot

### Video
- [ ] 60-second demo video
- [ ] Feature walkthrough (3-5 minutes)

### Graphics
- [ ] Social media cards (Twitter, LinkedIn)
- [ ] Hero image for blog post
- [ ] Badge/logo for README

**Generation tool:** Use ElBruno.Text2Image (`t2i` CLI) for AI-generated images

## Promotional timeline

### 4 weeks before launch
- [ ] Draft blog post
- [ ] Create product icon
- [ ] Record demo video
- [ ] Design social cards

### 2 weeks before launch
- [ ] Finalize blog post
- [ ] Generate promotional images
- [ ] Create launch announcement
- [ ] Reach out to communities

### Launch day
- [ ] Publish blog post
- [ ] Post on Twitter/X
- [ ] Post on LinkedIn
- [ ] Share in GitHub Discussions
- [ ] Announce on dev.to
- [ ] Create GitHub Release

### Week 1 post-launch
- [ ] Share demo video
- [ ] Engage with comments/questions
- [ ] Gather community feedback

### Ongoing
- [ ] Monitor GitHub issues
- [ ] Engage in discussions
- [ ] Share user stories
- [ ] Update docs based on feedback

## Community engagement

### Where to share
- GitHub Copilot CLI discussions
- .NET community forums
- r/csharp (Reddit)
- r/github (Reddit)
- Hacker News (if appropriate)
- DEV Community
- Twitter #dotnet #copilot #oss

### Call to action
- "Try it and let me know what you think"
- "Feedback welcome"
- "Help wanted: [specific contribution]"
- "Interested in extending with [feature]?"

## Metrics to track

- [ ] GitHub Stars
- [ ] NuGet Downloads
- [ ] Issues and Pull Requests
- [ ] Social media engagement
- [ ] Blog post views
- [ ] Discussion participation
- [ ] GitHub Releases downloads

## Success criteria

- ✅ 100+ GitHub stars in first month
- ✅ 1,000+ NuGet downloads in first month
- ✅ 10+ community discussions/issues
- ✅ 500+ blog post views
- ✅ 1+ community contribution PR

---

**Next step:** Generate promotional images using ElBruno.Text2Image with these prompts:

1. "A Windows desktop Systray notification icon for GitHub Copilot CLI monitoring application"
2. "A WPF Windows app dashboard showing notification history for long-running AI tasks"
3. "Hero image: minimized terminal with notification popup from desktop notification system"
