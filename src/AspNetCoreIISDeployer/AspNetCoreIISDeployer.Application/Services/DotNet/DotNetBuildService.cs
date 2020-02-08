using AspNetCoreIISDeployer.Application.Configuration;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetBuildService : DotNetServiceBase
    {
        public DotNetBuildService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public CommandLineProcessResult Build(string projectPath, string configuration, string outputDirectory)
        {
            var arguments = $"build -c {configuration} -o \"{outputDirectory}\" \"{projectPath}\"";

            return ExecuteDotNetCommand(arguments);
        }
    }
}