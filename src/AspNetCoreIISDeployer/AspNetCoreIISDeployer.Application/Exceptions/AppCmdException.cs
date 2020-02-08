using System;
using System.Collections.Generic;
using AspNetCoreIISDeployer.Application.Services;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class AppCmdException : Exception
    {
        public AppCmdException(string message, IReadOnlyList<ConsoleOutput> errorOutput) : base(message)
        {
            ErrorOutput = errorOutput ?? throw new ArgumentNullException(nameof(errorOutput));
        }

        public IReadOnlyList<ConsoleOutput> ErrorOutput { get; }
    }
}