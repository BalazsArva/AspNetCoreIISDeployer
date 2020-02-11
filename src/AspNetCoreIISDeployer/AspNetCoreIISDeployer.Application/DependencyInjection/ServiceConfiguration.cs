using System;
using AspNetCoreIISDeployer.Application.Configuration;
using AspNetCoreIISDeployer.Application.Services.ApplicationServices;
using AspNetCoreIISDeployer.Application.Services.DotNet;
using AspNetCoreIISDeployer.Application.Services.Git;
using AspNetCoreIISDeployer.Application.Services.IIS;
using AspNetCoreIISDeployer.Application.ViewModels;
using AspNetCoreIISDeployer.Application.ViewModels.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreIISDeployer.Application.DependencyInjection
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            var dotNetConfig = new DotNetConfiguration();
            var iisConfig = new IISMangementConfiguration();
            var gitConfig = new GitConfiguration();

            services
                .AddSingleton(dotNetConfig)
                .AddSingleton(iisConfig)
                .AddSingleton(gitConfig);

            services
                .AddSingleton<IAppService, AppService>()
                .AddSingleton<IGitService, GitService>()
                .AddSingleton<IDotNetPublishService, DotNetPublishService>()
                .AddSingleton<ISiteManagementService, SiteManagementService>();

            services
                .AddSingleton<MainWindowViewModel>();

            services
                .AddSingleton<IAppViewModelFactory, AppViewModelFactory>()
                .AddSingleton<IAppListViewModelFactory, AppListViewModelFactory>();

            services
                .AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}