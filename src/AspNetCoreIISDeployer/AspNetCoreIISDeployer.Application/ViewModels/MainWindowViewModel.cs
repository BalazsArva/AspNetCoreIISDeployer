using System.Security.Principal;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;
using AspNetCoreIISDeployer.Application.ViewModels.Factories;

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
            IAppService appService = new AppService();

            IAppViewModelFactory appViewModelFactory = new AppViewModelFactory(publishService, siteManagementService, gitService);

            AppList = new AppListViewModel(appViewModelFactory, appService);
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

        public string UserMode
        {
            get
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)
                    ? "Admin"
                    : "Normal user";
            }
        }
    }
}