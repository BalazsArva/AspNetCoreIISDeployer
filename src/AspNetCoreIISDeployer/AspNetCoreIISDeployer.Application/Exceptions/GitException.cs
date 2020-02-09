using System;
using System.Collections.Generic;
using AspNetCoreIISDeployer.Application.Services;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class GitException : Exception
    {
        public GitException(string message) : this(message, new List<ConsoleOutput>())
        {
        }

        public GitException(string message, IReadOnlyList<ConsoleOutput> output) : base(message)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public IReadOnlyList<ConsoleOutput> Output { get; }
    }
}