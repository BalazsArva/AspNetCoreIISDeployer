using System;
using System.Threading.Tasks;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public interface IRepositoryService
    {
        void RegisterPeriodicFetchForRepository(string repositoryPath);

        void UnregisterPeriodicFetchForRepository(string repositoryPath);

        void SubscribeToRepositoryUpdated(string repositoryPath, Func<Task> callback);

        string FindRepositoryRoot(string repositoryPath);

        Task<GitRepositoryInfo> GetRepositoryInfoAsync(string repositoryPath);

        Task FetchAsync(string repositoryPath, bool all, bool prune);
    }
}