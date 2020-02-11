using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;

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

        private readonly IDotNetPublishService publishService;
        private readonly ISiteManagementService siteManagementService;
        private readonly IGitService gitService;

        private bool enableSiteManagement = true;
        private bool enableRepositoryManagement = true;

        private PublishInfoViewModel publishInfo = new PublishInfoViewModel();
        private RepositoryInfoViewModel repositoryInfo = new RepositoryInfoViewModel();

        public AppViewModel(IDotNetPublishService publishService, ISiteManagementService siteManagementService, IGitService gitService, AppModel appModel)
        {
            publishAppCommand = new DelegateCommand(PublishApp);
            stopSiteCommand = new DelegateCommand(StopSite);
            startSiteCommand = new DelegateCommand(StartSite);
            restartSiteCommand = new DelegateCommand(RestartSite);
            createSiteCommand = new DelegateCommand(CreateSite);
            updateRepositoryInfoCommand = new DelegateCommand(_ => UpdateRepositoryInfo());

            fetchCommand = new DelegateCommand(FetchRepository);

            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));
            this.gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));

            AppModel = appModel;

            UpdatePublishInfo();
            UpdateRepositoryInfo();
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

        private void PublishApp(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                publishService.Publish(AppModel.ProjectPath, AppModel.BuildConfiguration, AppModel.PublishPath, AppModel.Environment);

                UpdatePublishInfo();
            });
        }

        private void StopSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Stop(AppModel.SiteName);

                UpdatePublishInfo();
            });
        }

        private void StartSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Start(AppModel.SiteName);

                UpdatePublishInfo();
            });
        }

        private void RestartSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Stop(AppModel.SiteName);
                siteManagementService.Start(AppModel.SiteName);

                UpdatePublishInfo();
            });
        }

        private void CreateSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Create(AppModel.AppPoolName, AppModel.SiteName, AppModel.HttpPort, AppModel.HttpsPort, AppModel.CertificateThumbprint, AppModel.PublishPath);
            });
        }

        private void FetchRepository(object _)
        {
            BackgroundInvokeRepositoryCommand(() =>
            {
                var projectDirectory = Path.GetDirectoryName(AppModel.ProjectPath);

                gitService.Fetch(projectDirectory, true, true);
            });
        }

        private async void BackgroundInvokeManagementCommand(Action command)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

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
                EnableSiteManagement = true;
            }
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

        private void UpdatePublishInfo()
        {
            var publishedAppInfo = publishService.GetGitPublishInfo(AppModel.PublishPath);

            PublishInfo.Branch = publishedAppInfo.Branch;
            PublishInfo.Commit = publishedAppInfo.Commit;
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