using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class NetShNotFoundException : Exception
    {
        public NetShNotFoundException(string attemptedPath) : base($"The netsh.exe tool could not be found. Location searched: '{attemptedPath}'.")
        {
        }
    }
}