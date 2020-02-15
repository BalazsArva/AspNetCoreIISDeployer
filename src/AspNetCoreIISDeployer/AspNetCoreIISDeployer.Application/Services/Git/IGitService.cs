namespace AspNetCoreIISDeployer.Application.Services.Git
{
    public interface IGitService
    {
        string GetCurrentBranch(string repositoryPath);

        string GetUpstreamOfCurrentBranch(string repositoryPath);

        string GetCurrentCommitHash(string repositoryPath);

        string GetCurrentUpstreamCommitHash(string repositoryPath);

        bool IsGitRepository(string path);

        CommandLineProcessResult Fetch(string repositoryPath, bool all, bool prune);
    }
}