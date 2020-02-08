using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class DotNetSdkMissingException : Exception
    {
        public DotNetSdkMissingException(string message) : base(message)
        {
        }
    }
}