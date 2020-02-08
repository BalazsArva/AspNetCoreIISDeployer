using System;
using System.IO;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.AppCmd
{
    public abstract class AppCmdServiceBase : CommandLineToolServiceBase
    {
        protected AppCmdServiceBase(AppCmdConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public AppCmdConfiguration Configuration { get; }

        protected CommandLineProcessResult ExecuteAppCmdCommand(string arguments)
        {
            EnsureAppCmdPresent();

            var commandResult = ExecuteCommandLineApplication(Configuration.AppCmdPath, arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new AppCmdException($"Failed to execute the '{arguments}' AppCmd command.", commandResult.ErrorLines);
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
    }
}