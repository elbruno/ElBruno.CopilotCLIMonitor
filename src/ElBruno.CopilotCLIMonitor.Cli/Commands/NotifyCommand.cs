using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Commands;

public static class NotifyCommand
{
    public static Command Build(IEventNotifier notifier)
    {
        var eventOption = new Option<string>("--event")
        {
            Description = "Event type (e.g. task-completed, error, approval-required)",
            Required = true
        };
        var messageOption = new Option<string>("--message")
        {
            Description = "Notification message",
            Required = true
        };
        var repoOption = new Option<string?>("--repository") { Description = "Repository name override" };
        var branchOption = new Option<string?>("--branch") { Description = "Branch name override" };
        var sourceOption = new Option<string?>("--source") { Description = "Source tag override (for event origin tracing)" };
        var originRepositoryOption = new Option<string?>("--origin-repository") { Description = "Source origin repository URL override" };

        var command = new Command("notify", "Send a notification event to the monitor")
        {
            eventOption,
            messageOption,
            repoOption,
            branchOption,
            sourceOption,
            originRepositoryOption
        };

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var eventType = parseResult.GetValue(eventOption)!;
            var message = parseResult.GetValue(messageOption)!;
            var repo = parseResult.GetValue(repoOption);
            var branch = parseResult.GetValue(branchOption);
            var source = parseResult.GetValue(sourceOption);
            var originRepository = parseResult.GetValue(originRepositoryOption);
            var detector = new RepositoryDetector();
            var cwd = Directory.GetCurrentDirectory();

            repo ??= detector.GetRepositoryName(cwd);
            branch ??= detector.GetCurrentBranch(cwd);
            originRepository ??= detector.GetOriginRepository(cwd);

            var monitorEvent = MonitorEvent.Parse(eventType, message, repo, branch, source, originRepository);
            await notifier.NotifyAsync(monitorEvent, cancellationToken);
        });

        return command;
    }
}
