using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Exceptions;

namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public class SiteManagementService : IISManagementServiceBase, ISiteManagementService
    {
        private const string AppCmdListAppPoolsCommandArgument = "list apppool";
        private const string AppCmdListSitesCommandArgument = "list site";

        private const string EmptyAppId = "00000000000000000000000000000000";

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

            if (httpsPort != Port.None)
            {
                if (string.IsNullOrWhiteSpace(certificateThumbprint))
                {
                    throw new SiteManagementException("A certificate thumbprint must be provided if the site uses HTTPS binding.");
                }

                // TODO: Validate existence of certificate based on thumbprint
            }

            ValidateSiteName(siteName);

            var createAppPoolCommandResult = CreateAppPool(appPoolName);
            var addSiteCommandResult = CommandLineProcessResult.Empty;

            if (!SiteExists(siteName))
            {
                int id = httpsPort != Port.None ? httpsPort : httpPort;

                var httpBinding = httpPort != Port.None ? $"http/*:{httpPort}:" : string.Empty;
                var httpsBinding = httpsPort != Port.None ? $"https/*:{httpsPort}:" : string.Empty;
                var bindings = string.Join(",", httpBinding, httpsBinding);

                var addSiteCommandArguments = $"add site /name:{siteName} /id:{id} /physicalPath:\"{publishPath}\" /bindings:{bindings}";

                addSiteCommandResult = ExecuteAppCmdCommand(addSiteCommandArguments);
            }

            var assignSiteToAppPoolCommandArguments = $"set app \"{siteName}/\" /applicationPool:{appPoolName}";
            var assignSiteToAppPoolCommandResult = ExecuteAppCmdCommand(assignSiteToAppPoolCommandArguments);

            // TODO: Should do more checks on the results. Sometimes 0 exitcode is returned even in case of an error and also sometimes errors are written to stdout instead of stderr.
            var result = new List<ConsoleOutput>();

            result.AddRange(createAppPoolCommandResult.Output);
            result.AddRange(addSiteCommandResult.Output);
            result.AddRange(assignSiteToAppPoolCommandResult.Output);

            if (httpsPort != Port.None)
            {
                var appId = GenerateAppId(httpsPort);

                // Note: when running this from PowerShell, appid must be surrounded by 's: appid='{value}'
                var bindCertToSiteCommandArguments = $"http add sslcert ipport=0.0.0.0:{httpsPort} certhash={certificateThumbprint} appid={{{appId}}}";

                var unbindCertFromSiteCommandResult = UnbindCertificateFromSite(httpsPort);
                var bindCertToSiteCommandResult = ExecuteNetShCommand(bindCertToSiteCommandArguments);

                result.AddRange(unbindCertFromSiteCommandResult.Output);
                result.AddRange(bindCertToSiteCommandResult.Output);
            }

            return new CommandLineProcessResult(0, result);
        }

        public CommandLineProcessResult UnbindCertificateFromSite(Port httpsPort)
        {
            try
            {
                return ExecuteNetShCommand($"http delete sslcert ipport=0.0.0.0:{httpsPort}");
            }
            catch (NetShException e) when (e.ExitCode == 1)
            {
                if (e.ErrorOutput.Any(x => string.Equals(x.Text, "The system cannot find the file specified.", StringComparison.OrdinalIgnoreCase)))
                {
                    return new CommandLineProcessResult(0, ConsoleOutput.FromSingleOutputLine($"No certificate is configured for port {httpsPort}, skipping removal."));
                }

                throw;
            }
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

        // TODO: Refactor apppool operations (either keep here or at ApplicationPoolManagementService)
        public CommandLineProcessResult StartAppPool(string appPoolName)
        {
            var arguments = $"start apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult StopAppPool(string appPoolName)
        {
            var arguments = $"stop apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult CreateAppPool(string appPoolName)
        {
            if (AppPoolExists(appPoolName))
            {
                return CommandLineProcessResult.Empty;
            }

            var arguments = $"add apppool /name:{appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult DeleteAppPool(string appPoolName)
        {
            var arguments = $"delete apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public IReadOnlyList<AppPoolDescriptor> ListAppPools()
        {
            const string appPoolPrefix = "APPPOOL ";

            var commandResult = ExecuteAppCmdCommand(AppCmdListAppPoolsCommandArgument);

            var result = new List<AppPoolDescriptor>();
            for (var i = 0; i < commandResult.Output.Count; ++i)
            {
                var outputLine = commandResult.Output[i];
                var text = outputLine.Text;

                if (outputLine.IsError || !text.StartsWith(appPoolPrefix))
                {
                    continue;
                }

                // appPoolPrefix.Length + 1: +1 because the name is contained between "s
                var appPoolName = new string(text.Skip(appPoolPrefix.Length + 1).TakeWhile(ch => ch != '"').ToArray());
                var state = AppPoolState.Unknown;

                if (text.Contains("state:Stopped"))
                {
                    state = AppPoolState.Stopped;
                }
                else if (text.Contains("state:Started"))
                {
                    state = AppPoolState.Started;
                }

                result.Add(new AppPoolDescriptor(appPoolName, state));
            }

            return result;
        }

        public IReadOnlyList<SiteDescriptor> ListSites()
        {
            const string sitePrefix = "SITE ";

            var commandResult = ExecuteAppCmdCommand(AppCmdListSitesCommandArgument);

            var result = new List<SiteDescriptor>();
            for (var i = 0; i < commandResult.Output.Count; ++i)
            {
                var outputLine = commandResult.Output[i];
                var text = outputLine.Text;

                if (outputLine.IsError || !text.StartsWith(sitePrefix))
                {
                    continue;
                }

                // sitePrefix.Length + 1: +1 because the name is contained between "s
                var siteName = new string(text.Skip(sitePrefix.Length + 1).TakeWhile(ch => ch != '"').ToArray());
                var state = SiteState.Unknown;

                if (text.Contains("state:Stopped"))
                {
                    state = SiteState.Stopped;
                }
                else if (text.Contains("state:Started"))
                {
                    state = SiteState.Started;
                }

                result.Add(new SiteDescriptor(siteName, state));
            }

            return result;
        }

        public bool AppPoolExists(string appPoolName)
        {
            var appPools = ListAppPools();

            return appPools.Any(x => string.Equals(x.Name, appPoolName, StringComparison.Ordinal));
        }

        public bool SiteExists(string siteName)
        {
            var sites = ListSites();

            return sites.Any(x => string.Equals(x.Name, siteName, StringComparison.Ordinal));
        }

        private static string GenerateAppId(Port httpsPort)
        {
            var httpsPortString = httpsPort.ToString();

            var zeroes = new string(EmptyAppId.Take(EmptyAppId.Length - httpsPortString.Length).ToArray());

            return Guid.Parse(zeroes + httpsPortString).ToString();
        }

        private static void ValidateSiteName(string siteName)
        {
            if (!IsValidSiteName(siteName))
            {
                throw new SiteManagementException($"The site name '{siteName}' is invalid.");
            }
        }

        private static bool IsValidSiteName(string siteName)
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