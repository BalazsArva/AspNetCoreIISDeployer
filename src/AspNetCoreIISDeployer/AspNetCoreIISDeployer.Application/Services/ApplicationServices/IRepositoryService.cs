using System;
using System.Threading.Tasks;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public interface IRepositoryService
    {
        string GetCurrentBranch(string repositoryPath);

        string GetCurrentCommitHash(string repositoryPath);

        string FindRepositoryRoot(string repositoryPath);

        Task FetchAsync(string repositoryPath, bool all, bool prune);

        void SubscribeToRepositoryUpdated(string repositoryPath, Func<Task> callback);
    }
}