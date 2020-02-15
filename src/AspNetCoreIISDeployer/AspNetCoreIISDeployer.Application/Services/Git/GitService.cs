﻿using System;
using System.Linq;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.Git
{
    public class GitService : CommandLineToolServiceBase, IGitService
    {
        private readonly GitConfiguration configuration;

        public GitService(GitConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public bool IsGitRepository(string path)
        {
            // TODO: Verify that this returns with 0 exit code when the target is not a git repository.
            var result = ExecuteCommandLineApplication(configuration.GitPath, "status", path);

            if (result.Output.Any(x => x.Text.StartsWith("fatal: not a git repository", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }

        public string GetCurrentCommitHash(string repositoryPath)
        {
            var result = ExecuteCommandLineApplication(configuration.GitPath, "rev-parse HEAD", repositoryPath);

            if (result.Output.Count == 1 && IsGitLongHashLike(result.Output[0].Text))
            {
                return result.Output[0].Text;
            }

            throw new GitException($"Could not retrieve commit hash from the specified repository at '{repositoryPath}'.", result.Output);
        }

        public string GetCurrentCommitHashOfHeadsRemote(string repositoryPath)
        {
            // TODO: Maybe consider detached head as well
            var remoteBranchesOutput = ExecuteCommandLineApplication(configuration.GitPath, "branch -r", repositoryPath);

            var remoteOfHeadOutputLine = remoteBranchesOutput
                .Output
                .Where(x => !x.IsError && x.Text.Contains("/HEAD -> "))
                .SingleOrDefault();

            var remoteOfHead = remoteOfHeadOutputLine?.Text.Split("->").ElementAtOrDefault(1)?.Trim();

            if (!string.IsNullOrEmpty(remoteOfHead))
            {
                var result = ExecuteCommandLineApplication(configuration.GitPath, $"rev-parse {remoteOfHead}", repositoryPath);

                if (result.Output.Count == 1 && IsGitLongHashLike(result.Output[0].Text))
                {
                    return result.Output[0].Text;
                }

                throw new GitException($"Could not retrieve the commit hash of the head's remote from the specified repository at '{repositoryPath}'.", result.Output);
            }

            throw new GitException($"Could not retrieve the commit hash of the head's remote from the specified repository at '{repositoryPath}'.", remoteBranchesOutput.Output);
        }

        public string GetCurrentBranch(string repositoryPath)
        {
            var result = ExecuteCommandLineApplication(configuration.GitPath, "branch --show-current", repositoryPath);

            if (result.Output.Count == 1 && !result.Output[0].IsError)
            {
                return result.Output[0].Text;
            }

            throw new GitException($"Could not retrieve current branch from the specified repository at '{repositoryPath}'.", result.Output);
        }

        public CommandLineProcessResult Fetch(string repositoryPath, bool all, bool prune)
        {
            var allArgument = all ? "--all" : string.Empty;
            var pruneArgument = prune ? "--prune" : string.Empty;

            var result = ExecuteCommandLineApplication(configuration.GitPath, $"fetch {allArgument} {pruneArgument}", repositoryPath);

            return result;
        }

        private static bool IsGitLongHashLike(string text)
        {
            return text.Length == 40 && text.All(ch =>
                ('0' <= ch && ch <= '9') ||
                ('a' <= ch && ch <= 'f') ||
                ('A' <= ch && ch <= 'F'));
        }
    }
}