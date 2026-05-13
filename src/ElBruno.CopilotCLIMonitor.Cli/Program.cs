using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Cli;
using ElBruno.CopilotCLIMonitor.Commands;
using ElBruno.CopilotCLIMonitor.Core.Services;
using Microsoft.Extensions.Logging;

var detector = new RepositoryDetector();
var installer = new HookInstaller();
using var loggerFactory = LoggingFactoryBuilder.Create();
var notifier = new HttpEventNotifier(logger: loggerFactory.CreateLogger<HttpEventNotifier>());

var root = new RootCommand("copilotclimon — GitHub Copilot CLI task monitor");

root.Add(NotifyCommand.Build(notifier));
root.Add(InitCommand.Build(detector, installer));
root.Add(UpgradeCommand.Build(detector, installer));
root.Add(DoctorCommand.Build(detector));
root.Add(UpdateCommand.Build());
root.Add(DiagnosticCommand.Build());

return await root.Parse(args).InvokeAsync();
