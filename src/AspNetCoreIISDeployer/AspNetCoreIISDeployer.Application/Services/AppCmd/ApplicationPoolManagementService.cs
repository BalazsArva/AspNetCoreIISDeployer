﻿using AspNetCoreIISDeployer.Application.Configuration;

namespace AspNetCoreIISDeployer.Application.Services.AppCmd
{
    public class ApplicationPoolManagementService : AppCmdServiceBase
    {
        public ApplicationPoolManagementService(AppCmdConfiguration configuration) : base(configuration)
        {
        }

        public CommandLineProcessResult Start(string appPoolName)
        {
            var arguments = $"start apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult Stop(string appPoolName)
        {
            var arguments = $"stop apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult Create(string appPoolName)
        {
            var arguments = $"add apppool /name:{appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }

        public CommandLineProcessResult Delete(string appPoolName)
        {
            var arguments = $"delete apppool {appPoolName}";

            return ExecuteAppCmdCommand(arguments);
        }
    }
}