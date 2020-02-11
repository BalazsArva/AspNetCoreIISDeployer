using System;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;

namespace AspNetCoreIISDeployer.Application.ViewModels.Factories
{
    public class AppListViewModelFactory : IAppListViewModelFactory
    {
        private readonly IAppViewModelFactory appViewModelFactory;
        private readonly IAppService appService;

        public AppListViewModelFactory(IAppViewModelFactory appViewModelFactory, IAppService appService)
        {
            this.appViewModelFactory = appViewModelFactory ?? throw new ArgumentNullException(nameof(appViewModelFactory));
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public AppListViewModel Create()
        {
            return new AppListViewModel(appViewModelFactory, appService);
        }
    }
}