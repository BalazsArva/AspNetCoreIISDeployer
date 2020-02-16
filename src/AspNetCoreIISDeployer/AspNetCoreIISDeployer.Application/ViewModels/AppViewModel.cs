﻿using System;
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

        public AppViewModel(INotificationService notificationService, ISiteService siteService, IRepositoryService repositoryService, AppModel appModel)
        {
            AppModel = appModel;

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

                await UpdateRepositoryInfoAsync();
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

        private async Task UpdateRepositoryInfoAsync()
        {
            var repositoryPath = Path.GetDirectoryName(AppModel.ProjectPath);
            var repositoryInfo = await repositoryService.GetRepositoryInfoAsync(repositoryPath);

            RepositoryInfo.Branch = repositoryInfo.Branch;
            RepositoryInfo.Commit = repositoryInfo.Commit;
            RepositoryInfo.RemoteCommit = repositoryInfo.RemoteCommit;
        }

        private async void PublishApp(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.PublishAppToSiteAsync(AppModel);
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

        private async void StopSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.StopSiteAsync(AppModel.SiteName);
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

        private async void StartSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.StartSiteAsync(AppModel.SiteName);
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

        private async void RestartSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.RestartSiteAsync(AppModel.SiteName);
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

        private async void CreateSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.CreateSiteAsync(AppModel);
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

        private async void DeleteSite(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableSiteManagement = false;

                await siteService.DeleteSiteAsync(AppModel);
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

        private async void FetchRepository(object _)
        {
            // TODO: Display output somewhere
            try
            {
                EnableRepositoryManagement = false;

                var repositoryPath = repositoryService.FindRepositoryRoot(AppModel.ProjectPath);

                await repositoryService.FetchAsync(repositoryPath, true, true);
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