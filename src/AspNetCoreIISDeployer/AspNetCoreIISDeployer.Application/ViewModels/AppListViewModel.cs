using System.Collections.ObjectModel;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppListViewModel : ViewModelBase
    {
        private ObservableCollection<AppViewModel> apps;

        public AppListViewModel(IDotNetPublishService publishService, ISiteManagementService siteManagementService)
        {
            Apps = new ObservableCollection<AppViewModel>(new[]
            {
                new AppViewModel(
                    publishService,
                    siteManagementService,
                    new AppModel
                    {
                        ProjectPath = @"F:\Dev\LearningOpenIdConnect\Oidc.IdentityProvider\Oidc.IdentityProvider\Oidc.IdentityProvider.Api\Oidc.IdentityProvider.Api.csproj",
                        AppPoolName = "identity-provider-api",
                        SiteName = "identity-provider-api",
                        BuildConfiguration = "Debug",
                        Environment = "Development",
                        HttpPort = 8010,
                        HttpsPort = 44310,
                        CertificateThumbprint = "7288558f67603405132b4f9328c155719d40f08e",
                        PublishPath = @"C:\Temp\Oidc.Idp\Publish"
                    })
            });
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
    }
}