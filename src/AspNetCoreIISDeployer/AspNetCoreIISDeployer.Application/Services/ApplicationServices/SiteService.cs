using System;
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

        public SiteService(IGitService gitService, IDotNetPublishService publishService, ISiteManagementService siteManagementService)
        {
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));
        }

        public Task PublishAppToSiteAsync(AppModel appModel)
        {
            return Task.Run(async () =>
            {
                publishService.Publish(appModel.ProjectPath, appModel.BuildConfiguration, appModel.PublishPath, appModel.Environment);

                await WriteGitInfoFilesAsync(appModel.ProjectPath, appModel.PublishPath);
            });
        }

        public Task StartSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
                siteManagementService.Start(siteName);
            });
        }

        public Task RestartSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
                siteManagementService.Stop(siteName);
                siteManagementService.Start(siteName);
            });
        }

        public Task StopSiteAsync(string siteName)
        {
            return Task.Run(() =>
            {
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