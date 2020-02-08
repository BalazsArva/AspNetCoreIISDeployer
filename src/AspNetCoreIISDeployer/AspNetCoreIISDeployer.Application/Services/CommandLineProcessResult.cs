using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Services
{
    public class CommandLineProcessResult
    {
        public CommandLineProcessResult(int exitCode, IReadOnlyList<string> outputLines, IReadOnlyList<string> errorLines)
        {
            ExitCode = exitCode;
            OutputLines = outputLines ?? new List<string>();
            ErrorLines = errorLines ?? new List<string>();
        }

        public int ExitCode { get; }

        public IReadOnlyList<string> OutputLines { get; }

        public IReadOnlyList<string> ErrorLines { get; }
    }
}