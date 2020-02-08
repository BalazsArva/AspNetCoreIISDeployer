using System;
using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class DotNetCliException : Exception
    {
        public DotNetCliException(string message, IReadOnlyList<string> errorOutput) : base(message)
        {
            ErrorOutput = errorOutput ?? throw new ArgumentNullException(nameof(errorOutput));
        }

        public IReadOnlyList<string> ErrorOutput { get; }
    }
}