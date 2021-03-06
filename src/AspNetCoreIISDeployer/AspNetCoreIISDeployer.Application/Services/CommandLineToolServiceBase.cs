﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AspNetCoreIISDeployer.Application.Services
{
    public abstract class CommandLineToolServiceBase
    {
        protected CommandLineToolServiceBase()
        {
        }

        protected CommandLineProcessResult ExecuteCommandLineApplication(string executablePath, string arguments)
        {
            var workingDirectory = Path.GetDirectoryName(executablePath);

            return ExecuteCommandLineApplication(executablePath, arguments, workingDirectory);
        }

        protected CommandLineProcessResult ExecuteCommandLineApplication(string executablePath, string arguments, string workingDirectory)
        {
            var processStartInfo = new ProcessStartInfo(executablePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = workingDirectory
            };

            var process = Process.Start(processStartInfo);

            var output = new List<ConsoleOutput>();
            var canReadMore = true;
            while (canReadMore)
            {
                var canReadFromStdOut = process.StandardOutput.EndOfStream == false;
                var canReadFromStdErr = process.StandardError.EndOfStream == false;

                if (canReadFromStdOut)
                {
                    output.Add(new ConsoleOutput(process.StandardOutput.ReadLine(), false));
                }

                if (canReadFromStdErr)
                {
                    output.Add(new ConsoleOutput(process.StandardError.ReadLine(), true));
                }

                canReadMore = canReadFromStdOut || canReadFromStdErr;
            }

            process.WaitForExit();

            return new CommandLineProcessResult(process.ExitCode, output);
        }
    }
}