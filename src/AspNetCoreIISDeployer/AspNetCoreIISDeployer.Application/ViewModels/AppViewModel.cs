using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.Git;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class AppViewModel : ViewModelBase
    {
        private readonly DelegateCommand publishAppCommand;
        private readonly DelegateCommand stopSiteCommand;
        private readonly DelegateCommand startSiteCommand;
        private readonly DelegateCommand restartSiteCommand;
        private readonly DelegateCommand createSiteCommand;
        private readonly DelegateCommand updateRepositoryInfoCommand;

        private readonly DelegateCommand fetchCommand;

        private readonly ISiteService siteService;
        private readonly IGitService gitService;

        private bool enableSiteManagement = true;
        private bool enableRepositoryManagement = true;

        private PublishInfoViewModel publishInfo = new PublishInfoViewModel();
        private RepositoryInfoViewModel repositoryInfo = new RepositoryInfoViewModel();

        public AppViewModel(ISiteService siteService, IGitService gitService, AppModel appModel)
        {
            publishAppCommand = new DelegateCommand(PublishApp);
            stopSiteCommand = new DelegateCommand(StopSite);
            startSiteCommand = new DelegateCommand(StartSite);
            restartSiteCommand = new DelegateCommand(RestartSite);
            createSiteCommand = new DelegateCommand(CreateSite);
            updateRepositoryInfoCommand = new DelegateCommand(_ => UpdateRepositoryInfo());

            fetchCommand = new DelegateCommand(FetchRepository);

            this.siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));

            AppModel = appModel;

            siteService.SubscribeToSiteUpdated(appModel.SiteName, UpdatePublishInfoAsync);

            UpdateRepositoryInfo();
            Initialize();
        }

        public ICommand PublishAppCommand => publishAppCommand;

        public ICommand StopSiteCommand => stopSiteCommand;

        public ICommand StartSiteCommand => startSiteCommand;

        public ICommand RestartSiteCommand => restartSiteCommand;

        public ICommand CreateSiteCommand => createSiteCommand;

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
            try
            {
                await UpdatePublishInfoAsync();
            }
            catch
            {
                // TODO: Show error
            }
        }

        private async Task UpdatePublishInfoAsync()
        {
            var publishedAppInfo = await siteService.GetGitPublishInfoAsync(AppModel.PublishPath);

            PublishInfo.Branch = publishedAppInfo.Branch;
            PublishInfo.Commit = publishedAppInfo.Commit;
        }

        private async void PublishApp(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.PublishAppToSiteAsync(AppModel);
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private async void StopSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.StopSiteAsync(AppModel.SiteName);
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private async void StartSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.StartSiteAsync(AppModel.SiteName);
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private async void RestartSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.RestartSiteAsync(AppModel.SiteName);
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private async void CreateSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.CreateSiteAsync(AppModel);
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableSiteManagement = true;
            }
        }

        private void FetchRepository(object _)
        {
            BackgroundInvokeRepositoryCommand(() =>
            {
                var projectDirectory = Path.GetDirectoryName(AppModel.ProjectPath);

                gitService.Fetch(projectDirectory, true, true);
            });
        }

        private async void BackgroundInvokeRepositoryCommand(Action command)
        {
            // TODO: Display output somewhere
            try
            {
                EnableRepositoryManagement = false;

                await Task.Run(() =>
                {
                    command();
                });
            }
            catch
            {
                // TODO: Show eror
            }
            finally
            {
                EnableRepositoryManagement = true;
            }
        }

        private void UpdateRepositoryInfo()
        {
            var repositoryPath = Path.GetDirectoryName(AppModel.ProjectPath);

            var branch = gitService.GetCurrentBranch(repositoryPath);
            var commit = gitService.GetCurrentCommitHash(repositoryPath);

            RepositoryInfo.Branch = branch;
            RepositoryInfo.Commit = commit;
        }
    }
}