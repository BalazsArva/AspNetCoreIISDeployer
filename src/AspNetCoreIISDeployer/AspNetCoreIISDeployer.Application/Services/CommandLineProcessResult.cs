using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Services
{
    public class CommandLineProcessResult
    {
        public CommandLineProcessResult(int exitCode, IReadOnlyList<ConsoleOutput> output)
        {
            ExitCode = exitCode;
            Output = output ?? new List<ConsoleOutput>();
        }

        public int ExitCode { get; }

        public IReadOnlyList<ConsoleOutput> Output { get; }
    }
}