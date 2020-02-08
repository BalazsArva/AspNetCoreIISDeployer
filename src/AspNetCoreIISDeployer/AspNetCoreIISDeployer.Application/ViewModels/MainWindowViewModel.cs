using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AppListViewModel appList;

        public MainWindowViewModel()
        {
            var dotNetConfig = new DotNetConfiguration();
            var iisConfig = new IISMangementConfiguration();

            IDotNetPublishService publishService = new DotNetPublishService(dotNetConfig);
            ISiteManagementService siteManagementService = new SiteManagementService(iisConfig);

            AppList = new AppListViewModel(publishService, siteManagementService);
        }

        public AppListViewModel AppList
        {
            get { return appList; }
            set
            {
                if (appList == value)
                {
                    return;
                }

                appList = value;
                NotifyPropertyChanged(nameof(AppList));
            }
        }
    }
}