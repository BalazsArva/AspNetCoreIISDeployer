﻿using System;
using System.Collections.Generic;
using AspNetCoreIISDeployer.Application.Services;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class AppCmdException : Exception
    {
        public AppCmdException(string message, int exitCode, IReadOnlyList<ConsoleOutput> errorOutput) : base(message)
        {
            ExitCode = exitCode;
            ErrorOutput = errorOutput ?? throw new ArgumentNullException(nameof(errorOutput));
        }

        public int ExitCode { get; }

        public IReadOnlyList<ConsoleOutput> ErrorOutput { get; }
    }
}