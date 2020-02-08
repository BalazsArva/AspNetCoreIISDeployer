using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetDevCertSetupService : DotNetServiceBase
    {
        private const string Arguments = "dev-certs https --trust";

        public DotNetDevCertSetupService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public DotNetCommandResult SetupDotNetDevCert()
        {
            var commandResult = ExecuteDotNetCommand(Arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new DotNetCliException($"Failed to execute the '{Arguments}' .NET CLI command.", commandResult.ErrorLines);
            }

            return commandResult;
        }
    }
}