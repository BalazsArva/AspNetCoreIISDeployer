using System.Collections.ObjectModel;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppListViewModel : ViewModelBase
    {
        private ObservableCollection<AppViewModel> apps;

        public AppListViewModel(IDotNetPublishService publishService, ISiteManagementService siteManagementService, IGitService gitService)
        {
            Apps = new ObservableCollection<AppViewModel>(new[]
            {
                new AppViewModel(
                    publishService,
                    siteManagementService,
                    gitService,
                    new AppModel
                    {
                        ProjectPath = @"F:\Dev\LearningOpenIdConnect\Oidc.IdentityProvider\Oidc.IdentityProvider\Oidc.IdentityProvider.Api\Oidc.IdentityProvider.Api.csproj",
                        AppPoolName = "identity-provider-api1",
                        SiteName = "identity-provider-api1",
                        BuildConfiguration = "Debug",
                        Environment = "Development",
                        HttpPort = 8011,
                        HttpsPort = 44311,
                        CertificateThumbprint = "7288558f67603405132b4f9328c155719d40f08e",
                        PublishPath = @"C:\Temp\Oidc.Idp\Publish1"
                    }),
                new AppViewModel(
                    publishService,
                    siteManagementService,
                    gitService,
                    new AppModel
                    {
                        ProjectPath = @"F:\Dev\LearningOpenIdConnect\Oidc.IdentityProvider\Oidc.IdentityProvider\Oidc.IdentityProvider.Api\Oidc.IdentityProvider.Api.csproj",
                        AppPoolName = "identity-provider-api2",
                        SiteName = "identity-provider-api2",
                        BuildConfiguration = "Debug",
                        Environment = "Development",
                        HttpPort = 8012,
                        HttpsPort = 44312,
                        CertificateThumbprint = "7288558f67603405132b4f9328c155719d40f08e",
                        PublishPath = @"C:\Temp\Oidc.Idp\Publish2"
                    }),
                new AppViewModel(
                    publishService,
                    siteManagementService,
                    gitService,
                    new AppModel
                    {
                        ProjectPath = @"F:\Dev\LearningOpenIdConnect\Oidc.IdentityProvider\Oidc.IdentityProvider\Oidc.IdentityProvider.Api\Oidc.IdentityProvider.Api.csproj",
                        AppPoolName = "identity-provider-api3",
                        SiteName = "identity-provider-api3",
                        BuildConfiguration = "Debug",
                        Environment = "Development",
                        HttpPort = 8013,
                        HttpsPort = 44313,
                        CertificateThumbprint = "7288558f67603405132b4f9328c155719d40f08e",
                        PublishPath = @"C:\Temp\Oidc.Idp\Publish3"
                    }),
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