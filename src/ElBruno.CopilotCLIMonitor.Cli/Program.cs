using System.CommandLine;
using ElBruno.CopilotCLIMonitor.Cli;
using ElBruno.CopilotCLIMonitor.Commands;
using ElBruno.CopilotCLIMonitor.Core.Services;

var detector = new RepositoryDetector();
var installer = new HookInstaller();
var notifier = new HttpEventNotifier();

var root = new RootCommand("copilotclimon — GitHub Copilot CLI task monitor");

root.Add(NotifyCommand.Build(notifier));
root.Add(InitCommand.Build(detector, installer));
root.Add(UpgradeCommand.Build(detector, installer));
root.Add(DoctorCommand.Build(detector));

return await root.Parse(args).InvokeAsync();
