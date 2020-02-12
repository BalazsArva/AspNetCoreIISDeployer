using System;
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

            return result;
        }
    }
}