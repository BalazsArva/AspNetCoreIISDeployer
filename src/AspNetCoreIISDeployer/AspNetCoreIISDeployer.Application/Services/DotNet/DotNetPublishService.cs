using AspNetCoreIISDeployer.Application.Configuration;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetPublishService : DotNetServiceBase
    {
        public DotNetPublishService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public CommandLineProcessResult Publish(string projectPath, string configuration, string outputDirectory, string environment = null)
        {
            var environmentArg = string.IsNullOrWhiteSpace(environment)
                ? string.Empty
                : $"/p:EnvironmentName={environment}";

            var arguments = $"publish -c {configuration} -o \"{outputDirectory}\" {environmentArg} \"{projectPath}\"";

            return ExecuteDotNetCommand(arguments);
        }
    }
}