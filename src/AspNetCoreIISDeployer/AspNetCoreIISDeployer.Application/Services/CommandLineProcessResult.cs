using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Services
{
    public class CommandLineProcessResult
    {
        public static readonly CommandLineProcessResult Empty = new CommandLineProcessResult(0, new List<ConsoleOutput>());

        public CommandLineProcessResult(int exitCode, ConsoleOutput output)
            : this(exitCode, new List<ConsoleOutput> { output })
        {
        }

        public CommandLineProcessResult(int exitCode, IReadOnlyList<ConsoleOutput> output)
        {
            ExitCode = exitCode;
            Output = output ?? new List<ConsoleOutput>();
        }

        public int ExitCode { get; }

        public IReadOnlyList<ConsoleOutput> Output { get; }
    }
}