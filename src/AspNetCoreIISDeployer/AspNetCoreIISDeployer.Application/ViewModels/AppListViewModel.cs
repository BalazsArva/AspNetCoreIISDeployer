using System;
using System.Collections.ObjectModel;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.ViewModels.Factories;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppListViewModel : ViewModelBase
    {
        private readonly IAppViewModelFactory appViewModelFactory;
        private readonly IAppService appService;

        private ObservableCollection<AppViewModel> apps = new ObservableCollection<AppViewModel>();

        public AppListViewModel(IAppViewModelFactory appViewModelFactory, IAppService appService)
        {
            this.appViewModelFactory = appViewModelFactory ?? throw new ArgumentNullException(nameof(appViewModelFactory));
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
                    Apps.Add(appViewModelFactory.Create(app));
                }
            }
            catch
            {
                // TODO: Display error
            }
        }
    }
}