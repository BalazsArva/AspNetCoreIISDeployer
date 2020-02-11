using System;
using System.Windows;
using AspNetCoreIISDeployer.Application.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreIISDeployer.Application
{
    public partial class App : System.Windows.Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            serviceProvider = ServiceConfiguration.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();
        }
    }
}