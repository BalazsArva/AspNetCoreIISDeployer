namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public interface INotificationService
    {
        void NotifyError(string title, string message);
    }
}