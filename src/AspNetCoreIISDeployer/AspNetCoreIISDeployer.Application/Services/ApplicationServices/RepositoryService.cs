﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Exceptions;
using AspNetCoreIISDeployer.Application.Services.Git;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class RepositoryService : IRepositoryService
    {
        private const string NoUpstreamBranchMessage = "(Current branch has no upstream)";
        private const string GitFolderName = ".git";

        private readonly IGitService gitService;

        private readonly object repoUpdatedCallbacksLock = new object();
        private readonly Dictionary<string, List<Func<Task>>> repoUpdatedCallbacks = new Dictionary<string, List<Func<Task>>>();

        public RepositoryService(IGitService gitService)
        {
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        }

        public void SubscribeToRepositoryUpdated(string repositoryPath, Func<Task> callback)
        {
            if (repositoryPath is null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            lock (repoUpdatedCallbacksLock)
            {
                if (!repoUpdatedCallbacks.ContainsKey(repositoryPath))
                {
                    repoUpdatedCallbacks.Add(repositoryPath, new List<Func<Task>>());
                }

                repoUpdatedCallbacks[repositoryPath].Add(callback);
            }
        }

        public Task<GitRepositoryInfo> GetRepositoryInfoAsync(string repositoryPath)
        {
            return Task.Run(() =>
            {
                if (!gitService.IsGitRepository(repositoryPath))
                {
                    return GitRepositoryInfo.Empty;
                }

                var branch = gitService.GetCurrentBranch(repositoryPath);
                var commit = gitService.GetCurrentCommitHash(repositoryPath);
                var commitOnRemote = gitService.GetCurrentUpstreamCommitHash(repositoryPath);

                return new GitRepositoryInfo(branch, commit, commitOnRemote ?? NoUpstreamBranchMessage);
            });
        }

        public Task FetchAsync(string repositoryPath, bool all, bool prune)
        {
            return Task.Run(async () =>
            {
                gitService.Fetch(repositoryPath, all, prune);

                // TODO: Only fire a notification if there is actually some change.
                await NotifyRepoUpdatedSubscribers(repositoryPath);
            });
        }

        public string FindRepositoryRoot(string repositoryPath)
        {
            if (repositoryPath is null)
            {
                throw new ArgumentNullException(nameof(repositoryPath));
            }

            // repositoryPath may point to anywhere within the repo, including subfolders and files. Find the .git folder there.
            var segments = repositoryPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > 0 && segments[segments.Length - 1] == GitFolderName)
            {
                if (Directory.Exists(repositoryPath))
                {
                    return repositoryPath;
                }
            }

            for (var i = segments.Length; i > 0; --i)
            {
                var path = string.Join(Path.DirectorySeparatorChar, segments, 0, i);
                var assumedGitPath = string.Join(Path.DirectorySeparatorChar, path, GitFolderName);

                if (Directory.Exists(path) && Directory.Exists(assumedGitPath))
                {
                    return path;
                }
            }

            // TODO: Throw a better exception
            throw new GitException($"The specified path '{repositoryPath}' is not in a git repository.");
        }

        private async Task NotifyRepoUpdatedSubscribers(string siteName)
        {
            List<Func<Task>> callbacks;
            lock (repoUpdatedCallbacksLock)
            {
                if (!repoUpdatedCallbacks.ContainsKey(siteName))
                {
                    return;
                }

                callbacks = repoUpdatedCallbacks[siteName];
            }

            foreach (var callback in repoUpdatedCallbacks[siteName])
            {
                await callback();
            }
        }
    }
}