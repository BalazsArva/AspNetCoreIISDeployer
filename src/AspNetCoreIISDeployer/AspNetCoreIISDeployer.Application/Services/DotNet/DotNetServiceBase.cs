using System;
using System.IO;
using System.Linq;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public abstract class DotNetServiceBase : CommandLineToolServiceBase
    {
        public DotNetConfiguration DotNetConfiguration { get; }

        protected DotNetServiceBase(DotNetConfiguration dotNetConfiguration)
        {
            DotNetConfiguration = dotNetConfiguration ?? throw new ArgumentNullException(nameof(dotNetConfiguration));
        }

        protected CommandLineProcessResult ExecuteDotNetCommand(string arguments)
        {
            EnsureDotNetCliPresent();

            var commandResult = ExecuteCommandLineApplication(DotNetConfiguration.DotNetCliPath, arguments);

            if (commandResult.ExitCode != 0)
            {
                throw new DotNetCliException($"Failed to execute the '{arguments}' .NET CLI command.", commandResult.Output);
            }

            return commandResult;
        }

        protected virtual void EnsureDotNetCliPresent()
        {
            var expectedPath = DotNetConfiguration.DotNetCliPath;

            if (!File.Exists(expectedPath))
            {
                throw new DotNetCliNotFoundException(expectedPath);
            }
        }

        protected virtual void EnsureSdkVersionSupported(DotNetVersion version)
        {
            // TODO: Refactor to use the generic command executor method. Not sure if this method will even be needed, maybe can check the output for version support errors as needed.
            EnsureDotNetCliPresent();

            var dotNetListSdksCommandResult = ExecuteCommandLineApplication(DotNetConfiguration.DotNetCliPath, "--list-sdks");

            if (dotNetListSdksCommandResult.Output.Any(line => !line.IsError && version.IsCompatible(line.Text)))
            {
                return;
            }

            throw new DotNetSdkMissingException($"Could not find a .NET SDK that is compatible with the specified version '{version}'.");
        }
    }
}