﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AspNetCoreIISDeployer.Application.Models;
using AspNetCoreIISDeployer.Application.Services.DotNet;
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

        private readonly IDotNetPublishService publishService;
        private readonly ISiteManagementService siteManagementService;

        private bool enableManagement = true;

        public AppViewModel(IDotNetPublishService publishService, ISiteManagementService siteManagementService, AppModel appModel)
        {
            publishAppCommand = new DelegateCommand(PublishApp);
            stopSiteCommand = new DelegateCommand(StopSite);
            startSiteCommand = new DelegateCommand(StartSite);
            restartSiteCommand = new DelegateCommand(RestartSite);
            createSiteCommand = new DelegateCommand(CreateSite);

            this.publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
            this.siteManagementService = siteManagementService ?? throw new ArgumentNullException(nameof(siteManagementService));

            AppModel = appModel;
        }

        public ICommand PublishAppCommand => publishAppCommand;

        public ICommand StopSiteCommand => stopSiteCommand;

        public ICommand StartSiteCommand => startSiteCommand;

        public ICommand RestartSiteCommand => restartSiteCommand;

        public ICommand CreateSiteCommand => createSiteCommand;

        public AppModel AppModel { get; }

        public bool EnableManagement
        {
            get { return enableManagement; }
            set
            {
                if (enableManagement != value)
                {
                    enableManagement = value;

                    NotifyPropertyChanged(nameof(EnableManagement));
                }
            }
        }

        private void PublishApp(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                publishService.Publish(AppModel.ProjectPath, AppModel.BuildConfiguration, AppModel.PublishPath, AppModel.Environment);
            });
        }

        private void StopSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Stop(AppModel.SiteName);
            });
        }

        private void StartSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Start(AppModel.SiteName);
            });
        }

        private void RestartSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Stop(AppModel.SiteName);
                siteManagementService.Start(AppModel.SiteName);
            });
        }

        private void CreateSite(object _)
        {
            BackgroundInvokeManagementCommand(() =>
            {
                siteManagementService.Create(AppModel.AppPoolName, AppModel.SiteName, AppModel.HttpPort, AppModel.HttpsPort, AppModel.CertificateThumbprint, AppModel.PublishPath);
            });
        }

        private async void BackgroundInvokeManagementCommand(Action command)
        {
            // TODO: Display output somewhere
            try
            {
                EnableManagement = false;

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
                EnableManagement = true;
            }
        }
    }
}