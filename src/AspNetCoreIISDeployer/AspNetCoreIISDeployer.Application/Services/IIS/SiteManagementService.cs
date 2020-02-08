using System.Collections.Generic;
using System.IO;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public class SiteManagementService : IISManagementServiceBase
    {
        public SiteManagementService(IISMangementConfiguration configuration) : base(configuration)
        {
        }

        public CommandLineProcessResult Create(string appPoolName, string siteName, Port httpPort, string publishPath)
        {
            return Create(appPoolName, siteName, httpPort, Port.None, null, publishPath);
        }

        public CommandLineProcessResult Create(string appPoolName, string siteName, Port httpsPort, string certificateThumbprint, string publishPath)
        {
            return Create(appPoolName, siteName, Port.None, httpsPort, certificateThumbprint, publishPath);
        }

        public CommandLineProcessResult Create(string appPoolName, string siteName, Port httpPort, Port httpsPort, string certificateThumbprint, string publishPath)
        {
            if (httpPort == Port.None && httpsPort == Port.None)
            {
                throw new InvalidPortMappingException($"At least one of the parameters '{nameof(httpPort)}' and '{nameof(httpsPort)}' must be set.");
            }

            if (!Directory.Exists(publishPath))
            {
                throw new SiteManagementException("The specified publish path does not exist.");
            }

            if (httpPort != Port.None)
            {
                if (string.IsNullOrWhiteSpace(certificateThumbprint))
                {
                    throw new SiteManagementException("A certificate thumbprint must be provided if the site uses HTTPS binding.");
                }

                // TODO: Validate existence of certificate based on thumbprint
            }

            ValidateSiteName(siteName);

            int id = httpsPort != Port.None ? httpsPort : httpPort;

            var httpBinding = httpPort != Port.None ? $"http/*:{httpPort}:" : string.Empty;
            var httpsBinding = httpsPort != Port.None ? $"https/*:{httpPort}:" : string.Empty;
            var bindings = string.Join(",", httpBinding, httpsBinding);

            var addSiteCommandArguments = $"add site /name:{siteName} /id:{id} /physicalPath:\"{publishPath}\" /bindings:{bindings}";
            var assignSiteToAppPoolCommandArguments = $"set app \"{siteName}/\" /applicationPool:{appPoolName}";

            var addSiteCommandResult = ExecuteAppCmdCommand(addSiteCommandArguments);
            var assignSiteToAppPoolCommandResult = ExecuteAppCmdCommand(assignSiteToAppPoolCommandArguments);

            var result = new List<ConsoleOutput>();

            result.AddRange(addSiteCommandResult.Output);
            result.AddRange(assignSiteToAppPoolCommandResult.Output);

            if (httpsPort != Port.None)
            {
                var bindCertToSiteCommandArguments = $"http add sslcert ipport=0.0.0.0:{httpsPort} certhash={certificateThumbprint} appid='{{{httpsPort}}}'";

                var bindCertToSiteCommandResult = ExecuteNetShCommand(bindCertToSiteCommandArguments);

                result.AddRange(bindCertToSiteCommandResult.Output);
            }

            return new CommandLineProcessResult(0, result);
        }

        public CommandLineProcessResult Delete(string siteName)
        {
            ValidateSiteName(siteName);

            var arguments = $"delete site \"{siteName}\"";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult Start(string siteName)
        {
            ValidateSiteName(siteName);

            var arguments = $"start site \"{siteName}\"";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult Stop(string siteName)
        {
            ValidateSiteName(siteName);

            var arguments = $"stop site \"{siteName}\"";

            return ExecuteAppCmdCommand(arguments);
        }

        private void ValidateSiteName(string siteName)
        {
            if (!IsValidSiteName(siteName))
            {
                throw new SiteManagementException($"The site name '{siteName}' is invalid.");
            }
        }

        private bool IsValidSiteName(string siteName)
        {
            if (string.IsNullOrEmpty(siteName))
            {
                return false;
            }

            var minLower = (int)'a';
            var maxLower = (int)'z';

            var minUpper = (int)'a';
            var maxUpper = (int)'z';

            var minNum = (int)'0';
            var maxNum = (int)'9';

            for (var i = 0; i < siteName.Length; ++i)
            {
                bool isValid;

                var ch = siteName[i];

                if (i == 0)
                {
                    isValid = (minLower <= ch && ch <= maxLower) || (minUpper <= ch && ch <= maxUpper);
                }
                else
                {
                    isValid =
                       ch == '-' ||
                       (minLower <= ch && ch <= maxLower) ||
                       (minUpper <= ch && ch <= maxUpper) ||
                       (minNum <= ch && ch <= maxNum);
                }

                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }
    }
}