using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCoreIISDeployer.Application.Services
{
    public abstract class CommandLineToolServiceBase
    {
        protected CommandLineToolServiceBase()
        {
        }

        protected CommandLineProcessResult ExecuteCommandLineApplication(string executablePath, string arguments)
        {
            var processStartInfo = new ProcessStartInfo(executablePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(processStartInfo);

            var outputLines = new List<string>();
            var errorLines = new List<string>();
            var canReadMore = true;
            while (canReadMore)
            {
                var canReadFromStdOut = process.StandardOutput.EndOfStream == false;
                var canReadFromStdErr = process.StandardError.EndOfStream == false;

                if (canReadFromStdOut)
                {
                    outputLines.Add(process.StandardOutput.ReadLine());
                }

                if (canReadFromStdErr)
                {
                    errorLines.Add(process.StandardError.ReadLine());
                }

                canReadMore = canReadFromStdOut || canReadFromStdErr;
            }

            process.WaitForExit();

            return new CommandLineProcessResult(process.ExitCode, outputLines, errorLines);
        }
    }
}