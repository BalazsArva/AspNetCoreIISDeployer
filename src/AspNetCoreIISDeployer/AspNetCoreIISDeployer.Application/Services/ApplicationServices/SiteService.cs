using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class SiteService : ISiteService
    {
        private const string BranchEntryPrefix = "branch=";
        private const string CommitEntryPrefix = "commit=";
        private const string GitPublishInfoFileName = "git.publishinfo";

        private readonly IGitService gitService;
        private readonly IDotNetPublishService publishService;
        private readonly ISiteManagementService siteManagementService;

        private readonly object siteUpdatedCallbacksLock = new object();
        private readonly Dictionary<string, List<Func<Task>>> siteUpdatedCallbacks = new Dictionary<string, List<Func<Task>>>();

        public SiteService(IGitService gitService, IDotNetPublishService publishService, ISiteManagementService siteManagementService)
        {
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));
        }

        public void SubscribeToSiteUpdated(string siteName, Func<Task> callback)
        {
            if (siteName is null)
            {
                throw new ArgumentNullException(nameof(siteName));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            lock (siteUpdatedCallbacksLock)
            {
                if (!siteUpdatedCallbacks.ContainsKey(siteName))
                {
                    siteUpdatedCallbacks.Add(siteName, new List<Func<Task>>());
                }

                siteUpdatedCallbacks[siteName].Add(callback);
            }
        }

        public Task PublishAppToSiteAsync(AppModel appModel)
        {
            return Task.Run(async () =>
            {
                var siteName = appModel.SiteName;
                var siteExists = siteManagementService.SiteExists(siteName);

                if (siteExists)
                {
                    siteManagementService.Stop(siteName);
                }

                publishService.Publish(appModel.ProjectPath, appModel.BuildConfiguration, appModel.PublishPath, appModel.Environment);

                await WriteGitInfoFilesAsync(appModel.ProjectPath, appModel.PublishPath);

                if (siteExists)
                {
                    siteManagementService.Start(siteName);
                }

                await NotifySiteUpdatedSubscribersAsync(siteName);
            });
        }

        public Task StartSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
                siteManagementService.SetServerAutoStart(siteName, true);
                siteManagementService.Start(siteName);
            });
        }

        public Task RestartSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
                siteManagementService.SetServerAutoStart(siteName, true);
                siteManagementService.Stop(siteName);
                siteManagementService.Start(siteName);
            });
        }

        public Task StopSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
                siteManagementService.SetServerAutoStart(siteName, false);
                siteManagementService.Stop(siteName);
            });
        }

        public Task CreateSiteAsync(AppModel appModel)
        {
            return Task.Run(async () =>
            {
                if (!Directory.Exists(appModel.PublishPath))
                {
                    publishService.Publish(appModel.ProjectPath, appModel.BuildConfiguration, appModel.PublishPath, appModel.Environment);

                    await WriteGitInfoFilesAsync(appModel.ProjectPath, appModel.PublishPath);
                }

                siteManagementService.Create(appModel.AppPoolName, appModel.SiteName, appModel.HttpPort, appModel.HttpsPort, appModel.CertificateThumbprint, appModel.PublishPath);
                siteManagementService.SetServerAutoStart(appModel.SiteName, true);

                await NotifySiteUpdatedSubscribersAsync(appModel.SiteName);
            });
        }

        public Task DeleteSiteAsync(AppModel appModel)
        {
            return Task.Run(async () =>
            {
                // TODO: Verify that there is actually a certificate bound to that port.
                siteManagementService.UnbindCertificateFromSite(appModel.HttpsPort);
                siteManagementService.Delete(appModel.SiteName);

                // TODO: Maybe should check whether there are any more applications in this pool.
                // TODO: Later it should also be validated that only 1 app belongs to an apppool as inprocess hosting only works like that.
                siteManagementService.DeleteAppPool(appModel.AppPoolName);

                Directory.Delete(appModel.PublishPath, true);

                await NotifySiteUpdatedSubscribersAsync(appModel.SiteName);
            });
        }

        public async Task<GitPublishInfo> GetGitPublishInfoAsync(string publishPath)
        {
            var filePath = Path.Combine(publishPath, GitPublishInfoFileName);

            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);

                var branch = lines.FirstOrDefault(x => x.StartsWith(BranchEntryPrefix))?.Substring(BranchEntryPrefix.Length);
                var commit = lines.FirstOrDefault(x => x.StartsWith(CommitEntryPrefix))?.Substring(CommitEntryPrefix.Length);

                return new GitPublishInfo(branch, commit);
            }

            return GitPublishInfo.Empty;
        }

        public Task<string> GetBoundCertificateHashAsync(AppModel appModel)
        {
            return Task.Run(() =>
            {
                try
                {
                    // TODO: Return early if there is no HTTPS port specified.
                    var certHash = siteManagementService.GetBoundCertificateHash(appModel.HttpsPort);

                    return certHash;
                }
                catch
                {
                    return null;
                }
            });
        }

        private async Task NotifySiteUpdatedSubscribersAsync(string siteName)
        {
            List<Func<Task>> callbacks;
            lock (siteUpdatedCallbacksLock)
            {
                if (!siteUpdatedCallbacks.ContainsKey(siteName))
                {
                    return;
                }

                callbacks = siteUpdatedCallbacks[siteName];
            }

            foreach (var callback in siteUpdatedCallbacks[siteName])
            {
                await callback();
            }
        }

        private async Task WriteGitInfoFilesAsync(string projectPath, string publishPath)
        {
            // This might not be the repo root but that's fine
            var repositoryPath = Path.GetDirectoryName(projectPath);

            if (!gitService.IsGitRepository(repositoryPath))
            {
                return;
            }

            var branch = gitService.GetCurrentBranch(repositoryPath);
            var commit = gitService.GetCurrentCommitHash(repositoryPath);

            using (var gitInfoFile = File.CreateText(Path.Combine(publishPath, GitPublishInfoFileName)))
            {
                await gitInfoFile.WriteLineAsync($"{BranchEntryPrefix}{branch}");
                await gitInfoFile.WriteLineAsync($"{CommitEntryPrefix}{commit}");

                await gitInfoFile.FlushAsync();
            }
        }
    }
}