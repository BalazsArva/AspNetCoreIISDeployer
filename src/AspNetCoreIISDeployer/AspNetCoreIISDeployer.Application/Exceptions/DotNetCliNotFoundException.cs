using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class DotNetCliNotFoundException : Exception
    {
        public DotNetCliNotFoundException(string attemptedPath) : base($"The .NET CLI could not be found. Location searched: '{attemptedPath}'.")
        {
        }
    }
}