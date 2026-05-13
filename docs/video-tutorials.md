# Video tutorials

This guide defines the official screen recordings for installation and usage walkthroughs.

## Planned tutorials

1. **Quick start (2-3 min)**
   - Install with `dotnet tool install -g ElBruno.CopilotCLIMonitor.Cli`
   - Start monitor with `copilotclimon start`
   - Send sample event with `copilotclimon notify`

2. **Hook setup (4-6 min)**
   - Run `copilotclimon hook --install`
   - Review generated `settings.json`
   - Trigger a hook event and inspect dashboard history

3. **Settings and preferences (3-4 min)**
   - Open tray settings
   - Configure quiet hours and logging level
   - Enable optional telemetry

4. **Troubleshooting (3-5 min)**
   - Check health endpoint
   - Validate IPC token configuration
   - Resolve common port conflicts

## Recording standards

- Resolution: **1920x1080**
- Frame rate: **30 fps**
- Theme: use default Windows theme and standard terminal font size
- Audio: optional voiceover, but captions are required
- Keep secrets out of recordings (tokens, usernames, private repo names)

## File naming convention

Store source captures and edited clips under `videos/` using:

- `01-quick-start.mp4`
- `02-hook-setup.mp4`
- `03-settings-preferences.mp4`
- `04-troubleshooting.mp4`

## Publishing checklist

1. Verify commands match current CLI options.
2. Confirm all UI steps reflect latest release.
3. Add links to videos in README and user manual.
4. Re-export if any sensitive value appears on-screen.
