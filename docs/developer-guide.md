# Developer guide

This guide is the single entry point for contributors working on ElBruno.CopilotCLIMonitor.

## 1. Development setup

Use the setup instructions in [setup.md](setup.md) for local prerequisites, install, and repository initialization.

## 2. Contributing workflow

Follow [../CONTRIBUTING.md](../CONTRIBUTING.md) for:
- branch strategy
- coding conventions
- testing expectations
- commit format

## 3. Architecture deep dive

Read [architecture.md](architecture.md) for system design, component responsibilities, and data flow.

## 4. Local build and test loop

From repository root:

```powershell
dotnet build -v minimal
dotnet test -v minimal
```

## 5. Core development areas

- **CLI entrypoint:** `src/ElBruno.CopilotCLIMonitor.Cli/Program.cs`
- **CLI handlers:** `src/ElBruno.CopilotCLIMonitor.Cli/Handlers/`
- **IPC contracts + shared models:** `src/ElBruno.CopilotCLIMonitor.Core/Models/`
- **Core services:** `src/ElBruno.CopilotCLIMonitor.Core/Services/`
- **Systray app:** `src/ElBruno.CopilotCLIMonitor/`
- **Tests:** `tests/ElBruno.CopilotCLIMonitor.Tests/`

## 6. Security and release references

- Hook security + routing: [hook-configuration.md](hook-configuration.md)
- Versioning strategy: [versioning.md](versioning.md)
- NuGet publishing flow: [nuget-publishing.md](nuget-publishing.md)
