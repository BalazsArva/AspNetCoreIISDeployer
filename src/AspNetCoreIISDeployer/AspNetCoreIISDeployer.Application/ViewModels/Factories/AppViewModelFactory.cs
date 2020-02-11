using System;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.ViewModels.Factories
{
    public class AppViewModelFactory : IAppViewModelFactory
    {
        private readonly IDotNetPublishService publishService;
        private readonly ISiteManagementService siteManagementService;
        private readonly IGitService gitService;

        public AppViewModelFactory(IDotNetPublishService publishService, ISiteManagementService siteManagementService, IGitService gitService)
        {
            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        }

        public AppViewModel Create(AppModel appModel)
        {
            if (appModel is null)
            {
                throw new ArgumentNullException(nameof(appModel));
            }

            return new AppViewModel(publishService, siteManagementService, gitService, appModel);
        }
    }
}