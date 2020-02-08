using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetPublishService : DotNetServiceBase
    {
        public DotNetPublishService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public DotNetCommandResult Publish(string projectPath, string configuration, string outputDirectory, string environment = null)
        {
            var environmentArg = string.IsNullOrWhiteSpace(environment)
                ? string.Empty
                : $"/p:EnvironmentName={environment}";

            var arguments = $"publish -c {configuration} -o \"{outputDirectory}\" {environmentArg} \"{projectPath}\"";

            var commandResult = ExecuteDotNetCommand(arguments);
            if (commandResult.ExitCode != 0)
            {
                throw new DotNetCliException($"Failed to execute the '{arguments}' .NET CLI command.", commandResult.ErrorLines);
            }

            return commandResult;
        }
    }
}