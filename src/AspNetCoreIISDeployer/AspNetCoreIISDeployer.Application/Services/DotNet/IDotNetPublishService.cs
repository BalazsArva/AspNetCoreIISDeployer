namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public interface IDotNetPublishService
    {
        CommandLineProcessResult Publish(string projectPath, string configuration, string outputDirectory, string environment = null);

        GitPublishInfo GetGitPublishInfo(string publishPath);
    }
}