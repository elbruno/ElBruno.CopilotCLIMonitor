using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class NotifyRequestValidatorTests
{
    [Fact]
    public void TryValidate_WithOriginRepository_AcceptsValidRequest()
    {
        var request = new NotifyRequest(
            "task-completed",
            "done",
            Repository: "repo-a",
            Branch: "main",
            OriginRepository: "https://github.com/elbruno/repo-a.git");

        var isValid = NotifyRequestValidator.TryValidate(request, out var error);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void TryValidate_WithControlCharsInOriginRepository_ReturnsFalse()
    {
        var request = new NotifyRequest(
            "task-completed",
            "done",
            OriginRepository: "https://github.com/elbruno/repo-a.git\nbad");

        var isValid = NotifyRequestValidator.TryValidate(request, out var error);

        Assert.False(isValid);
        Assert.Equal("Invalid origin repository value.", error);
    }
}
