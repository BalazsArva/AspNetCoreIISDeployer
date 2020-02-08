using System;

namespace AspNetCoreIISDeployer.Application.Exceptions
{
    public class SiteManagementException : Exception
    {
        public SiteManagementException(string message) : base(message)
        {
        }
    }
}