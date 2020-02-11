using System.Windows;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;
using AspNetCoreIISDeployer.Application.ViewModels;
using AspNetCoreIISDeployer.Application.ViewModels.Factories;

namespace AspNetCoreIISDeployer.Application
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dotNetConfig = new DotNetConfiguration();
            var iisConfig = new IISMangementConfiguration();
            var gitConfig = new GitConfiguration();

            IGitService gitService = new GitService(gitConfig);
            IDotNetPublishService publishService = new DotNetPublishService(dotNetConfig, gitService);
            ISiteManagementService siteManagementService = new SiteManagementService(iisConfig);

            IAppViewModelFactory appViewModelFactory = new AppViewModelFactory(publishService, siteManagementService, gitService);
            IAppService appService = new AppService();

            IAppListViewModelFactory appListViewModelFactory = new AppListViewModelFactory(appViewModelFactory, appService);

            MainWindow = new MainWindow(new MainWindowViewModel(appListViewModelFactory));
            MainWindow.Show();
        }
    }
}