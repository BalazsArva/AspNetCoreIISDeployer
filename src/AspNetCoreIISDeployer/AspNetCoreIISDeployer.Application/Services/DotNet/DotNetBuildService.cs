using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetBuildService : DotNetServiceBase
    {
        public DotNetBuildService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public DotNetCommandResult Build(string projectPath, string configuration, string outputDirectory)
        {
            var arguments = $"build -c {configuration} -o \"{outputDirectory}\" \"{projectPath}\"";

            var commandResult = ExecuteDotNetCommand(arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new DotNetCliException($"Failed to execute the '{arguments}' .NET CLI command.", commandResult.ErrorLines);
            }

            return commandResult;
        }
    }
}