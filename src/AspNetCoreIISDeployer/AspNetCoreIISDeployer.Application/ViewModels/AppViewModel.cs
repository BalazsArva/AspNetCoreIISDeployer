using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppViewModel : ViewModelBase
    {
        private readonly DelegateCommand publishAppCommand;
        private readonly DelegateCommand stopSiteCommand;
        private readonly DelegateCommand startSiteCommand;
        private readonly DelegateCommand restartSiteCommand;
        private readonly DelegateCommand createSiteCommand;
        private readonly DelegateCommand deleteSiteCommand;

        private readonly DelegateCommand updateRepositoryInfoCommand;
        private readonly DelegateCommand fetchCommand;
        private readonly INotificationService notificationService;
        private readonly ISiteService siteService;
        private readonly IRepositoryService repositoryService;

        private bool enableSiteManagement = true;
        private bool enableRepositoryManagement = true;

        private PublishInfoViewModel publishInfo = new PublishInfoViewModel();
        private RepositoryInfoViewModel repositoryInfo = new RepositoryInfoViewModel();
        private SiteInfoViewModel siteInfo = new SiteInfoViewModel();

        public AppViewModel(INotificationService notificationService, ISiteService siteService, IRepositoryService repositoryService, AppModel appModel)
        {
            AppModel = appModel ?? throw new ArgumentNullException(nameof(appModel));

            publishAppCommand = new DelegateCommand(PublishApp);
            stopSiteCommand = new DelegateCommand(StopSite);
            startSiteCommand = new DelegateCommand(StartSite);
            restartSiteCommand = new DelegateCommand(RestartSite);
            createSiteCommand = new DelegateCommand(CreateSite);
            deleteSiteCommand = new DelegateCommand(DeleteSite);

            updateRepositoryInfoCommand = new DelegateCommand(async _ => await UpdateRepositoryInfoAsync());
            fetchCommand = new DelegateCommand(FetchRepository);

            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            this.repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));

            var repositoryPath = repositoryService.FindRepositoryRoot(appModel.ProjectPath);
            repositoryService.SubscribeToRepositoryUpdated(repositoryPath, UpdateRepositoryInfoAsync);
            siteService.SubscribeToSiteUpdated(appModel.SiteName, UpdatePublishInfoAsync);

            Initialize();
        }

        public ICommand PublishAppCommand => publishAppCommand;

        public ICommand StopSiteCommand => stopSiteCommand;

        public ICommand StartSiteCommand => startSiteCommand;

        public ICommand RestartSiteCommand => restartSiteCommand;

        public ICommand CreateSiteCommand => createSiteCommand;

        public ICommand DeleteSiteCommand => deleteSiteCommand;

        public ICommand UpdateRepositoryInfoCommand => updateRepositoryInfoCommand;

        public ICommand FetchCommand => fetchCommand;

        public AppModel AppModel { get; }

        public RepositoryInfoViewModel RepositoryInfo
        {
            get { return repositoryInfo; }
            set
            {
                if (repositoryInfo != value)
                {
                    repositoryInfo = value;

                    NotifyPropertyChanged(nameof(RepositoryInfo));
                }
            }
        }

        public PublishInfoViewModel PublishInfo
        {
            get { return publishInfo; }
            set
            {
                if (publishInfo != value)
                {
                    publishInfo = value;

                    NotifyPropertyChanged(nameof(PublishInfo));
                }
            }
        }

        public SiteInfoViewModel SiteInfo
        {
            get { return siteInfo; }
            set
            {
                if (siteInfo != value)
                {
                    siteInfo = value;

                    NotifyPropertyChanged(nameof(SiteInfo));
                }
            }
        }

        public bool EnableSiteManagement
        {
            get { return enableSiteManagement; }
            set
            {
                if (enableSiteManagement != value)
                {
                    enableSiteManagement = value;

                    NotifyPropertyChanged(nameof(EnableSiteManagement));
                }
            }
        }

        public bool EnableRepositoryManagement
        {
            get { return enableRepositoryManagement; }
            set
            {
                if (enableRepositoryManagement != value)
                {
                    enableRepositoryManagement = value;

                    NotifyPropertyChanged(nameof(EnableRepositoryManagement));
                }
            }
        }

        private async void Initialize()
        {
            await UpdatePublishInfoAsync();
            await UpdateRepositoryInfoAsync();
            await UpdateCertificateInfoAsync();
        }

        private async Task UpdateCertificateInfoAsync()
        {
            var certificateHash = await siteService.GetBoundCertificateHashAsync(AppModel);

            SiteInfo.CertificateThumbprint = certificateHash;
        }

        private async Task UpdatePublishInfoAsync()
        {
            try
            {
                var publishedAppInfo = await siteService.GetGitPublishInfoAsync(AppModel.PublishPath);

                PublishInfo.Branch = publishedAppInfo.Branch;
                PublishInfo.Commit = publishedAppInfo.Commit;
            }
            catch (Exception e)
            {
                notificationService.NotifyError("Error", e.Message);
            }
        }

        private async Task UpdateRepositoryInfoAsync()
        {
            try
            {
                var repositoryPath = Path.GetDirectoryName(AppModel.ProjectPath);
                var repositoryInfo = await repositoryService.GetRepositoryInfoAsync(repositoryPath);

                RepositoryInfo.Branch = repositoryInfo.Branch;
                RepositoryInfo.Commit = repositoryInfo.Commit;
                RepositoryInfo.RemoteCommit = repositoryInfo.RemoteCommit;
            }
            catch (Exception e)
            {
                notificationService.NotifyError("Error", e.Message);
            }
        }

        private async void PublishApp(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.PublishAppToSiteAsync(AppModel));
        }

        private async void StopSite(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.StopSiteAsync(AppModel.SiteName));
        }

        private async void StartSite(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.StartSiteAsync(AppModel.SiteName));
        }

        private async void RestartSite(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.RestartSiteAsync(AppModel.SiteName));
        }

        private async void CreateSite(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.CreateSiteAsync(AppModel));
        }

        private async void DeleteSite(object _)
        {
            await SafeInvokeSiteManagementCommandAsync(() => siteService.DeleteSiteAsync(AppModel));
        }

        private async void FetchRepository(object _)
        {
            await SafeInvokeRepositoryManagementCommandAsync(() =>
            {
                var repositoryPath = repositoryService.FindRepositoryRoot(AppModel.ProjectPath);

                return repositoryService.FetchAsync(repositoryPath, true, true);
            });
        }

        private async Task SafeInvokeSiteManagementCommandAsync(Func<Task> task)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await task();
            }
            catch (Exception e)
            {
                notificationService.NotifyError("Error", e.Message);
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private async Task SafeInvokeRepositoryManagementCommandAsync(Func<Task> task)
        {
            // TODO: Display output somewhere
            try
            {
                EnableRepositoryManagement = false;

                await task();
            }
            catch (Exception e)
            {
                notificationService.NotifyError("Error", e.Message);
            }
            finally
            {
                EnableRepositoryManagement = true;
            }
        }
    }
}