using System.Windows;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        public void NotifyError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}