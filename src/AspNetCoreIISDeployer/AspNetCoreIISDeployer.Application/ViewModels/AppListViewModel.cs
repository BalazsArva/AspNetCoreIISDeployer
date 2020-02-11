using System;
using System.Collections.ObjectModel;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppListViewModel : ViewModelBase
    {
        private readonly IDotNetPublishService publishService;
        private readonly ISiteManagementService siteManagementService;
        private readonly IGitService gitService;
        private readonly IAppService appService;

        private ObservableCollection<AppViewModel> apps = new ObservableCollection<AppViewModel>();

        public AppListViewModel(IDotNetPublishService publishService, ISiteManagementService siteManagementService, IGitService gitService, IAppService appService)
        {
            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
            this.appService = appService ?? throw new ArgumentNullException(nameof(appService));

            Initialize();
        }

        public ObservableCollection<AppViewModel> Apps
        {
            get { return apps; }
            set
            {
                if (apps == value)
                {
                    return;
                }

                apps = value;
                NotifyPropertyChanged(nameof(Apps));
            }
        }

        private async void Initialize()
        {
            try
            {
                // TODO: Move file names elsewhere
                var configuredAppList = await appService.GetAppsAsync("globalconfig.json", "useroverrides.json");

                foreach (var app in configuredAppList.Apps)
                {
                    Apps.Add(new AppViewModel(publishService, siteManagementService, gitService, app));
                }
            }
            catch
            {
                // TODO: Display error
            }
        }
    }
}