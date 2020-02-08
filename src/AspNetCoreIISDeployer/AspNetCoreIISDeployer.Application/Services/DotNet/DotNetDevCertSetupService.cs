using AspNetCoreIISDeployer.Application.Configuration;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetDevCertSetupService : DotNetServiceBase
    {
        private const string Arguments = "dev-certs https --trust";

        public DotNetDevCertSetupService(DotNetConfiguration dotNetConfiguration) : base(dotNetConfiguration)
        {
        }

        public CommandLineProcessResult SetupDotNetDevCert()
        {
            return ExecuteDotNetCommand(Arguments);
        }
    }
}