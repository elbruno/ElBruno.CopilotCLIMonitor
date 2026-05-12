using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Core.Interfaces;
using ElBruno.CopilotCLIMonitor.Core.Models;

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

        var command = new Command("notify", "Send a notification event to the monitor")
        {
            eventOption,
            messageOption,
            repoOption,
            branchOption
        };

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var eventType = parseResult.GetValue(eventOption)!;
            var message = parseResult.GetValue(messageOption)!;
            var repo = parseResult.GetValue(repoOption);
            var branch = parseResult.GetValue(branchOption);

            var monitorEvent = MonitorEvent.Parse(eventType, message, repo, branch);
            await notifier.NotifyAsync(monitorEvent, cancellationToken);
        });

        return command;
    }
}
