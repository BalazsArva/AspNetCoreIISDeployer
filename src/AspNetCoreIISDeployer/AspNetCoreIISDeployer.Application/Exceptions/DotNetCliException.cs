using System;
using System.Collections.Generic;
using AspNetCoreIISDeployer.Application.Services;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class DotNetCliException : Exception
    {
        public DotNetCliException(string message, IReadOnlyList<ConsoleOutput> errorOutput) : base(message)
        {
            ErrorOutput = errorOutput ?? throw new ArgumentNullException(nameof(errorOutput));
        }

        public IReadOnlyList<ConsoleOutput> ErrorOutput { get; }
    }
}