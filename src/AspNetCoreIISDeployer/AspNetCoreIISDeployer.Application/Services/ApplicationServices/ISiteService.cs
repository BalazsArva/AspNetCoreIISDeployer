using System;
using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Models;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public interface ISiteService
    {
        void SubscribeToSiteUpdated(string siteName, Func<Task> callback);

        Task PublishAppToSiteAsync(AppModel appModel);

        Task RestartSiteAsync(string siteName);

        Task StartSiteAsync(string siteName);

        Task StopSiteAsync(string siteName);

        Task CreateSiteAsync(AppModel appModel);

        Task DeleteSiteAsync(AppModel appModel);

        Task<GitPublishInfo> GetGitPublishInfoAsync(string publishPath);
    }
}