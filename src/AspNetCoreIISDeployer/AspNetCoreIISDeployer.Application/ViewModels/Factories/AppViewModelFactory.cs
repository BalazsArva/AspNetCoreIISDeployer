using System;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;

namespace AspNetCoreIISDeployer.Application.ViewModels.Factories
{
    public class AppViewModelFactory : IAppViewModelFactory
    {
        private readonly INotificationService notificationService;
        private readonly ISiteService siteService;
        private readonly IRepositoryService repositoryService;

        public AppViewModelFactory(INotificationService notificationService, ISiteService siteService, IRepositoryService repositoryService)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            this.repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
        }

        public AppViewModel Create(AppModel appModel)
        {
            if (appModel is null)
            {
                throw new ArgumentNullException(nameof(appModel));
            }

            return new AppViewModel(notificationService, siteService, repositoryService, appModel);
        }
    }
}