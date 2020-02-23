namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public interface ISiteManagementService
    {
        CommandLineProcessResult Create(string appPoolName, string siteName, Port httpPort, Port httpsPort, string certificateThumbprint, string publishPath);

        CommandLineProcessResult Create(string appPoolName, string siteName, Port httpPort, string publishPath);

        CommandLineProcessResult Create(string appPoolName, string siteName, Port httpsPort, string certificateThumbprint, string publishPath);

        CommandLineProcessResult Delete(string siteName);

        CommandLineProcessResult Start(string siteName);

        CommandLineProcessResult Stop(string siteName);

        CommandLineProcessResult UnbindCertificateFromSite(Port httpsPort);

        CommandLineProcessResult CreateAppPool(string appPoolName);

        CommandLineProcessResult DeleteAppPool(string appPoolName);

        CommandLineProcessResult SetServerAutoStart(string siteName, bool autoStart);

        bool AppPoolExists(string appPoolName);

        bool SiteExists(string siteName);

        string GetBoundCertificateHash(Port httpsPort);
    }
}