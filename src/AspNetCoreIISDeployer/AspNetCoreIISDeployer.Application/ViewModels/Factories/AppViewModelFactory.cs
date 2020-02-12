using System;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.Git;

namespace AspNetCoreIISDeployer.Application.ViewModels.Factories
{
    public class AppViewModelFactory : IAppViewModelFactory
    {
        private readonly ISiteService siteService;
        private readonly IGitService gitService;

        public AppViewModelFactory(ISiteService siteService, IGitService gitService)
        {
            this.siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
        }

        public AppViewModel Create(AppModel appModel)
        {
            if (appModel is null)
            {
                throw new ArgumentNullException(nameof(appModel));
            }

            return new AppViewModel(siteService, gitService, appModel);
        }
    }
}