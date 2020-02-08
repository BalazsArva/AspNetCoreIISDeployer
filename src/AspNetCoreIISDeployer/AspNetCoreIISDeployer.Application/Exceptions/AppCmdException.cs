using System;
using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class AppCmdException : Exception
    {
        public AppCmdException(string message, IReadOnlyList<string> errorOutput) : base(message)
        {
            ErrorOutput = errorOutput ?? throw new ArgumentNullException(nameof(errorOutput));
        }

        public IReadOnlyList<string> ErrorOutput { get; }
    }
}