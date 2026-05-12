using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor.Tests.Services;

public class RepositoryDetectorTests
{
    private readonly RepositoryDetector _sut = new();

    [Fact]
    public void DetectRepositoryRoot_WhenInsideGitRepo_ReturnsNonNull()
    {
        // The test itself runs inside the repo so this should always find a root
        var root = _sut.DetectRepositoryRoot(Directory.GetCurrentDirectory());
        Assert.NotNull(root);
        Assert.True(Directory.Exists(root));
    }

    [Fact]
    public void DetectRepositoryRoot_WhenDirectoryDoesNotExist_ReturnsNull()
    {
        var result = _sut.DetectRepositoryRoot(@"C:\this-path-does-not-exist-8f3a2b9c");
        Assert.Null(result);
    }

    [Fact]
    public void DetectRepositoryRoot_WhenInsideGitRepo_PathContainsGitFolder()
    {
        var root = _sut.DetectRepositoryRoot(Directory.GetCurrentDirectory());
        Assert.NotNull(root);
        var gitDir = Path.Combine(root, ".git");
        Assert.True(Directory.Exists(gitDir), $".git directory expected at: {gitDir}");
    }

    [Fact]
    public void GetRepositoryName_WhenInsideGitRepo_ReturnsNonEmptyString()
    {
        var name = _sut.GetRepositoryName(Directory.GetCurrentDirectory());
        Assert.NotNull(name);
        Assert.NotEmpty(name);
    }

    [Fact]
    public void GetRepositoryName_WhenDirectoryDoesNotExist_ReturnsNull()
    {
        var result = _sut.GetRepositoryName(@"C:\this-path-does-not-exist-8f3a2b9c");
        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentBranch_WhenInsideGitRepo_ReturnsNonEmptyString()
    {
        var branch = _sut.GetCurrentBranch(Directory.GetCurrentDirectory());
        // May return null in detached HEAD state (CI environments).
        // If non-null, it must not be empty.
        if (branch is not null)
            Assert.NotEmpty(branch);
    }

    [Fact]
    public void GetCurrentBranch_WhenDirectoryDoesNotExist_ReturnsNull()
    {
        var result = _sut.GetCurrentBranch(@"C:\this-path-does-not-exist-8f3a2b9c");
        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentBranch_WhenInsideTempDirOutsideGit_ReturnsNull()
    {
        // Create a temp dir that is NOT a git repo
        var tempDir = Path.Combine(Path.GetTempPath(), $"copilotclimon-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var result = _sut.GetCurrentBranch(tempDir);
            Assert.Null(result);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }
}
