using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
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
            var gitConfig = new GitConfiguration();

            IGitService gitService = new GitService(gitConfig);
            IDotNetPublishService publishService = new DotNetPublishService(dotNetConfig, gitService);
            ISiteManagementService siteManagementService = new SiteManagementService(iisConfig);

            AppList = new AppListViewModel(publishService, siteManagementService, gitService);
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