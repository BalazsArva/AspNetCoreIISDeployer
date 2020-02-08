using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class AppCmdNotFoundException : Exception
    {
        public AppCmdNotFoundException(string attemptedPath) : base($"The appcmd.exe tool could not be found. Location searched: '{attemptedPath}'.")
        {
        }
    }
}