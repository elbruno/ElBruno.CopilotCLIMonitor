using ElBruno.CopilotCLIMonitor.Core.Interfaces;

namespace ElBruno.CopilotCLIMonitor.Tests.Fakes;

public sealed class FakeRepositoryDetector : IRepositoryDetector
{
    public string? RootToReturn { get; set; }
    public string? NameToReturn { get; set; }
    public string? BranchToReturn { get; set; }

    public string? DetectRepositoryRoot(string workingDirectory) => RootToReturn;
    public string? GetRepositoryName(string workingDirectory) => NameToReturn;
    public string? GetCurrentBranch(string workingDirectory) => BranchToReturn;
}
