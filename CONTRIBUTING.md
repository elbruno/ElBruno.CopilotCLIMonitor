# Contributing

We welcome contributions to ElBruno.CopilotCLIMonitor! This guide explains how to contribute code, documentation, and bug reports.

## Code of conduct

Please read our Code of Conduct before contributing. We are committed to providing a welcoming and inclusive environment.

## Getting started

### Prerequisites

- **Windows 10/11** – Required for Systray development
- **.NET 10 SDK** – [Download .NET 10](https://dotnet.microsoft.com/download)
- **Git** – Version control
- **Visual Studio 2022 Community** or **Visual Studio Code** – Recommended IDE

### Clone the repository

```bash
git clone https://github.com/elbruno/ElBruno.CopilotCLIMonitor.git
cd ElBruno.CopilotCLIMonitor
```

### Build from source

```bash
dotnet build
```

### Run tests

```bash
dotnet test
```

### Run the application locally

```bash
dotnet run --project src/ElBruno.CopilotCLIMonitor
```

## Development workflow

### 1. Create a feature branch

```bash
git checkout -b feature/your-feature-name
```

Branch naming convention: `feature/`, `bugfix/`, `docs/`, or `test/` prefix.

### 2. Make your changes

Follow the code style guidelines below.

### 3. Write tests

Add tests for new functionality:

```bash
# Add test to tests/ElBruno.CopilotCLIMonitor.Tests
# Run tests
dotnet test
```

### 4. Commit your changes

Use clear, descriptive commit messages:

```bash
git commit -m "feat: add support for custom hook events"
```

Follow conventional commit format: `type(scope): subject`

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

### 5. Push and create a Pull Request

```bash
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub.

## Code style

### C# coding standards

- **Language features** – Use modern C# (async/await, LINQ, records where appropriate)
- **Naming** – PascalCase for classes/methods, camelCase for variables/parameters
- **Indentation** – 4 spaces (no tabs)
- **Line length** – Target 120 characters max
- **Braces** – Allman style (opening brace on new line)

Example:

```csharp
public class NotificationService
{
    private readonly ILogger _logger;

    public NotificationService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task SendNotificationAsync(Notification notification)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        _logger.LogInformation("Sending notification: {Message}", notification.Message);
        // Implementation...
    }
}
```

### File organization

- One class per file
- File name matches class name (PascalCase)
- Namespace matches directory structure

### Documentation

- Use XML documentation comments for public APIs
- Include description, parameters, returns, exceptions

Example:

```csharp
/// <summary>
/// Sends a notification to the Systray application.
/// </summary>
/// <param name="notification">The notification to send.</param>
/// <returns>A task representing the asynchronous operation.</returns>
/// <exception cref="ArgumentNullException">Thrown when notification is null.</exception>
public async Task SendNotificationAsync(Notification notification)
{
    // Implementation...
}
```

## Architecture guidelines

### Project structure

```text
/src
  /ElBruno.CopilotCLIMonitor          // Systray application (WPF)
  /ElBruno.CopilotCLIMonitor.Core     // Shared business logic
  /ElBruno.CopilotCLIMonitor.Hooks    // Hook installation logic
  /ElBruno.CopilotCLIMonitor.Cli      // Command-line interface

/tests
  /ElBruno.CopilotCLIMonitor.Tests    // Unit and integration tests
```

### Dependency injection

Use Microsoft.Extensions.DependencyInjection:

```csharp
var services = new ServiceCollection();
services.AddSingleton<INotificationService, NotificationService>();
services.AddLogging();

var provider = services.BuildServiceProvider();
```

### Logging

Use ILogger from Microsoft.Extensions.Logging:

```csharp
_logger.LogInformation("Task completed: {TaskId}", taskId);
_logger.LogError(exception, "Failed to send notification");
_logger.LogDebug("Configuration loaded from {Path}", configPath);
```

### Async patterns

- Use `async/await` for I/O operations
- Avoid `.Result` and `.Wait()`
- Return `Task` or `Task<T>` from async methods

### Error handling

- Throw `ArgumentNullException` for null arguments
- Use custom exceptions for domain errors
- Log errors with context information

## Testing guidelines

### Unit tests

Test single components in isolation:

```csharp
[TestClass]
public class NotificationServiceTests
{
    [TestMethod]
    public async Task SendNotificationAsync_WithValidNotification_SucceedsAsync()
    {
        // Arrange
        var service = new NotificationService(mockLogger);
        var notification = new Notification { Message = "Test" };

        // Act
        await service.SendNotificationAsync(notification);

        // Assert
        mockLogger.Verify(x => x.Log(...), Times.Once);
    }
}
```

### Integration tests

Test component interactions:

```csharp
[TestClass]
public class RepositoryHookTests
{
    [TestMethod]
    public async Task InitRepository_CreatesHookDirectory()
    {
        // Arrange
        var hookInstaller = new HookInstaller();
        var tempRepo = CreateTemporaryRepository();

        // Act
        await hookInstaller.InitAsync(tempRepo);

        // Assert
        Assert.IsTrue(Directory.Exists(Path.Combine(tempRepo, ".copilotclimonitor")));
    }
}
```

### Test naming

Use descriptive names:

```csharp
// Good
SendNotificationAsync_WithNullNotification_ThrowsArgumentNullException

// Bad
TestSendNotification
```

## Documentation guidelines

### Markdown style

Follow Microsoft Style Guide:

- **Sentence-case headings** – "Getting started" not "Getting Started"
- **Active voice** – "Run the command" not "The command should be run"
- **Second person** – "You can configure..." not "Users can configure..."
- **Present tense** – "The system processes..." not "The system will process..."
- **No ampersands in prose** – "and" not "&" (except in code/brand names)

### File structure

```markdown
# Main heading

Overview paragraph (2-3 sentences).

## Section 1

Content with examples.

## Section 2

More content.

---

## Related

- [Link 1](...)
- [Link 2](...)
```

## Submitting changes

### Before submitting

1. **Run tests** – Ensure all tests pass
2. **Check code style** – Use IDE formatting/linting
3. **Update documentation** – If applicable
4. **Test locally** – Verify functionality works

### Pull Request checklist

- [ ] Tests pass locally
- [ ] Code follows style guidelines
- [ ] Documentation updated (if needed)
- [ ] Commit messages follow convention
- [ ] PR description explains changes
- [ ] No breaking changes to public API
- [ ] Windows Notification APIs tested (if modified)

### PR description template

```markdown
## Description
Brief explanation of changes.

## Related issue
Fixes #123

## Testing
How to test these changes.

## Breaking changes
Any breaking changes to public APIs.
```

## Release process

Only project maintainers (ElBruno) trigger releases.

If you're a maintainer:

1. Update version in `.csproj` files
2. Update `CHANGELOG.md`
3. Commit with message: `chore: release v1.0.0`
4. Tag commit: `git tag v1.0.0`
5. Push tag: `git push origin v1.0.0`
6. GitHub Actions automatically publishes to NuGet

## Reporting bugs

### Bug report template

```markdown
## Description
Clear description of the bug.

## Steps to reproduce
1. ...
2. ...
3. ...

## Expected behavior
What should happen.

## Actual behavior
What actually happens.

## Environment
- Windows version:
- .NET version:
- ElBruno.CopilotCLIMonitor version:

## Logs
Relevant logs from `%AppData%\ElBruno\CopilotCLIMonitor\logs\`

## Screenshots
If applicable, add screenshots.
```

## Requesting features

### Feature request template

```markdown
## Description
Clear description of the requested feature.

## Use case
Why this feature is needed.

## Proposed solution
Your idea for implementation.

## Alternatives considered
Other approaches you've thought of.

## Additional context
Any other relevant information.
```

## Community

- **Discussions** – Ask questions and share ideas
- **Issues** – Report bugs and request features
- **Slack/Discord** – Community chat (if applicable)

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

Feel free to open a GitHub discussion or issue!

---

Thank you for contributing to ElBruno.CopilotCLIMonitor! 🙏
