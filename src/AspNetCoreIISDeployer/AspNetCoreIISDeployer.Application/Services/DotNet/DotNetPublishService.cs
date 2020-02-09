using System;
using System.IO;
using System.Linq;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.Git;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetPublishService : DotNetServiceBase, IDotNetPublishService
    {
        private readonly IGitService gitService;

        public DotNetPublishService(DotNetConfiguration dotNetConfiguration, IGitService gitService) : base(dotNetConfiguration)
        {
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        }

        public CommandLineProcessResult Publish(string projectPath, string configuration, string outputDirectory, string environment = null)
        {
            var environmentArg = string.IsNullOrWhiteSpace(environment)
                ? string.Empty
                : $"/p:EnvironmentName={environment}";

            var arguments = $"publish -c {configuration} -o \"{outputDirectory}\" {environmentArg} \"{projectPath}\"";

            var result = ExecuteDotNetCommand(arguments);

            // This might not be the repo root but that's fine
            var repositoryPath = Path.GetDirectoryName(projectPath);

            WriteGitInfoFiles(repositoryPath, outputDirectory);

            return result;
        }

        public GitPublishInfo GetGitPublishInfo(string publishPath)
        {
            var filePath = Path.Combine(publishPath, "git.publishinfo");
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                var branch = lines.FirstOrDefault(x => x.StartsWith("branch="))?.Substring("branch=".Length);
                var commit = lines.FirstOrDefault(x => x.StartsWith("commit="))?.Substring("commit=".Length);

                return new GitPublishInfo(branch, commit);
            }

            return GitPublishInfo.Empty;
        }

        private void WriteGitInfoFiles(string repositoryPath, string publishPath)
        {
            if (!gitService.IsGitRepository(repositoryPath))
            {
                return;
            }

            var branch = gitService.GetCurrentBranch(repositoryPath);
            var commit = gitService.GetCurrentCommitHash(repositoryPath);

            using (var gitInfoFile = File.CreateText(Path.Combine(publishPath, "git.publishinfo")))
            {
                gitInfoFile.WriteLine($"branch={branch}");
                gitInfoFile.WriteLine($"commit={commit}");
            }
        }
    }
}