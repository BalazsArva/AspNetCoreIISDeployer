using System;
using System.IO;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public abstract class IISManagementServiceBase : CommandLineToolServiceBase
    {
        protected IISManagementServiceBase(IISMangementConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IISMangementConfiguration Configuration { get; }

        protected CommandLineProcessResult ExecuteAppCmdCommand(string arguments)
        {
            EnsureAppCmdPresent();

            var commandResult = ExecuteCommandLineApplication(Configuration.AppCmdPath, arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new AppCmdException($"Failed to execute the '{arguments}' AppCmd command.", commandResult.Output);
            }

            return commandResult;
        }

        protected CommandLineProcessResult ExecuteNetShCommand(string arguments)
        {
            EnsureNetShPresent();

            var commandResult = ExecuteCommandLineApplication(Configuration.NetShPath, arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new AppCmdException($"Failed to execute the '{arguments}' netsh command.", commandResult.Output);
            }

            return commandResult;
        }

        protected virtual void EnsureAppCmdPresent()
        {
            var expectedPath = Configuration.AppCmdPath;

            if (!File.Exists(expectedPath))
            {
                throw new AppCmdNotFoundException(expectedPath);
            }
        }

        protected virtual void EnsureNetShPresent()
        {
            var expectedPath = Configuration.NetShPath;

            if (!File.Exists(expectedPath))
            {
                throw new NetShNotFoundException(expectedPath);
            }
        }
    }
}