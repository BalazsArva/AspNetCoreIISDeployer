using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class InvalidPortMappingException : Exception
    {
        public InvalidPortMappingException(string message) : base(message)
        {
        }
    }
}